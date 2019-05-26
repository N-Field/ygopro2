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
public class MatchObject {
	public string address;
	public int port;
	public string password;
}

public class MyCardHelper {
	string username = null;
	int userid = -1;
	public bool login(string name, string password, out string fail_reason) {
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
					fail_reason = www.error;
					return false;
				}
			}
			string result = www.text;
			LoginObject result_object = JsonUtility.FromJson<LoginObject>(result);
			username = result_object.user.username;
			userid = result_object.user.id;
		} catch (Exception e) {
			fail_reason = e.Message;
			return false;
		}
		fail_reason = null;
		return true;
	}

	public string requestMatch(string match_type, out string fail_reason) {
		string ret;
		if (username == null || userid < 0) {
			fail_reason = "Not logged in";
			return null;
		}
		try {
			string auth_str = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + userid));
			Dictionary<String, String> header_list = new Dictionary<String, String>();
			header_list.Add("Authorization", auth_str);
			header_list.Add("Content-Type", "application/x-www-form-urlencoded");
			byte[] meta = new byte[1];
			WWW www = new WWW("https://api.mycard.moe/ygopro/match?locale=zh-CN&arena=" + match_type, meta, header_list);
			while (!www.isDone) { 
				if (Application.internetReachability == NetworkReachability.NotReachable || !string.IsNullOrEmpty(www.error))
				{
					fail_reason = www.error;
					return null;
				}
			}
			string result = www.text;
			MatchObject result_object = JsonUtility.FromJson<MatchObject>(result);
			ret = result_object.password;
		} catch (Exception e) {
			fail_reason = e.Message;
			return null;
		}
		fail_reason = null;
		return ret;
	}

	public static void DownloadFace(string name) {
		try { 
			WWW www = new WWW("https://api.moecube.com/accounts/users/"+www.EscapeURL(name, Encoding.UTF8)+".avatar");
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
