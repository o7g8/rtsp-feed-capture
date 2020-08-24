using System;
using System.Threading;
using Akka.Actor;

namespace rstp_capture_akkanet
{
    internal class FeedReader : UntypedActor
    {
        private readonly IActorRef FrameProcessor;
        private long frameNo = 0;

        public FeedReader(IActorRef frameProcessor)
        {
            FrameProcessor = frameProcessor;
        }

        protected override void OnReceive(object message) {
            switch(message) {
                case ProcessFeed feed:
                    CaptureFeed(feed.Url);
                    break;
                default:
                    Console.WriteLine("Invalid message");
                    break;
            }
        }

        private void CaptureFeed(string url)
        {
            while(true) {
                Console.WriteLine($"Sending frame {frameNo}");
                FrameProcessor.Tell(new ProcessFrame {Frame = null, FrameNo = frameNo});
                Thread.Sleep(1000);
                frameNo++;
            }
        }
    }
}