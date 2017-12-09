#!/bin/bash
docker run -p 27017:27017 -v $(pwd)/data:/data/db -d mongo:3.4.10
 
