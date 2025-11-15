param(
    [Parameter(Position = 0, Mandatory = $true)]
    [string]$ARCH
)

$ErrorActionPreference = 'Stop'

# Validate architecture directory
$ARCH_DIR = Join-Path $PSScriptRoot $ARCH
if (-not (Test-Path $ARCH_DIR -PathType Container))
{
    Write-Error "Unsupported architecture: $ARCH"
    exit 1
}

# Set up paths
$PROJECT_ROOT = (Get-Item $PSScriptRoot).Parent.Parent.FullName
$LUA_SRC_DIR = Join-Path $PROJECT_ROOT 'external/lua52'
$BUILD_DIR = Join-Path $PROJECT_ROOT "build/.out/win-$ARCH"

# Create build directory
if (-not (Test-Path $BUILD_DIR))
{
    New-Item -ItemType Directory -Path $BUILD_DIR | Out-Null
}

# Run CMake configure and build
cmake -G "Visual Studio 17 2022" -S $LUA_SRC_DIR -B $BUILD_DIR --toolchain (Join-Path $ARCH_DIR 'toolchain.cmake')
cmake --build $BUILD_DIR --config Release

# Parse CMakeCache.txt for LIB_SUFFIX and LIB_LUA_VER
$CMAKE_CACHE_FILE = Join-Path $BUILD_DIR 'CMakeCache.txt'
$LIB_SUFFIX = (Select-String -Path $CMAKE_CACHE_FILE -Pattern '^LIB_SUFFIX:' | ForEach-Object { $_.Line.Split('=')[1] }).Trim()
$LIB_LUA_VER = (Select-String -Path $CMAKE_CACHE_FILE -Pattern '^LIB_LUA_VER:' | ForEach-Object { $_.Line.Split('=')[1] }).Trim()

# Export directory
$EXPORT_DIR = Join-Path $PROJECT_ROOT "KeraLua/runtimes/win-$ARCH/native"
if (-not (Test-Path $EXPORT_DIR))
{
    New-Item -ItemType Directory -Path $EXPORT_DIR | Out-Null
}

# Copy built DLL to export directory
$LIB_NAME = "lua$LIB_LUA_VER" + ".dll"
$LIB_PATH = Join-Path $BUILD_DIR "bin$LIB_SUFFIX/$LIB_NAME"
if (-not (Test-Path $LIB_PATH))
{
    Write-Error "Built library not found: $LIB_PATH"
    exit 1
}
Copy-Item $LIB_PATH $EXPORT_DIR -Force

Write-Host "Build complete. Output: $EXPORT_DIR\$LIB_NAME"
