@echo off

set UNITY_PATH="C:\Program Files\Unity\Editor\Unity.exe"

mkdir output
mkdir KoishiPro2-src
%UNITY_PATH% -batchmode -nographics -silent-crashes -projectPath %cd% -logFile KoishiPro2-src/build.log -executeMethod BuildHelper.Build -quit
if %errorlevel% neq 0 exit /b %errorlevel%
cat KoishiPro2-src/build.log

bash -c "sed -i '/>UIStatusBarStyle</i\    <key>UIFileSharingEnabled</key>\n    <true />\n    <key>LSSupportsOpeningDocumentsInPlace</key>\n    <true />' output/Info.plist"

cd output
7z a -mx9 ../KoishiPro2-src/KoishiPro2-src.7z *
cd ..
