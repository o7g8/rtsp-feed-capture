# OpenCV in .NET

## Install .NET on Amazon Linux

```
sudo rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm
sudo yum install dotnet-sdk-3.1
```

## Install .NET (Ubuntu 18.04)

<https://docs.microsoft.com/de-de/dotnet/core/install/linux-ubuntu>

```
wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

sudo apt-get update; \
  sudo apt-get install -y apt-transport-https && \
  sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-3.1
```

## Install OpenCV (Ubuntu 18.04)

<https://github.com/shimat/opencvsharp>
<https://www.lostindetails.com/articles/How-to-use-OpenCV-with-CSharp>

```
dotnet new console -n ConsoleApp01
cd ConsoleApp01
dotnet add package OpenCvSharp4
dotnet add package OpenCvSharp4.runtime.ubuntu.18.04-x64
# -- edit Program.cs --- # from https://www.lostindetails.com/articles/How-to-use-OpenCV-with-CSharp
```

Find missing libraries:

```
ldd /home/ubuntu/.nuget/packages/opencvsharp4.runtime.ubuntu.18.04-x64/4.4.0.20200725/runtimes/ubuntu.18.04-x64/native/libOpenCvSharpExtern.so | grep 'not found'
```

Install the missing deps:

```
sudo apt install -y libgtk2.0-0 libtesseract4 libdc1394-22 libavcodec57 libavformat57 libswscale4 libopenexr22
```

Ensure there are no missing dependencies left:

```
ldd /home/ubuntu/.nuget/packages/opencvsharp4.runtime.ubuntu.18.04-x64/4.4.0.20200725/runtimes/ubuntu.18.04-x64/native/libOpenCvSharpExtern.so | grep 'not found'
```

Run teh test program to check the OpenCV .NET wrapper works:

```
dotnet run ./opencv-example.jpg 
```

### OpenCV from sources

https://cv-tricks.com/installation/opencv-4-1-ubuntu18-04/
https://linuxize.com/post/how-to-install-opencv-on-ubuntu-20-04/

## Install Docker (Ubuntu 18.04)

<https://docs.docker.com/engine/install/ubuntu/>

```
sudo apt-get install \
    apt-transport-https \
    ca-certificates \
    curl \
    gnupg-agent \
    software-properties-common

curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -

sudo add-apt-repository \
   "deb [arch=amd64] https://download.docker.com/linux/ubuntu \
   $(lsb_release -cs) \
   stable"

sudo apt-get update
sudo apt-get install docker-ce docker-ce-cli containerd.io
```

Install docker-compose (Ubuntu 18.04) <https://docs.docker.com/compose/install/>

```
sudo curl -L "https://github.com/docker/compose/releases/download/1.26.2/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose

sudo chmod +x /usr/local/bin/docker-compose
```

## OpenCVSharp examples:

https://github.com/shimat/opencvsharp/wiki

https://github.com/VahidN/OpenCVSharp-Samples - OpenCVSharp examples

https://csharp.hotexamples.com/examples/OpenCvSharp/VideoCapture/Read/php-videocapture-read-method-examples.html


## Dockerize .NET app

https://docs.microsoft.com/en-us/dotnet/core/docker/build-container?tabs=windows

https://hub.docker.com/_/microsoft-dotnet-core-runtime/


## Sampling of images

Do grabbing for frames which we need to skip and read the necessary ones <https://answers.opencv.org/question/24714/skipping-frames-in-videocapture/>

Some other stuff:
```
  self.videoCapture.set(cv2.CAP_PROP_FRAME_WIDTH, frameWidth)
  self.videoCapture.set(cv2.CAP_PROP_FRAME_HEIGHT, frameHeight)
  self.videoCapture.set(cv2.CAP_PROP_FPS, fpsCapture)
  self.videoCapture.set(cv2.CAP_PROP_BUFFERSIZE, bufferSize)
```

OpenCV may not be efficient in skipping frames (depends on the codec) <https://stackoverflow.com/questions/22704936/reading-every-nth-frame-from-videocapture-in-opencv>

Ways to improve the situation:
- use FFMPEG/GSTreamer capture "backend" in OpenCV and enforce to use GPU/hw decoder by the backend
- write an "ingester" directly using libffmpeg and libgstreamer to skip the frames.