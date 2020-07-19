#!/bin/sh

DIR="/app"

if [ "$(ls -A $DIR)" ]; then
    echo "Moving files from $DIR..."
    mkdir /data
    mv -n /app/* /data
    rm /app -r
else
    echo "Target folder is empty or not a directory, skipping..."
fi

cd /data
dotnet OtherWorldBot.dll