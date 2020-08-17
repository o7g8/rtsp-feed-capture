FROM debian:buster-slim AS base

ARG DEBIAN_FRONTEND=noninteractive

# Update the source packages
RUN \
    apt-get update && \
    apt-get upgrade -y --no-install-recommends

# Essentials
RUN \
    apt-get install -y --no-install-recommends dnsutils && \
    apt-get install -y --no-install-recommends ca-certificates && \
    apt-get install -y --no-install-recommends openssl && \
    apt-get install -y --no-install-recommends git && \
    apt-get install -y --no-install-recommends build-essential && \
    apt-get install -y --no-install-recommends g++ && \
    apt-get install -y --no-install-recommends gcc && \
    apt-get install -y --no-install-recommends ccache && \
    apt-get install -y --no-install-recommends cmake && \
    apt-get install -y --no-install-recommends pkg-config && \
    apt-get install -y --no-install-recommends ninja-build && \
    apt-get install -y --no-install-recommends libglib2.0-dev && \
    apt-get install -y --no-install-recommends libgstrtspserver-1.0-dev && \
    apt-get install -y --no-install-recommends libcgroup-dev && \
    apt-get install -y --no-install-recommends python3 && \
    apt-get install -y --no-install-recommends python3-pip && \
    apt-get install -y --no-install-recommends python3-setuptools && \
    apt-get install -y --no-install-recommends python3-wheel && \
    pip3 install --user meson

RUN useradd -m user

RUN \
    PATH=~/.local/bin/:$PATH && \
    cd /home/user/ && \
    git clone --single-branch -b 1.10.5 https://github.com/GStreamer/gst-rtsp-server.git && \
    cd gst-rtsp-server/ && \
    meson build && \
    cd build/ && \
    meson compile

COPY data /home/user/source

# Set the entry-point
ENTRYPOINT ["/home/user/gst-rtsp-server/build/examples/test-mp4", \
            "/home/user/source/file_example_MP4_1280_10MG.mp4", \
            "--port=8554"]
