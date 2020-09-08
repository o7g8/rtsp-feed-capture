using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace work_manager_akkanet
{
    internal class WorkManager : UntypedActor
    {
        private readonly ActorSystem actorSystem;
        private readonly int maxWorkQueueSize;
        private readonly int syncBeatPeriodMs;
        private Dictionary<string, List<IActorRef>>  feedReaders;
        private Dictionary<string, IActorRef> frameStackers;
        private Dictionary<string, IActorRef> inferencers;
        private Dictionary<string, Queue<FrameStack>> inferenceWorkQueue;
        private Dictionary<string, Queue<MsgRequestInferenceJob>> inferenceAvailabilityQueue;
        private ICancelable sync;
        
        public WorkManager(Config config, ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;
            this.maxWorkQueueSize = config.MaxQueueSize;
            this.syncBeatPeriodMs = config.SyncBeat;
            feedReaders = CreateFeedReaders(config.Feeds);
            frameStackers = CreateFrameStackers(config.Models);
            inferencers = CreateInferencers(config.Models);
            this.inferenceWorkQueue = inferencers.Keys.ToDictionary(x => x, x => new Queue<FrameStack>(maxWorkQueueSize));
            this.inferenceAvailabilityQueue = CreateInferenceAvailabilityQueue(inferencers.Keys);
        }

        private Dictionary<string, Queue<MsgRequestInferenceJob>> CreateInferenceAvailabilityQueue(IEnumerable<string> models)
        {
            var result = new Dictionary<string, Queue<MsgRequestInferenceJob>>();
            foreach(var model in models) {
                result[model] = new Queue<MsgRequestInferenceJob>();
                result[model].Enqueue(new MsgRequestInferenceJob(model));
            }
            return result;
        }

        protected override void PreStart() {
            this.sync = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                0, syncBeatPeriodMs, Self, new MsgSync(), Self
            );
        }

        private Dictionary<string, IActorRef> CreateInferencers(Model[] models)
        {
            return models.Select(model => new {
                Key = model.ModelName,
                Value = actorSystem.ActorOf(Props.Create<Inferencer>(Self, model.ModelName, model.debugInferenceTimeMs))
            }).ToDictionary(x => x.Key, x => x.Value);
        }

        private Dictionary<string, IActorRef> CreateFrameStackers(Model[] models)
        {
            return models.Select(model => new {
                Key = model.ModelName,
                Value = actorSystem.ActorOf(Props.Create<FrameStacker>(Self, model.ModelName, model.FramesInStack, model.debugStackingTimeMs))
            }).ToDictionary(x => x.Key, x => x.Value);
        }

        private Dictionary<string, List<IActorRef>> CreateFeedReaders(Feed[] feeds)
        {
            return feeds.Select(feed => new {
                Key = feed.ModelName,
                Value = actorSystem.ActorOf(Props.Create<FeedReader>(Self, feed.Url, feed.ModelName, feed.debugCaptureTimeMs))
                }
            )
            .GroupBy(x => x.Key)
            .ToDictionary(x => x.Key, x => x.Select(g => g.Value).ToList());
        }

        protected override void OnReceive(object message)
        {
            switch(message) {
                case MsgProcessFrame frame:
                    ProcessFrame(frame);
                    break;
                case MsgStackReady stack:
                    ProcessStack(stack);
                    break;
                case MsgRequestInferenceJob inferenceRequest:
                    ProcessInferenceAvailabilityRequest(inferenceRequest);
                    break;
                case MsgSync syncBeat:
                    ProcessSyncBeat();
                    break;
                default:
                    break;
            }
        }

        private void ProcessSyncBeat()
        {
            foreach(var modelName in this.inferenceWorkQueue.Keys) {
                var workQueueLength = inferenceWorkQueue[modelName].Count();
                if(workQueueLength < maxWorkQueueSize) {
                    Console.WriteLine($"WorkManager: {modelName} syncbeat requesting frames (Q={workQueueLength}).");
                    feedReaders[modelName].ForEach(x => x.Tell(new MsgRequestFrame()));
                } else {
                    Console.WriteLine($"WorkManager: {modelName} syncbeat skipping frames (Q={workQueueLength}).");
                }
            }
        }

        private void ProcessInferenceAvailabilityRequest(MsgRequestInferenceJob inferenceRequest)
        {
            var workQueue = inferenceWorkQueue[inferenceRequest.ModelName];
            if(workQueue.Any()) {
                Console.WriteLine($"WorkManager: {inferenceRequest.ModelName} submitting inference (Q={workQueue.Count}).");
                var job = workQueue.Dequeue();
                inferencers[inferenceRequest.ModelName].Tell(new MsgInferenceJob {ModelName = job.ModelName, Stack = job.FramesStack });
            } else {
                Console.WriteLine($"WorkManager: {inferenceRequest.ModelName} requesting inference.");
                inferenceAvailabilityQueue[inferenceRequest.ModelName].Enqueue(inferenceRequest);
            }
        }

        private void ProcessStack(MsgStackReady stack)
        {
            Console.WriteLine($"WorkManager: {stack.ModelName} new frame stack (Q={inferenceWorkQueue[stack.ModelName].Count}, QA={inferenceAvailabilityQueue[stack.ModelName].Count})");
            if(inferenceAvailabilityQueue[stack.ModelName].Any()) {
                inferenceAvailabilityQueue[stack.ModelName].Dequeue();
                inferencers[stack.ModelName].Tell(new MsgInferenceJob {ModelName = stack.ModelName, Stack = stack.Stack });
            } else {
                inferenceWorkQueue[stack.ModelName].Enqueue(new FrameStack {ModelName = stack.ModelName});
            }
        }

        private void ProcessFrame(MsgProcessFrame frame)
        {
            Console.WriteLine($"WorkManager: {frame.ModelName} received frame from {frame.Url}");
            var stacker = frameStackers[frame.ModelName];
            stacker.Tell(new MsgStackFrame {ModelName = frame.ModelName, Url = frame.Url});
        }
    }
}