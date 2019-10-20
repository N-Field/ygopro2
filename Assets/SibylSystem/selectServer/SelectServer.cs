using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

public class SelectServer : WindowServantSP
{
    UIPopupList list;
    UIPopupList serversList;

    UIInput inputIP;
    UIInput inputPort;
    UIInput inputPsw;
    UIInput inputVersion;

    UISprite inputIP_;
    UISprite inputPort_;

    public override void initialize()
    {
        createWindow(Program.I().new_ui_selectServer);
        UIHelper.registEvent(gameObject, "exit_", onClickExit);
        UIHelper.registEvent(gameObject, "face_", onClickFace);
        UIHelper.registEvent(gameObject, "join_", onClickJoin);
        UIHelper.registEvent(gameObject, "clearPsw_", onClearPsw);
        serversList = UIHelper.getByName<UIPopupList>(gameObject, "server");
        //serversList.fontSize = 30;
        if (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified || Application.systemLanguage == SystemLanguage.ChineseTraditional)
        {
            serversList.value = Config.Get("serversPicker", "[自定义]");
        } else {
            serversList.value = Config.Get("serversPicker", "[Custom]");
        }
        UIHelper.registEvent(gameObject, "server", pickServer);
        UIHelper.getByName<UIInput>(gameObject, "name_").value = Config.Get("name", "YGOPro2 User");
        list = UIHelper.getByName<UIPopupList>(gameObject, "history_");
        list.enabled = true;
        UIHelper.registEvent(gameObject, "history_", onSelected);
        name = Config.Get("name", "YGOPro2 User");
        inputIP = UIHelper.getByName<UIInput>(gameObject, "ip_");
        inputPort = UIHelper.getByName<UIInput>(gameObject, "port_");
        inputPsw = UIHelper.getByName<UIInput>(gameObject, "psw_");
        inputIP_ = UIHelper.getByName<UISprite>(gameObject, "ip_");
        inputPort_ = UIHelper.getByName<UISprite>(gameObject, "port_");
        //inputVersion = UIHelper.getByName<UIInput>(gameObject, "version_");
        set_version("0x" + String.Format("{0:X}", Config.ClientVersion));

        //方便免修改 [selectServerWithRoomlist.prefab]
        serversList.items.Add("[OCG]Koishi");
        //serversList.items.Add("[OCG]Koishi备用节点");
        serversList.items.Add("[OCG]Mercury233");
        serversList.items.Add("[OCG]23333 Pre");
        serversList.items.Add("[TCG]Koishi");
        serversList.items.Add("[轮抽服]2Pick");
        serversList.items.Add("[DL]高速决斗");
        serversList.items.Add("[GOAT]YGOPro Goat");
        serversList.items.Add("[DIY]YGOPro 222DIY");
        serversList.items.Add("[AI]Doom Bots of Doom");
        //serversList.items.Add("[OCG&TCG]한국서버");
        //serversList.items.Add("[OCG&TCG]YGOhollow (JP)");
        if (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified || Application.systemLanguage == SystemLanguage.ChineseTraditional)
        {
            serversList.items.Add("[自定义]");
        } else {
            serversList.items.Add("[Custom]");
        }

        SetActiveFalse();
    }

