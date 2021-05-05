@echo off

set UNITY_PATH="C:\Program Files\Unity\Editor\Unity.exe"

mkdir output
mkdir dist
%UNITY_PATH% -batchmode -nographics -silent-crashes -projectPath %cd% -logFile dist/build.log -executeMethod BuildHelper.Build -quit
if %errorlevel% neq 0 exit /b %errorlevel%
cat dist/build.log
mv output dist/src
