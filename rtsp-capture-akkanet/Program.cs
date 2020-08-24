using System;
using System.Linq;
using Akka.Actor;

namespace rstp_capture_akkanet
{
    class Program
    {
        static void Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("rtsp");
            var frameProcessor = actorSystem.ActorOf<FrameProcessor>("frameProcessor");
            var feedReaders = Enumerable
                .Range(1, 3)
                .Select(i => {
                    var feedReader = actorSystem.ActorOf(Props.Create<FeedReader>(frameProcessor), $"feedReader{i}");
                    feedReader.Tell(new ProcessFeed { Url = "rtsp://127.0.0.1:8554/test", Id = i });
                    return feedReader;
                })
                .ToList();

            Console.ReadKey();
            feedReaders.ForEach(x => actorSystem.Stop(x));
            actorSystem.Stop(frameProcessor);
        }
    }
}
