using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace work_manager_akkanet
{
    internal class WorkManager : UntypedActor
    {
        private readonly ActorSystem actorSystem;
        private readonly int maxQueueSize;
        private Dictionary<string, List<IActorRef>>  feedReaders;
        private Dictionary<string, IActorRef> frameStackers;
        private Dictionary<string, IActorRef> inferencers;
        private Dictionary<string, double> paceFactors;
        private Dictionary<string, Queue<FrameStack>> inferenceQueue;
        
        public WorkManager(Config config, ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;
            this.maxQueueSize = config.MaxQueueSize;
            feedReaders = CreateFeedReaders(config.Feeds);
            frameStackers = CreateFrameStackers(config.Models);
            inferencers = CreateInferencers(config.Models);
            this.inferenceQueue = inferencers.Keys.ToDictionary(x => x, x => new Queue<FrameStack>(maxQueueSize));
            paceFactors = inferencers.Keys.ToDictionary(x => x, x => 1.0);
        }

        private Dictionary<string, IActorRef> CreateInferencers(Model[] models)
        {
            return models.Select(model => new {
                Key = model.ModelName,
                Value = actorSystem.ActorOf(Props.Create<Inferencer>(model.ModelName, model.debugInferenceTimeMs))
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
                Value = actorSystem.ActorOf(Props.Create<FeedReader>(Self, feed.Url, feed.ModelName, feed.MaxFPS, feed.debugCaptureTimeMs))
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
                default:
                    break;
            }
        }

        private void ProcessStack(MsgStackReady stack)
        {
            Console.WriteLine($"WorkManager: {stack.ModelName} new frame stack ");
            inferenceQueue[stack.ModelName].Enqueue(new FrameStack {ModelName = stack.ModelName});
            Console.WriteLine($"WorkManager: {stack.ModelName} queue depth {inferenceQueue[stack.ModelName].Count}");
            var paceFactor = inferenceQueue[stack.ModelName].Count > maxQueueSize ? 0.5 : 1.2;
            ChangeFeedReadersPace(stack.ModelName, paceFactor);
        }

        private void ChangeFeedReadersPace(string modelName, double factor)
        {
            Console.WriteLine($"WorkManager: {modelName} old pace factor {paceFactors[modelName]}, new factor {factor}");
            paceFactors[modelName] = factor;
            feedReaders[modelName].ForEach(x => 
                x.Tell(new MsgChangeRate {RateChange = factor}));
        }

        private void ProcessFrame(MsgProcessFrame frame)
        {
            Console.WriteLine($"WorkManager: Received frame from {frame.Url}");
            var stacker = frameStackers[frame.ModelName];
            stacker.Tell(new MsgStackFrame {ModelName = frame.ModelName, Url = frame.Url});
        }
    }
}