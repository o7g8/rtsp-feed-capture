using System;
using Akka.Actor;

namespace rstp_capture_akkanet
{
    internal class FrameProcessor : UntypedActor 
    {
        protected override void OnReceive(object message) {
            switch(message) {
                case ProcessFrame frame:
                    RunInference(frame.Frame, frame.FrameNo, frame.Id);
                    break;
                default:
                    break;
            }
        }

        private void RunInference(object frame, long no, int id)
        {
            //frame.SaveImage($"frame{i}.jpg");
            Console.WriteLine($"Doing inference of feed={id} frame={no}.");
        }
    }
}