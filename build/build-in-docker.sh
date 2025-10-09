#!/usr/bin/env bash

set -eou pipefail

SCRIPT_PATH="$(realpath "$0")"
SCRIPT_DIR="$(dirname "$SCRIPT_PATH")"
PROJECT_ROOT="$(realpath "$SCRIPT_DIR/..")"

PLATFORM="$1"
ARCH="$2"
if [ -z "$PLATFORM" ] || [ -z "$ARCH" ]; then
  echo "Usage: $0 <platform> <architecture>"
  exit 1
fi
PLATFORM_DIR="$SCRIPT_DIR/$PLATFORM"
if [ ! -d "$PLATFORM_DIR" ]; then
  echo "Unsupported platform: $PLATFORM"
  exit 1
fi
ARCH_DIR="$PLATFORM_DIR/$ARCH"
if [ ! -d "$ARCH_DIR" ]; then
  echo "Unsupported architecture: $ARCH"
  exit 1
fi

IMAGE_TAG="keralua-build-$PLATFORM-$ARCH"

echo "Building Docker image '$IMAGE_TAG' for platform '$PLATFORM' and architecture '$ARCH'..."
docker build -t "$IMAGE_TAG" -f "$PLATFORM_DIR/Dockerfile" --build-arg ARCH="$ARCH" "$PLATFORM_DIR"
echo "Running build inside container..."
docker run --rm -v "$PROJECT_ROOT":/src "$IMAGE_TAG"