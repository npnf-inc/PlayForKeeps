#!/bin/bash

# Uninstalls the npnf Platform SDK from your current Unity project
# The npnf Settings asset will not be deleted

DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
FILELIST=$DIR/npnf_filelist.txt

while read -r line
do
	sdkfile=$DIR/$line
	echo $sdkfile
	rm -fRv "$sdkfile"
	rm -fRv "$sdkfile.meta"
done < "$FILELIST"

if [ ! -d "$DIR/NPNF/Resources" ]; then
	rm -fRv "$DIR/NPNF"
fi

echo
echo "The npnf Platform SDK is uninstalled!"

