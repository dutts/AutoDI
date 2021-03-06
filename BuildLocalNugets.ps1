
[CmdletBinding()]
Param([Parameter(Mandatory=$true)][string]$Version,
[Parameter()][System.IO.DirectoryInfo]$TargetDir)

msbuild "AutoDI.sln" /p:AUTODI_VERSION_FULL=$Version /p:Configuration=Debug
msbuild "AutoDI.AspNetCore\AutoDI.AspNetCore.csproj" /t:Pack /p:Version=$Version

if (!(Test-Path "nuget.exe")) {
    Invoke-WebRequest -Uri https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile "nuget.exe"
}

.\nuget pack Nuget\AutoDI\AutoDI.nuspec -Version $Version
.\nuget pack Nuget\AutoDI.Fody\AutoDI.Fody.nuspec -Version $Version

if ($TargetDir){
    Move-Item "AutoDI.*.nupkg" $TargetDir -Force
    Move-Item "AutoDI.AspNetCore\bin\Debug\*.nupkg" $TargetDir -Force

    Write-Verbose "Moved nugets to $TargetDir"
}