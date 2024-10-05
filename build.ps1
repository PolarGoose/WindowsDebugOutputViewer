Function Info($msg) {
    Write-Host -ForegroundColor DarkGreen "`nINFO: $msg`n"
}

Function Error($msg) {
    Write-Host `n`n
    Write-Error $msg
    exit 1
}

Function CheckReturnCodeOfPreviousCommand($msg) {
    if(-Not $?) {
        Error "${msg}. Error code: $LastExitCode"
    }
}

Function GetVersion() {
    $gitCommand = Get-Command -Name git

    $tag = & $gitCommand describe --exact-match --tags HEAD
    if(-Not $?) {
        Info "The commit is not tagged. Use 'v0.0-dev' as a tag instead"
        $tag = "v0.0-dev"
    }

    return $tag.Substring(1)
}

Function ForceCopy($file, $dstFolder) {
    Info "Copy `n  '$file' `n to `n  '$dstFolder'"
    New-Item $dstFolder -Force -ItemType "directory" > $null
    Copy-Item $file -Destination $dstFolder -Force
}

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
$env:DOTNET_NOLOGO = "true"

$root = Resolve-Path "$PSScriptRoot"
$buildRoot = "$root/build"
$buildResultsFolder = "$buildRoot/Release/net462"
$publishFolder = "$buildRoot/Publish"
$version = GetVersion

Info "Run 'dotnet build'. version=$version"
dotnet build `
    --configuration Release `
    /property:DebugType=None `
    /property:Version=$version `
    $root/WindowsDebugOutputViewer.sln
CheckReturnCodeOfPreviousCommand "Failed to build the project"

ForceCopy $buildResultsFolder/WindowsDebugOutputViewer.exe $publishFolder

Info "Compress `n $publishFolder/WindowsDebugOutputViewer.exe"
Compress-Archive -Force -Path $publishFolder/WindowsDebugOutputViewer.exe -DestinationPath $publishFolder/WindowsDebugOutputViewer.zip
