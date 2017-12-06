# step 1- Build docker container

in Dockerfile directory excute this command  

docker build -t autumn/postgres:0.1 .

# step 2 - Launch docker image

docker run -p 5432:5432 -d autumn/postgres:0.1