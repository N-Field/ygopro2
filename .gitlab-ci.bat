@echo off

set UNITY_PATH="C:\Program Files\Unity\Editor\Unity.exe"

mkdir output
%UNITY_PATH% -batchmode -nographics -silent-crashes -projectPath %cd% -logFile build.log -executeMethod BuildHelper.Build -quit
cat build.log

sed -i '/>UIStatusBarStyle</i\    <key>UIFileSharingEnabled</key>\n    <true />\n    <key>LSSupportsOpeningDocumentsInPlace</key>\n    <true />' output/Info.plist

mkdir dist
cd output
7z a -mx9 ../dist/KoishiPro2-src.7z *
cd ..

aws s3 --endpoint=https://minio.mycard.moe:9000 cp dist/KoishiPro2-src.7z s3://mycard/download/
