@echo off

set UNITY_PATH="C:\Program Files\Unity\Editor\Unity.exe"

mkdir output
mkdir dist
%UNITY_PATH% -batchmode -nographics -silent-crashes -projectPath %cd% -logFile dist/build.log -executeMethod BuildHelper.Build -quit
if %errorlevel% neq 0 exit /b %errorlevel%
cat dist/build.log

bash -c "sed -i '/>UIStatusBarStyle</i\    <key>UIFileSharingEnabled</key>\n    <true />\n    <key>LSSupportsOpeningDocumentsInPlace</key>\n    <true />' output/Info.plist"

cd output
7z a -mx9 ../dist/KoishiPro2-src.7z *
cd ..

aws s3 --endpoint=https://minio.mycard.moe:9000 sync dist s3://mycard/koishipro2
