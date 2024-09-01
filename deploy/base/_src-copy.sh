# #!/bin/bash
# this will ensure proper line endings on .sh file running on windows
(set -o igncr) 2>/dev/null && set -o igncr; # this comment is needed

# copy all files from src to remote server using scp
echo "Creating remote root directory for $ENV"
ssh vm_linode "mkdir -p $REMOTE_PATH"

echo "Copying files to remote server - appServer"
ssh vm_linode "mkdir -p $REMOTE_PATH/appServer"
scp -r $SRC_PATH/appServer/* vm_linode:$REMOTE_PATH/appServer/

echo "Copying files to remote server - ui"
ssh vm_linode "mkdir -p $REMOTE_PATH/ui"
scp -r $SRC_PATH/ui/* vm_linode:$REMOTE_PATH/ui/

echo "Copying files to remote server - db"
ssh vm_linode "mkdir -p $REMOTE_PATH/db"
scp -r $SRC_PATH/db/* vm_linode:$REMOTE_PATH/db/

echo "Copying files to remote server - root folder"
scp $SRC_PATH/destiny* vm_linode:$REMOTE_PATH/
scp $SRC_PATH/docker-compose.yml vm_linode:$REMOTE_PATH/