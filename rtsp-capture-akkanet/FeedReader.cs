using System;
using Akka.Actor;
using OpenCvSharp;

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

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case ProcessFeed feed:
                    CaptureFeed(feed.Url, feed.Id);
                    break;
                default:
                    Console.WriteLine("Invalid message");
                    break;
            }
        }

        private void CaptureFeed(string url, int feedId)
        {
            // TODO: on the real webcam use https://getakka.net/articles/utilities/scheduler.html
            // to send a message itself every 1 sec.
            using (var capture = new VideoCapture(url))
            {
                var fps = (int)capture.Fps;
                Console.WriteLine($"Feed {feedId} FPS: {capture.Fps}");
                int i = -1;
                using (var image = new Mat())
                {
                    while (true)
                    {
                        capture.Grab();
                        i++;
                        if (i % fps != 0)
                        {
                            continue;
                        }

                        capture.Read(image);
                        if (image.Empty())
                        {
                            Console.WriteLine($"Feed {feedId}: end of video");
                            break;
                        }
                        
                        Console.WriteLine($"Feed {feedId}: sending frame {i}");
                        FrameProcessor.Tell(new ProcessFrame {Frame = image, FrameNo = i, Id = feedId});
                    }
                }
            }
        }
    }
}