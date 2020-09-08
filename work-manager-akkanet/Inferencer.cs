using System;
using System.Threading.Tasks;
using Akka.Actor;

namespace work_manager_akkanet
{
    internal class Inferencer : ReceiveActor
    {
        private readonly IActorRef workManager;
        private readonly string modelName;
        private readonly int debugInferenceTimeMs;

        public Inferencer(IActorRef workManager, string modelName, int debugInferenceTimeMs)
        {
            this.workManager = workManager;
            this.modelName = modelName;
            this.debugInferenceTimeMs = debugInferenceTimeMs;
            ReceiveAsync<MsgInferenceJob>(m => DoInferenceJob(m));
        }

        private void AskForWork()
        {
            Console.WriteLine($"Inferencer {modelName} requesting job.");
            workManager.Tell(new MsgRequestInferenceJob(modelName));
        }

        private async Task DoInferenceJob(MsgInferenceJob m)
        {
            AskForWork();
            Console.WriteLine($"Inferencer {m.ModelName} doing inference.");
            await Task.Delay(debugInferenceTimeMs);
            Console.WriteLine($"Inferencer {m.ModelName} done inference.");
        }
    }
}