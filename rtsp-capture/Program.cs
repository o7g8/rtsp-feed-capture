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

        var fps = (int) capture.Fps;
        //fps = 900;
        Console.WriteLine($"FPS: {capture.Fps}");

        int i = -1;
        using(var image = new Mat()) {
          while(true) {
            capture.Grab();
            i++;
            if(i % fps != 0) {
              continue;
            }

            capture.Read(image);
            if(image.Empty()) {
              quit(0, $"{DateTime.Now}: end of video");
            }
            Console.WriteLine($"{DateTime.Now}: #{i} {image.Width} x {image.Height}");
            //image.SaveImage($"frame{i}.jpg");
          }
        }
      }
    }
  }
}