FROM mongo:3.0.15-wheezy

# Modify child mongo to use /var/lib/mongodb as dbpath (because /data/db wont persist the build)
RUN mkdir -p /var/lib/mongodb \
    && echo "dbpath = /var/lib/mongodb " > /etc/mongodb.conf \
    && chown -R mongodb:mongodb /var/lib/mongodb 

COPY scripts/ /tmp

RUN INSERT_FILES=/tmp/*.tar.gz \
   && for f in $INSERT_FILES; do tar zxvf $f; done

RUN mongod --fork --logpath /var/log/mongodb.log --dbpath /var/lib/mongodb --smallfiles \
    && INSERT_FILES=/tmp/*.js \
    && for f in $INSERT_FILES; do mongo 127.0.0.1:27017/samples $f; done \
    && mongod --dbpath /var/lib/mongodb --shutdown \
    && chown -R mongodb /var/lib/mongodb

# Make the new dir a VOLUME to persists it 
VOLUME /var/lib/mongodb 

CMD ["mongod", "--config", "/etc/mongodb.conf", "--smallfiles"]
