using System;
using Akka.Actor;

namespace rstp_capture_akkanet
{
    class Program
    {
        static void Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("rtsp");
            var frameProcessor = actorSystem.ActorOf<FrameProcessor>("frameProcessor");
            var feedReader = actorSystem.ActorOf(Props.Create<FeedReader>(frameProcessor), "feedReader");
            feedReader.Tell(new ProcessFeed { Url = "rtsp://127.0.0.1:8554/test" });

            Console.ReadKey();
            actorSystem.Stop(feedReader);
            actorSystem.Stop(frameProcessor);
        }
    }
}
