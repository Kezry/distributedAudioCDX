param(
    [string]$Root = (Split-Path -Parent $PSScriptRoot)
)

$ErrorActionPreference = "Stop"

$required = @(
    ".gitignore",
    "LICENSE",
    "README.md",
    "docs/PROTOCOL.md",
    "android-sound-end/settings.gradle",
    "android-config-app/settings.gradle",
    "windows-sender/DistributedAudioCDX.sln",
    "windows-sender/driver/DacdxVirtualAudio.vcxproj"
)

foreach ($item in $required) {
    $path = Join-Path $Root $item
    if (-not (Test-Path $path)) {
        throw "Missing required file: $item"
    }
}

[xml](Get-Content (Join-Path $Root "windows-sender/src/Dacdx.Windows.App/MainWindow.xaml") -Raw) | Out-Null
[xml](Get-Content (Join-Path $Root "android-sound-end/app/src/main/AndroidManifest.xml") -Raw) | Out-Null
[xml](Get-Content (Join-Path $Root "android-config-app/app/src/main/AndroidManifest.xml") -Raw) | Out-Null
[xml](Get-Content (Join-Path $Root "windows-sender/driver/DacdxVirtualAudio.vcxproj") -Raw) | Out-Null
Get-Content (Join-Path $Root "shared/protocol/device-announcement.schema.json") -Raw | ConvertFrom-Json | Out-Null
Get-Content (Join-Path $Root "shared/models/channels.json") -Raw | ConvertFrom-Json | Out-Null

Write-Host "Repository structure validation passed."

