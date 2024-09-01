# #!/bin/bash
# this will ensure proper line endings on .sh file running on windows
(set -o igncr) 2>/dev/null && set -o igncr; # this comment is needed

# copy all files from src to remote server using scp
ENV=prod
SRC_PATH=D:/src/clients/webforless/destiny-limo
REMOTE_PATH=/home/root/src/clients/webforless/destiny-limo-lms/$ENV/destiny-limo

echo "-----------> Copying Files to remote server on ${ENV} environment..."

# launch the base script
. ../base/_src-copy.sh

echo "-----------> Completed Copying Files to remote server on ${ENV} environment!"