#/bin/bash

docker-compose --file docker-compose.yaml up --build --remove-orphans >> log-camera.txt
