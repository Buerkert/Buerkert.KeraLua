#!/usr/bin/env bash

set -eou pipefail

SCRIPT_PATH="$(realpath "$0")"
SCRIPT_DIR="$(dirname "$SCRIPT_PATH")"
ARCH="$1"
if [ -z "$ARCH" ]; then
  echo "Usage: $0 <architecture>"
  exit 1
fi
ARCH_DIR="$SCRIPT_DIR/$ARCH"
if [ ! -d "$ARCH_DIR" ]; then
  echo "Unsupported architecture: $ARCH"
  exit 1
fi
PROJECT_ROOT="$(realpath "$SCRIPT_DIR/../..")"
LUA_SRC_DIR="$PROJECT_ROOT/external/lua52"
BUILD_DIR="$PROJECT_ROOT/build/.out/linux-$ARCH"

mkdir -p "$BUILD_DIR"

cmake \
  -S "$LUA_SRC_DIR" \
  -B "$BUILD_DIR" \
  -DCMAKE_BUILD_TYPE=Release \
  --toolchain "$ARCH_DIR/toolchain.cmake"
cmake --build "$BUILD_DIR"

CMAKE_CACHE_FILE="$BUILD_DIR/CMakeCache.txt"
LIB_SUFFIX="$(awk -F= '/^LIB_SUFFIX:/ {print $2}' "$CMAKE_CACHE_FILE")"
LIB_LUA_VER="$(awk -F= '/^LIB_LUA_VER:/ {print $2}' "$CMAKE_CACHE_FILE")"

EXPORT_DIR="$PROJECT_ROOT/KeraLua/runtimes/linux-$ARCH/native"
mkdir -p "$EXPORT_DIR"
cp "$BUILD_DIR/lib${LIB_SUFFIX}/liblua${LIB_LUA_VER}.so" "$EXPORT_DIR"
