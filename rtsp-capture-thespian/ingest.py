import cv2
import logging
import time
from thespian.actors import *

def feed_system():
    actorsys = ActorSystem()
    #actorsys = ActorSystem("multiprocTCPBase")
    frameProcessor = actorsys.createActor(FrameProcessor)
    feed = actorsys.createActor(VideoFeed)
    actorsys.ask(feed, ("readFeed", "rtsp://127.0.0.1:8554/test", "feed1", frameProcessor))
    #time.sleep(60)
    actorsys.shutdown()

class VideoFeed(Actor):
    def readFeed(self, url, name, frameProcessor):
        cameraStream = cv2.VideoCapture(url)
        fps = int(cameraStream.get(cv2.CAP_PROP_FPS))
        i=0
        while True:
            if i % fps != 0:
                valid = cameraStream.grab()
                if not valid:
                    print("Bad grab")
                    break
                i = i+1
                continue
            valid, frame = cameraStream.read()
            if not valid:
                print("End")
                break
            i = i +1
            print("sending frame")
            self.send(frameProcessor, ("processFrame", name, frame))

    def receiveMessage(self, message, sender):
        # readFeed(url, name)
        if not isinstance(message, tuple):
            logging.error("VideoFeed: wrong message format")
            return
        msgId, url, name, frameProcessor = message
        if msgId != "readFeed":
            logging.error("VideoFeed: wrong message {}".format(msgId))
            return
        self.readFeed(url, name, frameProcessor)

class FrameProcessor(Actor):
    def receiveMessage(self, message, sender):
        # processFrame(senderName, frame)
        print("got msg")
        if not isinstance(message, tuple):
            logging.error("FrameProcessor: wrong message format")
            return
        msgId, senderName, frame = message
        if msgId != "processFrame":
            logging.error("FrameProcessor: wrong message {}".format(msgId))
            return           
        print("FrameProcessor: received a frame from {}".format(senderName))

def video():
    cameraStream = cv2.VideoCapture("rtsp://127.0.0.1:8554/test")
    fps = int(cameraStream.get(cv2.CAP_PROP_FPS))
    i=0
    while True:
        #print("Go {}".format(i))
        if i % fps != 0:
            #print("Grabbing {}".format(i))
            valid = cameraStream.grab()
            if not valid:
                print("Bad grab")
                break
            i = i+1
            continue

        valid, frame = cameraStream.read()
        if not valid:
            print("End")
            break
        print("Frame: {}".format(i))
        i = i +1


if __name__ == "__main__":
    feed_system()
    #say_hello()