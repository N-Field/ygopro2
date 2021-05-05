#include <iostream>
#import <UIKit/UIKit.h>

using namespace std;
namespace core {
    template <class type>
    class StringStorageDefault {};
    template <class type,class type2>
    class basic_string {
    public:
        char * str;
        basic_string( char* arg){
            str = arg;
        }
    };
}

void OpenURLInGame(core::basic_string< char,core::StringStorageDefault<char> > const&arg){}

void OpenURL(core::basic_string<char,core::StringStorageDefault<char> >const &arg){
    const void *arg2 = arg.str;
    UIApplication *app = [UIApplication sharedApplication];
    NSString *urlStr = [NSString stringWithUTF8String:(char *)arg2];
    NSURL *url = [NSURL URLWithString:urlStr];
    [app openURL:url];
}


void OpenURL(std::string const &arg){
    UIApplication *app = [UIApplication sharedApplication];
    NSString *urlStr = [NSString stringWithUTF8String:arg.c_str()];
    NSURL *url = [NSURL URLWithString:urlStr];
    [app openURL:url];
    
}
