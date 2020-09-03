using Akka.Actor;

namespace work_manager_akkanet
{
    internal class FrameStacker : UntypedActor
    {
        public FrameStacker(string modelName, int framesInStack, int debugStackingTimeMs)
        {
            
        }
        protected override void OnReceive(object message)
        {
            throw new System.NotImplementedException();
        }
    }
}