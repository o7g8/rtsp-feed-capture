## Choice of the language

- Good openCV support+GPU: Python, C++
- Good support for Greengrass SDK - StreamManager: Python, C++.

## OpenCV VideoIO

There are number of video backends supported by OpenCV <https://docs.opencv.org/4.4.0/d0/da7/videoio_overview.html>
https://docs.opencv.org/4.3.0/d4/d15/group__videoio__flags__base.html

I need to find one which supports GPU and has minimal impact on the CPU:
- https://github.com/opencv/opencv/blob/master/samples/gpu/video_reader.cpp
- https://github.com/opencv/opencv/issues/10201#issuecomment-402453361

Look at FFMPEG and GStreamer.
 - GStreamer (might be better candiate, because allows to specify custom pipeline):
   - https://answers.opencv.org/question/119583/how-does-the-cv2videocapture-class-work-with-gstreamer-pipelines/
   - address 2sec delay https://superuser.com/questions/1461611/why-does-opencvgstreamer-lags-exactly-two-seconds-behind-real-time 
   - https://stackoverflow.com/questions/12952624/hardware-accelerated-h264-decoding-using-ffmpeg-opencv
   - https://answers.opencv.org/question/200787/video-acceleration-gstremer-pipeline-in-videocapture/
   - https://docs.nvidia.com/jetson/l4t/index.html#page/Tegra%2520Linux%2520Driver%2520Package%2520Development%2520Guide%2Faccelerated_gstreamer.html%23wwpID0E0A30HA
   - https://gstconf.ubicast.tv/protected/videos/v12586e9bc53dgvct23vir6pu9al37/attachments/213356.pdf Efficient Video Processing on Embedded GPU
- FFmpeg:
  - uses env variable OPENCV_FFMPEG_CAPTURE_OPTIONS="video_codec;h264_cuvid" https://answers.opencv.org/question/226410/decode-with-multiple-gpus-how-can-i-specify-whichi-gpu-to-use-in-cvvideocapture/
  - https://github.com/opencv/opencv/issues/11480 - ther env variable doesn't allow to use the different codecs for different streams.
  - reads the ffmpeg output directly https://medium.com/@fanzongshaoxing/use-ffmpeg-to-decode-h-264-stream-with-nvidia-gpu-acceleration-16b660fd925d
  - https://www.tal.org/tutorials/ffmpeg_nvidia_encode
  - https://docs.nvidia.com/video-technologies/video-codec-sdk/ffmpeg-with-nvidia-gpu/index.html
  - https://superuser.com/questions/1444978/using-ffmpeg-with-nvidia-gpu
  - https://trac.ffmpeg.org/wiki/HWAccelIntro
  - https://stackoverflow.com/questions/10715170/receiving-rtsp-stream-using-ffmpeg-library

## Build OpenCV with GPU

https://gist.github.com/raulqf/f42c718a658cddc16f9df07ecc627be7
https://www.pyimagesearch.com/2020/02/03/how-to-use-opencvs-dnn-module-with-nvidia-gpus-cuda-and-cudnn/
https://jamesbowley.co.uk/accelerate-opencv-4-4-0-build-with-cuda-and-python-bindings/#cuda_performance

https://github.com/Kjue/python-opencv-gpu-video

The OpenCvSharp may not work with GPU!
OpenCVSharp doesn't have GStreamer compiled in!!! see Cv2.GetBuildInformation()

https://github.com/shimat/opencvsharp/issues/1011
https://github.com/shimat/opencvsharp/issues/581
https://stackoverflow.com/questions/32484659/opencvsharp-installed-using-nuget-package-manager-not-detecting-a-cuda-device

!!!To get the GPU support use gstreamer, stick to Python or switch to C++!!!
https://answers.opencv.org/question/215996/changing-gstreamer-pipeline-to-opencv-in-pythonsolved/

## Actor model in C++
SObjectizer https://github.com/eao197/so-5-5
http://actor-framework.org/
http://www.actor-framework.org/pdf/manual.pdf
https://github.com/actor-framework/actor-framework

## TODO

* Introduce actors

* Measure how many actors can be handled on CPU.

* Build OpenCV and OpenCVSharp for GPU.

* Test the performance of grabbing/reading on GPU.