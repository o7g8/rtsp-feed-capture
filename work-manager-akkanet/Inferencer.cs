using Akka.Actor;

namespace work_manager_akkanet
{
    internal class Inferencer : UntypedActor
    {
        public Inferencer(string modelName, int debugInferenceTimeMs)
        {
            
        }
        protected override void OnReceive(object message)
        {
            throw new System.NotImplementedException();
        }
    }
}