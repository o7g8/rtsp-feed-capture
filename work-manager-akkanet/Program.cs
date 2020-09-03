﻿using System;
using System.Linq;
using Akka.Actor;

/*
The Actor system is built of followig actors:

FeedReader <----[SetFPS]----- WorkManager --[InferStack]--> Inference
           --[ProcessFrame]->             <--[PullStack]---
                 |  ^
    [StackFrame] *  |[FramesStacked]
              FrameStacker
*/
namespace work_manager_akkanet
{
    class Program
    {
        private static readonly Config config = new Config {
            Models = new Model[] {
               new Model {
                    ModelName = "model1",
                    FramesInStack = 2,
                    debugInferenceTimeMs = 200,
                    debugStackingTimeMs = 10
                },
               new Model {
                   ModelName = "model2",
                   FramesInStack = 3,
                   debugInferenceTimeMs = 300,
                   debugStackingTimeMs = 15 },
            },
            Feeds = new Feed[] {
                new Feed {Url = "url1/model1", ModelName = "model1", MaxFPS = 1, debugCaptureTimeMs = 20 },
                new Feed {Url = "url2/model1", ModelName = "model1", MaxFPS = 1, debugCaptureTimeMs = 20 },
                new Feed {Url = "url3/model1", ModelName = "model1", MaxFPS = 1, debugCaptureTimeMs = 20 },
                new Feed {Url = "url4/model2", ModelName = "model2", MaxFPS = 1, debugCaptureTimeMs = 25 },
                new Feed {Url = "url5/model2", ModelName = "model2", MaxFPS = 1, debugCaptureTimeMs = 25 },
            }
        };
        static void Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("work_manager");
            var workManager = actorSystem.ActorOf(Props.Create<WorkManager>(config, actorSystem));
            // var workManager = actorSystem.ActorOf<WorkManager>("workManager");
            // CreateFeedReaders(config.Feeds, actorSystem, workManager);
            // CreateFrameStackers(config.Models, workManager);
            // CreateInference(config.Models, workManager);
            // var frameProcessor = actorSystem.ActorOf<FrameProcessor>("frameProcessor");
            // var feedReaders = Enumerable
            //     .Range(1, 10)
            //     .Select(i => {
            //         var feedReader = actorSystem.ActorOf(Props.Create<FeedReader>(frameProcessor), $"feedReader{i}");
            //         feedReader.Tell(new ProcessFeed { Url = "rtsp://127.0.0.1:8554/test", Id = i });
            //         return feedReader;
            //     })
            //     .ToList();

            Console.ReadKey();
            actorSystem.Stop(workManager);
        }
    }
}
