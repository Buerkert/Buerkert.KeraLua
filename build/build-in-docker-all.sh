#!/usr/bin/env bash

set -eou pipefail

SCRIPT_PATH="$(realpath "$0")"
SCRIPT_DIR="$(dirname "$SCRIPT_PATH")"

"$SCRIPT_DIR/build-in-docker.sh" linux x64
"$SCRIPT_DIR/build-in-docker.sh" linux x86
"$SCRIPT_DIR/build-in-docker.sh" linux arm64
"$SCRIPT_DIR/build-in-docker.sh" linux arm

echo "Building Windows targets is only supported on a Windows hosts."