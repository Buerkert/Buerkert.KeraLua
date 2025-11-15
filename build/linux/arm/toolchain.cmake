# the name of the target operating system
set(CMAKE_SYSTEM_NAME Linux)

# which compilers to use for C and C++
set(CMAKE_C_COMPILER arm-linux-gnueabihf-gcc)
set(CMAKE_CXX_COMPILER arm-linux-gnueabihf-g++)

set(CMAKE_C_FLAGS_INIT "-DLUA_USE_DLOPEN")
set(CMAKE_CXX_FLAGS_INIT "-DLUA_USE_DLOPEN")