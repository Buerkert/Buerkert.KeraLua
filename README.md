# Buerkert.KeraLua

[![GitHub Actions](https://github.com/Buerkert/Buerkert.KeraLua/workflows/CI/badge.svg)](https://github.com/Buerkert/Buerkert.KeraLua/actions)
[![Release](https://badgen.net/github/release/Buerkert/Buerkert.KeraLua)](https://github.com/Buerkert/Buerkert.KeraLua/releases/latest)
[![dependent repos](https://badgen.net/github/dependents-repo/Buerkert/Buerkert.KeraLua)](https://github.com/orgs/Buerkert/packages?repo_name=Buerkert.KeraLua)
[![dependent pkg](https://badgen.net/github/dependents-pkg/Buerkert/Buerkert.KeraLua)](https://github.com/orgs/Buerkert/packages?repo_name=Buerkert.KeraLua)

C# Native bindings of Lua 5.2 for .NET

## Building

### Prerequisites

For successful the following prerequisites must be met:

- .NET 8 SDK
- Docker
- Windows or Linux
    - Linux won't be able to compile `liblua` for Windows

### Setup

1. Before build fetch the submodules:
    ```sh
    git submodule update --init --recursive
    ```

2. Make sure that the Docker daemon is running.
    ```sh
    docker info
   ```

### Build

1. Compile the lua `liblua` using the helper script
    ```sh
    .\build\build-in-docker-all.ps1 # on Windows
    ./build/build-in-docker-all.sh # on Linux
   ```
2. Build the solution
    ```sh
    dotnet build KeraLua.sln
    ```

Original documentation: [NLua/KeraLua](https://github.com/NLua/KeraLua/blob/master/README.md)
