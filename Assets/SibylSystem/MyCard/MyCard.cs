using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

public class MyCard : WindowServantSP
{
    public bool isMatching = false;
    public bool isRequesting = false;
    const string mycard_ip = "tiramisu.mycard.moe";
    const string athletic_port = "8911";
    const string entertain_port = "7911";
    Thread requestThread = null;
    MyCardHelper helper;
    UIInput inputUsername;
    UIInput inputPsw;

    public override void initialize()
    {
        createWindow(Program.I().new_ui_mycard);
        UIHelper.registEvent(gameObject, "exit_", onClickExit);
        UIHelper.registEvent(gameObject, "joinAthletic_", onClickJoinAthletic);
        UIHelper.registEvent(gameObject, "joinEntertain_", onClickJoinEntertain);
        UIHelper.registEvent(gameObject, "database_", onClickDatabase);
        UIHelper.registEvent(gameObject, "community_", onClickCommunity);
        inputUsername = UIHelper.getByName<UIInput>(gameObject, "name_");
        inputPsw = UIHelper.getByName<UIInput>(gameObject, "psw_");
        helper = new MyCardHelper();
        loadUser();
        SetActiveFalse();
    }

    void saveUser() {
        Config.Set("mycard_username", inputUsername.value);
        Config.Set("mycard_password", inputPsw.value);
        Program.I().selectServer.name = inputUsername.value;
    }

    void loadUser() {
        inputUsername.value = Config.Get("mycard_username", "MyCard");
        inputPsw.value = Config.Get("mycard_password", "");
    }

    public void terminateThread()
    { 
        if (!isRequesting && requestThread == null)
        {
            return;
        }
        requestThread.Abort();
        requestThread = null;
    }
    void onClickExit()
    {
        Program.I().shiftToServant(Program.I().menu);
        if (TcpHelper.tcpClient != null)
        {
            if (isRequesting) {
                terminateThread();
            }
            if (TcpHelper.tcpClient.Connected)
            {
                TcpHelper.tcpClient.Close();
            }
        }
    }

    void onClickDatabase() { 
        Application.OpenURL("https://ygobbs.com/");
    }

    void onClickCommunity() { 
        Application.OpenURL("https://mycard.moe/ygopro/arena");
    }

    void matchThread(string username, string password, string match_type) {
        try { 
            Program.PrintToChat(InterString.Get("正在登录至MyCard。"));
            string fail_reason = "";
            bool res = helper.login(username, password, out fail_reason);
            if (!res) {
                Program.PrintToChat(InterString.Get("MyCard登录失败。原因: ") + fail_reason);
                isRequesting = false;
                return;
            }
            Program.PrintToChat(InterString.Get("正在请求匹配。匹配类型: ") + match_type);
            string pswString = helper.requestMatch(match_type, out fail_reason);
            if (pswString == null) { 
                Program.PrintToChat(InterString.Get("匹配请求失败。原因: ") + fail_reason);
                isRequesting = false;
                return;
            }
            Program.PrintToChat(InterString.Get("匹配成功。正在进入房间。"));
            Program.I().mycard.isMatching = true;

            (new Thread(() => { TcpHelper.join(mycard_ip, username, match_type == "athletic" ? athletic_port : entertain_port, pswString, "0x" + String.Format("{0:X}", Config.ClientVersion)); })).Start();
            isRequesting = false;
        } catch (Exception e) {
            if (e.GetType() != typeof(ThreadAbortException)) { 
                Program.PrintToChat(InterString.Get("未知错误: ") + e.Message);
            } else { 
                Program.PrintToChat(InterString.Get("匹配已中断。"));
            }
            isRequesting = false;
        }
    }

    void startMatch(string match_type) {
        string username = inputUsername.value;
        string password = inputPsw.value;
        if (username == "" || password == "")
        { 
            RMSshow_onlyYes("", InterString.Get("用户名或密码为空。"), null);
            return;
        }
        if (isRequesting) 
        {
            terminateThread();
        }
        saveUser();
        isRequesting = true;
        Program.PrintToChat(InterString.Get("已开始匹配。"));
        requestThread = new Thread(() =>
        {
            matchThread(username, password, match_type);
        });
    }

    void onClickJoinAthletic() {
        startMatch("athletic");
    }

    void onClickJoinEntertain() { 
        startMatch("entertain");
    }
}
