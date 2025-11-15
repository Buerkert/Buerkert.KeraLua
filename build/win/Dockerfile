# escape=`

# Use the latest Windows Server Core 2022 image.
FROM mcr.microsoft.com/windows/servercore:ltsc2022

# Restore the default Windows shell for correct batch processing.
SHELL ["cmd", "/S", "/C"]

RUN `
    curl -SL --output vs_buildtools.exe https://aka.ms/vs/17/release/vs_buildtools.exe `
    && (start /w vs_buildtools.exe --quiet --wait --norestart --nocache `
        --installPath C:\BuildTools `
        --add Microsoft.VisualStudio.Workload.VCTools `
        --add Microsoft.VisualStudio.Component.Windows11SDK.26100 `
        --add Microsoft.VisualStudio.Component.VC.CMake.Project `
        --add Microsoft.VisualStudio.Component.VC.Tools.ARM64 `
        || IF "%ERRORLEVEL%"=="3010" EXIT 0)

# Install last CMake 3.xx version since 4.0 dropped compatibility with versions <3.5
RUN curl -SL --output "cmake.msi" "https://github.com/Kitware/CMake/releases/download/v3.31.8/cmake-3.31.8-windows-x86_64.msi" && `
    msiexec.exe /i "cmake.msi" /quiet /norestart ADD_CMAKE_TO_PATH=System && `
    del /q "cmake.msi"

ARG TARGET_ARCH

ENV TARGET_ARCH="${TARGET_ARCH}"
ENV SRC_DIR="C:\src"

ENTRYPOINT ["C:\\BuildTools\\Common7\\Tools\\VsDevCmd.bat", "&&", "powershell.exe", "-NoLogo", "-ExecutionPolicy", "Bypass", "-File", "%SRC_DIR%\\build\\win\\build.ps1", "%TARGET_ARCH%"]