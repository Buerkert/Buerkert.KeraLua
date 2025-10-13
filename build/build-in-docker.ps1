param(
    [Parameter(Mandatory = $true, Position = 0, HelpMessage = "Target platform (e.g., linux, win)")]
    [string]$Platform,

    [Parameter(Mandatory = $true, Position = 1, HelpMessage = "Target architecture (e.g., x64, x86)")]
    [string]$Architecture
)

$ErrorActionPreference = 'Stop'

$ProjectRoot = (Get-Item $PSScriptRoot).Parent.FullName
Write-Host "Project root: " -ForegroundColor DarkGray

# Validate inputs
if ([string]::IsNullOrWhiteSpace($Platform) -or [string]::IsNullOrWhiteSpace($Architecture))
{
    Write-Host "Usage: .\\build-with-docker.ps1 <platform> <architecture>" -ForegroundColor Yellow
    exit 1
}

$PlatformDir = Join-Path $PSScriptRoot $Platform
if (-not (Test-Path -Path $PlatformDir -PathType Container))
{
    Write-Error "Unsupported platform: $Platform"
    exit 1
}

$ArchDir = Join-Path $PlatformDir $Architecture
if (-not (Test-Path -Path $ArchDir -PathType Container))
{
    Write-Error "Unsupported architecture: $Architecture"
    exit 1
}

$SrcMnt = if ($Platform -eq 'win') { "$ProjectRoot`:C:\src" } else { "$ProjectRoot`:/src" }

$ImageTag = "keralua-build:$Platform-$Architecture"
Write-Host "Building Docker image '$ImageTag' for platform '$Platform' and architecture '$Architecture'..." -ForegroundColor Cyan
& "docker" build -t $ImageTag --build-arg "TARGET_ARCH=$Architecture" -m 8GB $PlatformDir
Write-Host "Running build inside container..." -ForegroundColor Cyan
& "docker" run --storage-opt "size=127GB" --rm -v $SrcMnt $ImageTag
Write-Host "Build completed." -ForegroundColor Green