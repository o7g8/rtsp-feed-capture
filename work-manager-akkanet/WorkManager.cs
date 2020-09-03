using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace work_manager_akkanet
{
    internal class WorkManager : UntypedActor
    {
        private readonly ActorSystem actorSystem;
        private IReadOnlyList<IActorRef> feedReaders;
        private Dictionary<string, IActorRef> frameStackers;
        private Dictionary<string, IActorRef> inferencers;
        
        public WorkManager(Config config, ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;
            feedReaders = CreateFeedReaders(config.Feeds);
            frameStackers = CreateFrameStackers(config.Models);
            inferencers = CreateInferencers(config.Models);
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

        private List<IActorRef> CreateFeedReaders(Feed[] feeds)
        {
            return feeds.Select(feed => 
                actorSystem.ActorOf(Props.Create<FeedReader>(Self, feed.Url, feed.ModelName, feed.MaxFPS, feed.debugCaptureTimeMs))
            ).ToList();
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

        }

        private void ProcessFrame(MsgProcessFrame frame)
        {
            Console.WriteLine($"Received frame from {frame.Url}");
            var stacker = frameStackers[frame.ModelName];
            stacker.Tell(new MsgStackFrame {ModelName = frame.ModelName, Url = frame.Url});
        }
    }
}