using System;
using System.Collections.Generic;
using System.Threading;
using Akka.Actor;

namespace work_manager_akkanet
{
    internal class FrameStacker : UntypedActor
    {
        private readonly IActorRef workManager;
        private readonly string modelName;
        private readonly int framesInStack;

        private readonly int debugStackingTimeMs;

        private readonly Queue<object> framesStack;

        public FrameStacker(IActorRef workManager, string modelName, int framesInStack, int debugStackingTimeMs)
        {
            this.workManager = workManager;
            this.modelName = modelName;
            this.framesInStack = framesInStack;
            this.debugStackingTimeMs = debugStackingTimeMs;
            framesStack = new Queue<object>(framesInStack);
        }
        protected override void OnReceive(object message)
        {
            switch(message) {
                case MsgStackFrame frame:
                    StackFrame(frame);
                    break;
                default:
                    break;
            }
        }

        private void StackFrame(MsgStackFrame frame)
        {
            if(frame.ModelName != modelName) {
                throw new InvalidOperationException($"Wrong model: {frame.ModelName}, expected: {modelName}");
            }

            Console.WriteLine($"Stacker {modelName} stacking frame from {frame.Url}");
            Thread.Sleep(debugStackingTimeMs);
            framesStack.Enqueue(frame);
            
            if(framesStack.Count == framesInStack) {
                Console.WriteLine($"Stacker {modelName} stack ready");
                workManager.Tell(new MsgStackReady {ModelName = modelName});
            }
        }
    }
}