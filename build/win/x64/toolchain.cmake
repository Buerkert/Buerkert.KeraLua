# the name of the target operating system
set(CMAKE_SYSTEM_NAME Windows)

set(CMAKE_GENERATOR_PLATFORM "x64" CACHE INTERNAL "")

cmake_policy(SET CMP0011 NEW)
cmake_policy(SET CMP0091 NEW)
set(CMAKE_MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Debug>:Debug>")