#!/bin/bash

MM_PATH="./misc/URLUtility.mm"
IPHONE_SDK_PATH="/Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS.sdk"

_run_for_arch() {
	TARGET_ARCH=$1
	OUTPUT_PATH=$2
	echo "Running for $TARGET_ARCH to $OUTPUT_PATH"
	lipo ./Libraries/libiPhone-lib.a -thin "$TARGET_ARCH" -output "$OUTPUT_PATH"
	ar -d "$OUTPUT_PATH" URLUtility.o
	clang -c "$MM_PATH" -arch "$TARGET_ARCH" -isysroot "$IPHONE_SDK_PATH"
	ar -q "$OUTPUT_PATH" ./URLUtility.o
	rm -f ./URLUtility.o
}

mkdir /tmp/tmp-libs
_run_for_arch arm64 /tmp/tmp-libs/libiPhone-lib-arm64.a
_run_for_arch armv7 /tmp/tmp-libs/libiPhone-lib-armv7.a
_run_for_arch armv7s /tmp/tmp-libs/libiPhone-lib-armv7s.a
lipo -create /tmp/tmp-libs/libiPhone-lib-arm64.a /tmp/tmp-libs/libiPhone-lib-armv7.a /tmp/tmp-libs/libiPhone-lib-armv7s.a -output ./Libraries/libiPhone-lib-new.a && \
	mv ./Libraries/libiPhone-lib-new.a ./Libraries/libiPhone-lib.a

# patch -p1 < ./misc/patches/iPhone_Sensors.mm.patch
# echo '#define UNITY_USES_LOCATION 0' >> ./Classes/Preprocessor.h