    private void pickServer()
    {
        string server = serversList.value;
        switch (server)
        {
            case "[OCG]Koishi":
                {
                    UIHelper.getByName<UIInput>(gameObject, "ip_").value = "koishi.momobako.com";
                    UIHelper.getByName<UIInput>(gameObject, "port_").value = "7210";
                    Config.Set("serversPicker", "[OCG]Koishi");

                    inputIP_.enabled = true;
                    inputPort_.enabled = false;
                    break;
                }
            /*case "[OCG]Koishi备用节点":
                {
                    UIHelper.getByName<UIInput>(gameObject, "ip_").value = "koishi.momobako.com";
                    UIHelper.getByName<UIInput>(gameObject, "port_").value = "7210";
                    Config.Set("serversPicker", "[OCG]Koishi备用节点");

                    inputIP_.enabled = true;
                    inputPort_.enabled = false;
                    break;
                }*/
            case "[OCG]Mercury233":
                {
                    UIHelper.getByName<UIInput>(gameObject, "ip_").value = "s1.ygo233.com";
                    UIHelper.getByName<UIInput>(gameObject, "port_").value = "233";
                    Config.Set("serversPicker", "[OCG]Mercury233");

                    inputIP_.enabled = true;
                    inputPort_.enabled = false;
                    break;
                }
            case "[OCG]23333 Pre":
                {
                    UIHelper.getByName<UIInput>(gameObject, "ip_").value = "s1.ygo233.com";
                    UIHelper.getByName<UIInput>(gameObject, "port_").value = "23333";
                    Config.Set("serversPicker", "[OCG]23333 Pre");

                    inputIP_.enabled = true;
                    inputPort_.enabled = false;
                    break;
                }
            case "[TCG]Koishi":
                {
                    UIHelper.getByName<UIInput>(gameObject, "ip_").value = "koishi.momobako.com";
                    UIHelper.getByName<UIInput>(gameObject, "port_").value = "1311";
                    Config.Set("serversPicker", "[TCG]Koishi");

                    inputIP_.enabled = false;
                    inputPort_.enabled = false;
                    break;
                }
            case "[轮抽服]2Pick":
                {
                    UIHelper.getByName<UIInput>(gameObject, "ip_").value = "2pick.mycard.moe";
                    UIHelper.getByName<UIInput>(gameObject, "port_").value = "765";
                    Config.Set("serversPicker", "[轮抽服]2Pick");

                    inputIP_.enabled = false;
                    inputPort_.enabled = false;
                    break;
                }
             case "[DIY]YGOPro 222DIY":
                {
                    UIHelper.getByName<UIInput>(gameObject, "ip_").value = "koishi.momobako.com";
                    UIHelper.getByName<UIInput>(gameObject, "port_").value = "222";
                    Config.Set("serversPicker", "[DIY]YGOPro 222DIY");

                    inputIP_.enabled = false;
                    inputPort_.enabled = false;
                    break;
                }
             case "[AI]Doom Bots of Doom":
                {
                    UIHelper.getByName<UIInput>(gameObject, "ip_").value = "koishi.momobako.com";
                    UIHelper.getByName<UIInput>(gameObject, "port_").value = "573";
                    Config.Set("serversPicker", "[AI]Doom Bots of Doom");

                    inputIP_.enabled = false;
                    inputPort_.enabled = false;
                    break;
                }
            case "[DL]高速决斗":
                {
                    UIHelper.getByName<UIInput>(gameObject, "ip_").value = "koishi.momobako.com";
                    UIHelper.getByName<UIInput>(gameObject, "port_").value = "7373";
                    Config.Set("serversPicker", "[DL]高速决斗");

                    inputIP_.enabled = false;
                    inputPort_.enabled = false;
                    break;
                }
            case "[GOAT]YGOPro Goat":
                {
                    UIHelper.getByName<UIInput>(gameObject, "ip_").value = "koishi.momobako.com";
                    UIHelper.getByName<UIInput>(gameObject, "port_").value = "2005";
                    Config.Set("serversPicker", "[GOAT]YGOPro Goat");

                    inputIP_.enabled = false;
                    inputPort_.enabled = false;
                    break;
                }
            case "[DIY]YGOPro 222DIY":
                {
                    UIHelper.getByName<UIInput>(gameObject, "ip_").value = "222diy.momobako.com";
                    UIHelper.getByName<UIInput>(gameObject, "port_").value = "222";
                    Config.Set("serversPicker", "[DIY]YGOPro 222DIY");

                    inputIP_.enabled = false;
                    inputPort_.enabled = false;
                    break;
                }
            /*case "[OCG&TCG]한국서버":
            {
                UIHelper.getByName<UIInput>(gameObject, "ip_").value = "cygopro.fun25.co.kr";
                UIHelper.getByName<UIInput>(gameObject, "port_").value = "17225";
                Config.Set("serversPicker", "[OCG&TCG]한국서버 (KR)");

                inputIP_.enabled = false;
                inputPort_.enabled = false;
                break;
            }
            case "[OCG&TCG]YGOhollow (JP)":
            {
                UIHelper.getByName<UIInput>(gameObject, "ip_").value = "ygosvrjp.tk";
                UIHelper.getByName<UIInput>(gameObject, "port_").value = "7911";
                Config.Set("serversPicker", "[OCG&TCG]YGOhollow (JP)");

                inputIP_.enabled = false;
                inputPort_.enabled = false;
                break;
            } */
            default:
            {
                if (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified || Application.systemLanguage == SystemLanguage.ChineseTraditional)
                {
                    Config.Set("serversPicker", "[自定义]");
                } else {
                    Config.Set("serversPicker", "[Custom]");
                }

                inputIP_.enabled = true;
                inputPort_.enabled = true;
                break;
            }
        }

    }

    void onSelected()
    {
        if (list != null)
        {
            readString(list.value);
        }
    }

    private void readString(string str)
    {
/*
        str = str.Substring(1, str.Length - 1);
        string version = "", remain = "";
        string[] splited;
        splited = str.Split(")");
        try
        {
            version = splited[0];
            remain = splited[1];
        }
        catch (Exception)
        {
        }
        splited = remain.Split(":");
        string ip = "";
        try
        {
            ip = splited[0];
            remain = splited[1];
        }
        catch (Exception)
        {
        }
        splited = remain.Split(" ");
        string psw = "", port = "";
        try
        {
            port = splited[0];
            psw = splited[1];
        }
        catch (Exception)
        {
        }
        if (EditIpAndPort)
        {
            inputIP.value = ip;
            inputPort.value = port;
        }
        inputPsw.value = psw;
*/
        //确保密码为空时，退出后密码依旧保持为空
        str = str.Substring(5, str.Length - 5);
        inputPsw.value = str;
        //inputVersion.value = version;
    }

