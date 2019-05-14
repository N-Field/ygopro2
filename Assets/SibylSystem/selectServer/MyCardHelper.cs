using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

public class JSONObject { 
	public string Stringify()
	{
		return JsonUtility.ToJson(this);
	}
}

public class LoginUserObject : JSONObject {
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
public class LoginObject : JSONObject {
	public LoginUserObject user;
	public string token;
	public string message;
}

public class LoginRequest : JSONObject {
	public string account;
	public string password;
	public LoginRequest(string user, string pass) {
		this.account = user;
		this.password = pass;
	}
}

public class MatchObject : JSONObject {
	public string address;
	public int port;
	public string password;
}

public class MyCardHelper {
	string username = null;
	int userid = -1;
	X509Certificate Cert;

	public MyCardHelper() { 
		byte[] cert_buffer = Encoding.UTF8.GetBytes(mycard_cert);
		Cert = new X509Certificate("cert/mycard.cer");
	}
	public bool login(string name, string password, out string fail_reason) {
		try { 
			LoginRequest data = new LoginRequest(name, password);
			string data_str = data.Stringify();
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.moecube.com/accounts/signin");
			request.Method = "POST";
			request.ContentType = "application/json";
			request.ContentLength = Encoding.UTF8.GetByteCount(data_str);
			request.ClientCertificates.Add(Cert);
			Stream request_stream = request.GetRequestStream();
			StreamWriter stream_writer = new StreamWriter(request_stream, Encoding.UTF8);
			stream_writer.Write(data_str);
			stream_writer.Close();

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
	
			if (response.StatusCode != HttpStatusCode.OK)
			{ 
				fail_reason = response.StatusDescription;
				return false;
			}
			else
			{
				Stream response_stream = response.GetResponseStream();
				StreamReader stream_reader = new StreamReader(response_stream, Encoding.UTF8);
				string result = stream_reader.ReadToEnd();
				stream_reader.Close();
				response_stream.Close();
				LoginObject result_object = JsonUtility.FromJson<LoginObject>(result);
				username = result_object.user.username;
				userid = result_object.user.id;
			}
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
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.mycard.moe/ygopro/match?locale=zh-CN&arena=" + match_type);
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.Headers.Add("Authorization", auth_str);
			request.ClientCertificates.Add(Cert);

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
	
			if (response.StatusCode != HttpStatusCode.OK)
			{ 
				fail_reason = response.StatusDescription;
				return null;
			}
			else
			{
				Stream response_stream = response.GetResponseStream();
				StreamReader stream_reader = new StreamReader(response_stream, Encoding.UTF8);
				string result = stream_reader.ReadToEnd();
				stream_reader.Close();
				response_stream.Close();
				MatchObject result_object = JsonUtility.FromJson<MatchObject>(result);
				ret = result_object.password;
			}
		} catch (Exception e) {
			fail_reason = e.Message;
			return null;
		}
		fail_reason = null;
		return ret;
	}
}
