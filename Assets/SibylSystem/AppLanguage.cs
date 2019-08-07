using UnityEngine;
using System.IO;

public class AppLanguage
{
    public static string CN = "/zh-CN";        //简体中文(中国) ChineseSimplified
    public static string TW = "/zh-TW";        //繁体中文(台湾) ChineseTraditional
    public static string Chinese = "/Chinese"; //汉语

    public static string US = "/en-US";        //英语(美国) English
    public static string FR = "/fr-FR";        //法语(法国) French
    public static string DE = "/de-DE";        //德语(德国) German
    public static string IT = "/it-IT";        //意大利语(意大利) Italian

    public static string JP = "/ja-JP";        //日语(日本) Japanese
    public static string KR = "/ko-KR";        //韩语(韩国) Korean

    public static string ES = "/es-ES";        //西班牙语(西班牙) Spanish

    //public static string Null = "";          //Unknown
    public static string Null = "/en-US";      //English

    public static string LanguageDir()
    {
        if (Application.systemLanguage == SystemLanguage.ChineseSimplified) {        //可能无法识别
            if (Directory.Exists("cdb/zh-CN") && Directory.Exists("Documents/config/zh-CN")) {
                return CN; //中文简体
            } else {
                return Null;
            }
        } else if (Application.systemLanguage == SystemLanguage.ChineseTraditional) {//可能无法识别
            if (Directory.Exists("cdb/zh-TW") && Directory.Exists("Documents/config/zh-TW")) {
                return TW; //中文繁體
            } else {
                return Null;
            }
        } else if (Application.systemLanguage == SystemLanguage.Chinese) {
            if (Directory.Exists("cdb/Chinese") && Directory.Exists("Documents/config/Chinese")) {
                return Chinese; //汉语
            } else if (Directory.Exists("cdb/zh-CN") && Directory.Exists("Documents/config/zh-CN")) {
                return CN; //中文简体
            } else if (Directory.Exists("cdb/zh-TW") && Directory.Exists("Documents/config/zh-TW")) {
                return TW; //中文繁體
            } else {
                return Null;
            }
        } else if (Application.systemLanguage == SystemLanguage.English) {
            if (Directory.Exists("cdb/en-US") && Directory.Exists("Documents/config/en-US")) {
                return US; //English
            } else {
                return Null;
            }
        } else if (Application.systemLanguage == SystemLanguage.French) {
            if (Directory.Exists("cdb/fr-FR") && Directory.Exists("Documents/config/fr-FR")) {
                return FR; //Français
            } else {
                return Null;
            }
        } else if (Application.systemLanguage == SystemLanguage.German) {
            if (Directory.Exists("cdb/de-DE") && Directory.Exists("Documents/config/de-DE")) {
                return DE; //Deutsch
            } else {
                return Null;
            }
        } else if (Application.systemLanguage == SystemLanguage.Italian) {
            if (Directory.Exists("cdb/it-IT") && Directory.Exists("Documents/config/it-IT")) {
                return IT; //Italiano
            } else {
                return Null;
            }
        } else if (Application.systemLanguage == SystemLanguage.Japanese) {
            if (Directory.Exists("cdb/ja-JP") && Directory.Exists("Documents/config/ja-JP")) {
                return JP; //日本語
            } else {
                return Null;
            }
        } else if (Application.systemLanguage == SystemLanguage.Korean) {
            if (Directory.Exists("cdb/ko-KR") && Directory.Exists("Documents/config/ko-KR")) {
                return KR; //한국어
            } else {
                return Null;
            }
        } else if (Application.systemLanguage == SystemLanguage.Spanish) {
            if (Directory.Exists("cdb/es-ES") && Directory.Exists("Documents/config/es-ES")) {
                return ES; //Español
            } else {
                return Null;
            }
        } else {
            return Null;
        }
    }

}
