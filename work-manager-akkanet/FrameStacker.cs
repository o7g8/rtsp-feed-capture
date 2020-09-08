using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;

namespace work_manager_akkanet
{
    internal class FrameStacker : ReceiveActor
    {
        private readonly IActorRef workManager;
        private readonly string modelName;
        private readonly int maxFramesInStack;
        private readonly int debugStackingTimeMs;
        private readonly Queue<object> framesStack;

        public FrameStacker(IActorRef workManager, string modelName, int framesInStack, int debugStackingTimeMs)
        {
            this.workManager = workManager;
            this.modelName = modelName;
            this.maxFramesInStack = framesInStack;
            this.debugStackingTimeMs = debugStackingTimeMs;
            framesStack = new Queue<object>(framesInStack);
            ReceiveAsync<MsgStackFrame>(frame => StackFrame(frame));
        }

        private async Task StackFrame(MsgStackFrame frame)
        {
            if(frame.ModelName != modelName) {
                throw new InvalidOperationException($"Wrong model: {frame.ModelName}, expected: {modelName}");
            }

            Console.WriteLine($"Stacker {modelName} stacking frame from {frame.Url}");
            await Task.Delay(debugStackingTimeMs);
            framesStack.Enqueue(frame);
            
            if(framesStack.Count == maxFramesInStack) {
                Console.WriteLine($"Stacker {modelName} stack ready");
                framesStack.Clear();
                workManager.Tell(new MsgStackReady {ModelName = modelName});
            }
        }
    }
}