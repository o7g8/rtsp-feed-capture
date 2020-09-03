using System;
using System.Threading;
using Akka.Actor;

namespace work_manager_akkanet
{
    internal class FeedReader : UntypedActor
    {
        private readonly IActorRef workManager;
        private readonly string url;
        private readonly string modelName;
        private readonly int maxFPS;

        private readonly int debugCaptureTimeMs;
        private double FPS;
        private ICancelable schedulerCancellation;

        public FeedReader(IActorRef workManager, string url, string modelName, int maxFPS, int debugCaptureTimeMs)
        {
           this.workManager = workManager;
           this.url = url;
           this.modelName = modelName;
           this.maxFPS = maxFPS;
           this.FPS = maxFPS;
           this.debugCaptureTimeMs = debugCaptureTimeMs;
           this.schedulerCancellation = CreateFrameCaptureScheduler();
        }
        protected override void OnReceive(object message)
        {
            switch(message) {
                case MsgCaptureFrame frame:
                    CaptureFrame();
                    break;
                case MsgChangeRate rateChange:
                    ChangeRate(rateChange);
                    break;
                default:
                    break;
            }
        }

        private void ChangeRate(MsgChangeRate rateChange)
        {
            var newFPS = FPS * rateChange.RateChange;
            if(newFPS > maxFPS) {
                Console.WriteLine($"{url}: asked new rate {newFPS} > {maxFPS}, do nothing.");
            }
            schedulerCancellation.Cancel();
            FPS = newFPS;
            schedulerCancellation = CreateFrameCaptureScheduler();
            Console.WriteLine($"{url}: switched to rate {FPS}.");
        }

        private ICancelable CreateFrameCaptureScheduler()
        {
            return Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
               TimeSpan.FromMilliseconds(100),
               TimeSpan.FromMilliseconds(1000 / FPS),
               Self,
               new MsgCaptureFrame(),
               Self
           );
        }

        private void CaptureFrame()
        {
            Thread.Sleep(debugCaptureTimeMs);
            Console.WriteLine($"{url}: capture frame, FPS={FPS}");
            workManager.Tell(new MsgProcessFrame {ModelName = modelName, Url = url});
        }
    }
}