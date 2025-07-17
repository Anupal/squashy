#!/bin/bash

# cmdline args
if [ $# -ne 2 ]; then
  echo "Usage: $0 [deb|rpm|all] <version>"
  exit 1
fi

TYPE=$1
VERSION=$2
DO_DEB=false
DO_RPM=false

case "$TYPE" in
  deb)
    DO_DEB=true
    ;;
  rpm)
    DO_RPM=true
    ;;
  all)
    DO_DEB=true
    DO_RPM=true
    ;;
  *)
    echo "Usage: $TYPE [deb|rpm|all] <version>"
    exit 1
    ;;
esac

# globals
RELEASE_DIR="build"
BINARY_DIR="$RELEASE_DIR/bin"
DEBIAN_DIR="$RELEASE_DIR/debian"


if $DO_DEB; then
    echo "packaging $RELEASE_DIR/Squashy.$VERSION.deb"
    mkdir -p $DEBIAN_DIR/DEBIAN
    mkdir -p $DEBIAN_DIR/usr/local/bin

    cat <<EOF > $DEBIAN_DIR/DEBIAN/control
Package: squashy
Version: $VERSION
Architecture: all
Maintainer: Anupal Mishra <anupalmishra@gmail.com>
Description: Simple commandline git squasher.
EOF

    cp $BINARY_DIR/* $DEBIAN_DIR/usr/local/bin/
    chmod +x $DEBIAN_DIR/usr/local/bin/Squashy

    dpkg-deb --build $DEBIAN_DIR $RELEASE_DIR/Squashy.$VERSION.deb
fi

if $DO_RPM; then
    echo "rpm not supported yet"
fi
