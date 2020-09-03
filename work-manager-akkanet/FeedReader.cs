using Akka.Actor;

namespace work_manager_akkanet
{
    internal class FeedReader : UntypedActor
    {
        public FeedReader(string url, string modelName, int maxFPS, int debugCaptureTimeMs)
        {
            
        }
        protected override void OnReceive(object message)
        {
            throw new System.NotImplementedException();
        }
    }
}