using System;
using System.IO;
using OpenCvSharp;

namespace OpenCvTest
{
  class Program
  {
    private static void quit(int i, string msg) {
      Console.WriteLine(msg);
      Environment.Exit(i);
    }

    static void Main(string[] args)
    {
      if (args.Length < 1) {
        quit(-1, "Please specify RTSP source. e.g. 'rtsp://127.0.0.1:8554/test'");
      }

      string rtspUrl = args[0];
      using(var capture = new VideoCapture(rtspUrl)) {
        if(!capture.IsOpened()) {
          quit(-1, "the capture is not opened");
        }

        int i = 1;
        using(var image = new Mat()) {
          while(true) {
            if(!capture.Read(image) || image.Empty()) {
              quit(0, "end of video");
            }

            var size = image.Size();
            Console.WriteLine($"{size.Width} x {size.Height}");
            //image.SaveImage($"frame{i}.jpg");

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
            i++;
          }
        }
      }
    }
  }
}