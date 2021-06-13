#!/bin/bash

MM_PATH="$MISC_PATH/URLUtility.mm"
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

TMP_PATH=/tmp/tmp-libs-$RANDOM
mkdir $TMP_PATH
_run_for_arch arm64 $TMP_PATH/libiPhone-lib-arm64.a
_run_for_arch armv7 $TMP_PATH/libiPhone-lib-armv7.a
_run_for_arch armv7s $TMP_PATH/libiPhone-lib-armv7s.a
lipo -create $TMP_PATH/libiPhone-lib-arm64.a $TMP_PATH/libiPhone-lib-armv7.a $TMP_PATH/libiPhone-lib-armv7s.a -output ./Libraries/libiPhone-lib-new.a && \
	mv ./Libraries/libiPhone-lib-new.a ./Libraries/libiPhone-lib.a

# patch -p1 < $MISC_PATH/patches/iPhone_Sensors.mm.patch
# echo '#define UNITY_USES_LOCATION 0' >> ./Classes/Preprocessor.h