    void onClearPsw()
    {
        string PswString = File.ReadAllText("config/passwords.conf");
        string[] lines = PswString.Replace("\r", "").Split("\n");
        for (int i = 0; i < lines.Length; i++)
        {
            list.RemoveItem(lines[i]);//清空list
        }
        FileStream stream = new FileStream("config/passwords.conf", FileMode.Truncate, FileAccess.ReadWrite);//清空文件内容
        stream.Close();
        inputPsw.value = "";
        Program.PrintToChat(InterString.Get("房间密码已清空"));
    }

    public override void show()
    {
        base.show();
        Program.I().room.RMSshow_clear();
        printFile(true);
        Program.charge();
    }

    public override void preFrameFunction()
    {
        base.preFrameFunction();
        Menu.checkCommend();
    }

    void printFile(bool first)
    {
        list.Clear();
        if (File.Exists("config/passwords.conf") == false)
        {
            File.Create("config/passwords.conf").Close();
        }
        string txtString = File.ReadAllText("config/passwords.conf");
        string[] lines = txtString.Replace("\r", "").Split("\n");
        for (int i = 0; i < lines.Length; i++)
        {
            if (i == 0)
            {
                if (first)
                {
                    readString(lines[i]);
                }
            }
            list.AddItem(lines[i]);
        }
    }

    void onClickExit()
    {
        Program.I().shiftToServant(Program.I().menu);
        if (TcpHelper.tcpClient != null)
        {
            if (TcpHelper.tcpClient.Connected)
            {
                TcpHelper.tcpClient.Close();
            }
        }
    }

    public void set_version(string str)
    {
        UIHelper.getByName<UIInput>(gameObject, "version_").value = str;
    }

    void onClickJoin()
    {
        if (!isShowed)
        {
            return;
        }
        string Name = UIHelper.getByName<UIInput>(gameObject, "name_").value;
        string ipString = UIHelper.getByName<UIInput>(gameObject, "ip_").value;
        string portString = UIHelper.getByName<UIInput>(gameObject, "port_").value;
        string pswString = UIHelper.getByName<UIInput>(gameObject, "psw_").value;
        string versionString = UIHelper.getByName<UIInput>(gameObject, "version_").value;
        KF_onlineGame(Name, ipString, portString, versionString, pswString);
    }

    public void onClickRoomList()
    {
        if (!isShowed)
        {
            return;
        }
        string Name = UIHelper.getByName<UIInput>(gameObject, "name_").value;
        string ipString = UIHelper.getByName<UIInput>(gameObject, "ip_").value;
        string portString = UIHelper.getByName<UIInput>(gameObject, "port_").value;
        string pswString = "L";
        string versionString = UIHelper.getByName<UIInput>(gameObject, "version_").value;
        KF_onlineGame(Name, ipString, portString, versionString, pswString);
    }

    public void onHide(bool Bool)
    {
        gameObject.SetActive(!Bool);
    }

    public void KF_onlineGame(string Name, string ipString, string portString, string versionString, string pswString = "")
    {
        name = Name;
        Config.Set("name", name);
        if (ipString == "" || portString == "" || versionString == "")
        {
            RMSshow_onlyYes("", InterString.Get("非法输入！请检查输入的主机名。"), null);
        }
        else
        {
            if (name != "")
            {
                //string fantasty = "(" + versionString + ")" + ipString + ":" + portString + " " + pswString;
                string fantasty = "psw: " + pswString;
                list.items.Remove(fantasty);
                list.items.Insert(0, fantasty);
                list.value = fantasty;
                if (list.items.Count > 5)
                {
                    list.items.RemoveAt(list.items.Count - 1);
                }
                string all = "";
                for (int i = 0; i < list.items.Count; i++)
                {
                    all += list.items[i] + "\r\n";
                }
                File.WriteAllText("config/passwords.conf", all);
                printFile(false);
				Program.I().mycard.isMatching = false;
				(new Thread(() => { TcpHelper.join(ipString, name, portString, pswString, versionString); })).Start();
            }
            else
            {
                RMSshow_onlyYes("", InterString.Get("昵称不能为空。"), null);
            }
        }
    }

    GameObject faceShow = null;

    public string name = "";

    void onClickFace()
    {
        name = UIHelper.getByName<UIInput>(gameObject, "name_").value;
        RMSshow_face("showFace", name);
        Config.Set("name", name);
    }

}
