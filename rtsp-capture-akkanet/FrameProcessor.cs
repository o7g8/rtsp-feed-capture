using System;
using Akka.Actor;

namespace rstp_capture_akkanet
{
    internal class FrameProcessor : UntypedActor 
    {
        protected override void OnReceive(object message) {
            switch(message) {
                case ProcessFrame frame:
                    RunInference(frame.Frame, frame.FrameNo);
                    break;
                default:
                    break;
            }
        }

        private void RunInference(object frame, long no)
        {
            Console.WriteLine($"Doing inference of frame #{no}.");
        }
    }
}