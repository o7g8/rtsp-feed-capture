using System;
using System.Threading.Tasks;
using Akka.Actor;

namespace work_manager_akkanet
{
    internal class FeedReader : ReceiveActor
    {
        private readonly IActorRef workManager;
        private readonly string url;
        private readonly string modelName;
        private readonly int debugCaptureTimeMs;

        public FeedReader(IActorRef workManager, string url, string modelName, int debugCaptureTimeMs)
        {
           this.workManager = workManager;
           this.url = url;
           this.modelName = modelName;
           this.debugCaptureTimeMs = debugCaptureTimeMs;
           ReceiveAsync<MsgRequestFrame>(m => ProcessRequestFrame());
        }

        private async Task ProcessRequestFrame()
        {
            Console.WriteLine($"{DateTime.Now} {url}: capture frame.");
            await Task.Delay(debugCaptureTimeMs);
            workManager.Tell(new MsgProcessFrame {ModelName = modelName, Url = url});
        }
    }
}