using UnityEngine;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections.Generic;

public class Menu : WindowServantSP 
{
    //GameObject screen;
    public override void initialize()
    {
        string hint = File.ReadAllText("Documents/config/hint.conf");
        createWindow(Program.I().new_ui_menu);
        UIHelper.registEvent(gameObject, "setting_", onClickSetting);
        UIHelper.registEvent(gameObject, "deck_", onClickSelectDeck);
        UIHelper.registEvent(gameObject, "online_", onClickOnline);
        UIHelper.registEvent(gameObject, "myCard_", onClickMyCard);
        UIHelper.registEvent(gameObject, "replay_", onClickReplay);
        UIHelper.registEvent(gameObject, "single_", onClickPizzle);
        //UIHelper.registEvent(gameObject, "ai_", onClickAI);
        UIHelper.registEvent(gameObject, "exit_", onClickExit);
        UIHelper.registEvent(gameObject, "joinQQ_", onClickJoinQQ);
        UIHelper.registEvent(gameObject, "download_", onClickDownload);
        //(new Thread(up)).Start();
    }

    public override void show()
    {
        base.show();
        Program.charge();
    }

    public override void hide()
    {
        base.hide();
    }

    static int Version = 0;
    string upurl = "";
    void up()
    {
        try
        {
            string url = "http://ygoforge.com/update.asp";
            WebClient wc = new WebClient();
            Stream s = wc.OpenRead(url);
            StreamReader sr = new StreamReader(s, Encoding.UTF8);
            string result = sr.ReadToEnd();
            sr.Close();
            s.Close();
            string[] lines = result.Replace("\r", "").Split("\n");
            if (lines.Length > 0)
            {
                string[] mats = lines[0].Split(":.:");
                if (mats.Length == 2)
                {
                    if (Version.ToString() != mats[0])
                    {
                        upurl = mats[1];
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
    }

    public override void ES_RMS(string hashCode, List<messageSystemValue> result)
    {
        base.ES_RMS(hashCode, result);
        if (hashCode == "RMSshow_onlyYes")
        {
            Application.OpenURL(upurl);
        }
    }

    bool outed = false;
    public override void preFrameFunction()
    {
        base.preFrameFunction();
        if (upurl != "" && outed == false)
        {
            outed = true;
            RMSshow_onlyYes("RMSshow_onlyYes", InterString.Get("发现更新!@n你可以免费下载"), null);
        }
        Menu.checkCommend();
    }

    void onClickExit()
    {
        Program.I().quit();
        Program.Running = false;
        TcpHelper.SaveRecord();
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE) // IL2CPP 使用此方法才能退出
        Application.Quit();
#else
        Process.GetCurrentProcess().Kill();
#endif
    }

    void onClickOnline()
    {
        Program.I().shiftToServant(Program.I().selectServer);
    }

    void onClickMyCard()
    {
        Program.I().shiftToServant(Program.I().mycard);
    }

    void onClickAI()
    {
        Program.I().shiftToServant(Program.I().aiRoom);
    }

    void onClickPizzle()
    {
        Program.I().shiftToServant(Program.I().puzzleMode);
    }

    void onClickReplay()
    {
        Program.I().shiftToServant(Program.I().selectReplay);
    }

    void onClickSetting()
    {
        Program.I().setting.show();
    }

    void onClickSelectDeck()
    {
        Program.I().shiftToServant(Program.I().selectDeck);
    }

    void onClickJoinQQ()
    {
#if !UNITY_EDITOR && UNITY_ANDROID //Android
        AndroidJavaObject jo = new AndroidJavaObject("cn.unicorn369.library.API");
        jo.Call("doJoinQQGroup", "UHm3h3hSrmgp-iYqMiZcc2zO5J1Q8OyW");
#else
        Application.OpenURL("https://jq.qq.com/?_wv=1027&k=50MZVQA");
#endif
    }

    void onClickDownload()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN //编译器、Windows
        Application.OpenURL("https://github.com/Unicorn369/closeup_mobile/releases/tag/0.1");
#elif UNITY_ANDROID //Android
        AndroidJavaObject jo = new AndroidJavaObject("cn.unicorn369.library.API");
        if (!File.Exists("updates/closeup_0.1.txt")) {//用于检查更新
            if (File.Exists("closeup_0.1.zip")) {//如果有则直接解压
                jo.Call("doExtractZipFile", "closeup_0.1.zip", Program.ANDROID_GAME_PATH);
            } else if (File.Exists("updates/closeup_0.1.txt")){//如果有则下载更新包
                jo.Call("doDownloadZipFile", "https://github.com/Unicorn369/closeup_mobile/releases/download/0.1/closeup_0.1.zip");
            } else {//否则下载并解压，锁定目录：ANDROID_GAME_PATH
                jo.Call("doDownloadZipFile", "https://github.com/Unicorn369/closeup_mobile/releases/download/0.1/closeup_0.1.zip");
            }
        } else {
            jo.Call("showToast", "已是最新，无需再次下载！");
            Program.PrintToChat(InterString.Get("已是最新，无需再次下载！"));
        }
#endif
    }

    public static void deleteShell()
    {
        try
        {
            if (File.Exists("commamd.shell") == true)
            {
                File.Delete("commamd.shell");
            }
        }
        catch (Exception)
        {
        }
    }

    static int lastTime = 0;
    public static void checkCommend()
    {
        if (Program.TimePassed() - lastTime > 1000)
        {
            lastTime = Program.TimePassed();
            if (Program.I().selectDeck == null)
            {
                return;
            }
            if (Program.I().selectReplay == null)
            {
                return;
            }
            if (Program.I().puzzleMode == null)
            {
                return;
            }
            if (Program.I().selectServer == null)
            {
                return;
            }
            if (Program.I().mycard == null)
            {
                return;
            }
            try
            {
                if (File.Exists("commamd.shell") == false)
                {
                    File.Create("commamd.shell").Close();
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
            string all = "";
            try
            {
                all = File.ReadAllText("commamd.shell",Encoding.UTF8);
                string[] mats = all.Split(" ");
                if (mats.Length > 0)
                {
                    switch (mats[0])
                    {
                        case "online":
                            if (mats.Length == 5)
                            {
                                UIHelper.iniFaces();//加载用户头像
                                Program.I().selectServer.KF_onlineGame(mats[1], mats[2], mats[3], mats[4]);
                            }
                            if (mats.Length == 6)
                            {
                                UIHelper.iniFaces();
                                Program.I().selectServer.KF_onlineGame(mats[1], mats[2], mats[3], mats[4], mats[5]);
                            }
                            break;
                        case "edit":
                            if (mats.Length == 2)
                            {
                                Program.I().selectDeck.KF_editDeck(mats[1]);//编辑卡组
                            }
                            break;
                        case "Documents/replay":
                            if (mats.Length == 2)
                            {
                                UIHelper.iniFaces();
                                Program.I().selectReplay.KF_replay(mats[1]);//编辑录像
                            }
                            break;
                        case "puzzle":
                            if (mats.Length == 2)
                            {
                                UIHelper.iniFaces();
                                Program.I().puzzleMode.KF_puzzle(mats[1]);//运行残局
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
            try
            {
                if (all != "")
                {
                    if (File.Exists("commamd.shell") == true)
                    {
                        File.WriteAllText("commamd.shell", "");
                    }
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
        }
    }
}
