@REM from https://medium.com/rkttu/write-your-github-actions-workflow-for-build-windows-application-94e5a989f477
@echo off
pushd "%~dp0"
if exist Debug rd /s /q Debug
if exist Release rd /s /q Release
if exist x64 rd /s /q x64
"%programfiles(x86)%\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe" /p:Configuration=Release /p:Platform=x64
"%programfiles(x86)%\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe" /p:Configuration=Release /p:Platform=x86
:exit
popd
@echo on