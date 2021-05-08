/*

MyCard Helper for MyCard API

Author: Nanahira (78877@qq.com)

IMPORTANT: This module is for intracting with MyCard API for allowing players to have ranked matches in MyCard ranking system. Please contact MyCard before forking or redistributing the project with this module, or including this module in other projects.

Please send emails to pokeboyexn@gmail.com for further information.


*/


using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

[Serializable]
public class LoginUserObject {
	public int id;
	public string username;
	public string name;
	public string email;
	public string password_hash;
	public bool active;
	public bool admin;
	public string avatar;
	public string locale;
	public string registration_ip_address;
	public string ip_address;
	public string created_at;
	public string updated_at;
	public string token;
}

[Serializable]
public class LoginObject {
	public LoginUserObject user;
	public string token;
}

[Serializable]
public class LoginRequest {
	public string account;
	public string password;
}

[Serializable]
public class MatchResultObject {
	public string address;
	public int port;
	public string password;
}

public class MyCardHelper {
	public string username = null;
	int userid = -1;
	public bool login(string name, string password, out string failReason) {
		try { 
			LoginRequest data = new LoginRequest();
			data.account = name;
			data.password = password;
			string data_str = JsonUtility.ToJson(data);
			Dictionary<String, String> header_list = new Dictionary<String, String>();
			header_list.Add("Content-Type", "application/json");
			byte[] data_bytes = Encoding.UTF8.GetBytes(data_str);
			WWW www = new WWW("https://api.moecube.com/accounts/signin", data_bytes, header_list);
			while (!www.isDone) { 
				if (Application.internetReachability == NetworkReachability.NotReachable || !string.IsNullOrEmpty(www.error))
				{
					failReason = www.error;
					return false;
				}
			}
			string result = www.text;
			LoginObject result_object = JsonUtility.FromJson<LoginObject>(result);
			username = result_object.user.username;
			userid = result_object.user.id;
		} catch (Exception e) {
			failReason = e.Message;
			return false;
		}
		failReason = null;
		return true;
	}

	public MatchResultObject requestMatch(string matchType, out string failReason) {
		MatchResultObject matchResultObject;
		if (username == null || userid < 0) {
			failReason = "Not logged in";
			return null;
		}
		try {
			string auth_str = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + userid));
			Dictionary<String, String> header_list = new Dictionary<String, String>();
			header_list.Add("Authorization", auth_str);
			header_list.Add("Content-Type", "application/x-www-form-urlencoded");
			byte[] meta = new byte[1];
			WWW www = new WWW("https://api.moecube.com/ygopro/match?locale=zh-CN&arena=" + matchType, meta, header_list);
			while (!www.isDone) { 
				if (Application.internetReachability == NetworkReachability.NotReachable || !string.IsNullOrEmpty(www.error))
				{
					failReason = www.error;
					return null;
				}
			}
			string result = www.text;
			matchResultObject = JsonUtility.FromJson<MatchResultObject>(result);
			ret = result_object.password;
		} catch (Exception e) {
			failReason = e.Message;
			return null;
		}
		failReason = null;
		return ret;
	}

	public static void DownloadFace(string name) {
		try { 
			WWW www = new WWW("https://api.moecube.com/accounts/users/"+WWW.EscapeURL(name, Encoding.UTF8)+".avatar");
			while (!www.isDone) { 
				if (Application.internetReachability == NetworkReachability.NotReachable || !string.IsNullOrEmpty(www.error))
				{
					return;
				}
			}
			string result = www.text;
			if(result == "{\"message\":\"Not Found\"}")
				return;
			DownloadFaceFromUrl(name, result);
		} catch (Exception e) { 
			return;
		}

	}

	private static void DownloadFaceFromUrl(string nameFace, string url)
	{
		string face = "textures/face/" + nameFace + ".png";
		HttpDldFile df = new HttpDldFile();
		df.Download(url, face);
		if (File.Exists(face))
		{
			Texture2D Face = UIHelper.getTexture2D(face);
			UIHelper.faces.Remove(nameFace);
			UIHelper.faces.Add(nameFace, Face);
		}
	}
}
