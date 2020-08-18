## Video capture with OpenCV

Start the RTSP mock server with `run.sh` in `../rtsp-server-mock`.

Run videe frames capture:

```
dotnet run rtsp://127.0.0.1:8554/test >log
```

Measurements are done on 1 FPS.
EC2: t3.small.
OpenCV: CPU based.
All CPU consumption figures below are per core.
Video resolution: 1280x720.

Aproaches:

* Capture an individual frame using `PosFrames`:

```csharp
var fps = capture.Fps;
int frameNo = 0;
//...
//loop over frames
capture.PosFrames = frameNo *  (int)fps;
// same as: capture.Set(VideoCaptureProperties.PosFrames, frameNo * (int)fps);
capture.Read(image);
// ensure the image is not empty..
frameNo++;
```

Outcome: CPU = 14%, takes 2 min to read the 30sec video, the frames come out sometimes with 7 sec delays.

* Capture every frame and ignore all but n*FPS.

Outcome: CPU = 25%, able to read all 900 frames in the 30sec video.
Obvious overhead on decoding of every frame.

* Do `grab` for frames which should be skipped and `Read` for the real ones.

Outcome: CPU = 12-15%, able to keep pace with the 30 sec video. The reduction of `Read` calls doens't significantly reduce CPU load: even if we `Read` a frame every 4sec the CPU goes to 11-12%. `Read` of 1 frame per the entire video 30sec (while grabbing every frame) keeps the CPU at 11%.

So far I conclude that the 11-12% CPU consumption is a sort of "baseline" consumed by `Grab`.

## TODO

* Introduce actors

* Measure how many actors can be handled on CPU.

* Build OpenCV and OpenCVSharp for GPU.

* Test the performance of grabbing/reading on GPU.