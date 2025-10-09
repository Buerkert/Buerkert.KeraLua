$ErrorActionPreference = 'Stop'

Write-Host "Switching to linux containers..." -ForegroundColor DarkGray
& 'C:\Program Files\Docker\Docker\DockerCli.exe' -SwitchLinuxEngine

$PSScriptRoot/build-in-docker.ps1 linux x64
$PSScriptRoot/build-in-docker.ps1 linux x86
$PSScriptRoot/build-in-docker.ps1 linux arm64
$PSScriptRoot/build-in-docker.ps1 linux arm

Write-Host "Switching to windows containers..." -ForegroundColor DarkGray
& 'C:\Program Files\Docker\Docker\DockerCli.exe' -SwitchWindowsEngine

$PSScriptRoot/build-in-docker.ps1 win x64
$PSScriptRoot/build-in-docker.ps1 win x86
$PSScriptRoot/build-in-docker.ps1 win arm64
$PSScriptRoot/build-in-docker.ps1 win arm