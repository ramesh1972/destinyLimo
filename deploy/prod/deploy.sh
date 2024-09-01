# #!/bin/bash
# this will ensure proper line endings on .sh file running on windows
(set -o igncr) 2>/dev/null && set -o igncr; # this comment is needed

ENV=prod
SRC_PATH=D:/src/clients/webforless/destiny-limo
REMOTE_PATH=/home/root/src/clients/webforless/destiny-limo-lms/$ENV/destiny-limo

# copy files
./src-copy.sh

echo "-----------> Deploying on remote server on ${ENV} environment..."

# launch the base script
. ../base/_deploy.sh

echo "-----------> Completed Deploying on remote server on ${ENV} environment!"