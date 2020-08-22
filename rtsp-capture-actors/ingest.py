import cv2
from thespian.actors import *

class Hello(Actor):
    def receiveMessage(self, message, sender):
        self.send(sender, 'Hello, world!')

def say_hello():
    ActorSystem()
    hello = ActorSystem().createActor(Hello)
    print(ActorSystem().ask(hello, 'are you there?', 1.5))


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
    video()
    #say_hello()