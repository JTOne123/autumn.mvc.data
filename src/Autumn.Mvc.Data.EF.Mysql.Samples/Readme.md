# step 1- Build docker container

in Dockerfile directory excute this command  

docker build -t autumn/mysql:0.1 .

# step 2 - Launch docker image

docker run -p 3306:3306 -d autumn/mysql:0.1