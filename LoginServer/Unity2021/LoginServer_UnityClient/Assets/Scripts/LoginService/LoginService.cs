using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;

#if UNITY_EDITOR
/*
 * Here we are just cleaning up some open web sockets, so that way they dont leak into the editor every time we start/stop the game
 * */
[UnityEditor.InitializeOnLoad]
public class HandleLogoutClient
{
    static HandleLogoutClient()
	{
        UnityEditor.EditorApplication.playModeStateChanged += ModeChanged;
    }
    static void ModeChanged(UnityEditor.PlayModeStateChange evt)
    {
        if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode &&
             UnityEditor.EditorApplication.isPlaying)
        {
            if(LoginService.lastLoginClientUsed)
			{
                if (LoginService.lastLoginClientUsed.ws!=null)
                {
                    Debug.Log("Editor Utilities: Cleaning up open websockets");
                    LoginService.lastLoginClientUsed.ws.Close();
                }
			}
        }
    }
}
#endif

[System.Serializable]
public class LoginPacket
{
    public string action = "na";
}

[System.Serializable]
public class LoginPacketGeneric : LoginPacket
{
    public string u;
    public string p;
    public LoginPacketGeneric(string u, string p, string action)
    {
        this.action = action;
        this.u = u;
        this.p = p;

    }
}

[System.Serializable]
public class LoginPacketSuccessfulLogin : LoginPacket
{
    public string sessionKey;
    public LoginPacketSuccessfulLogin(string sessionKey)
    {
        this.sessionKey = sessionKey;
        Debug.Log(sessionKey);

    }
}

public class LoginDataManipulator
{
    private static char getCharCode(string saltKey, int place) {
        return saltKey[place % saltKey.Length];
    }

    private static string salt(string message, string key)
	{
        return key + message;
    }

    public static string decrypt(string data, string password)
    {

        var saltedData = salt(data, password);
        var key = salt(password, password);

        var ret = "";
        for (var place = 0; place < data.Length; place++)
        {
            //dataManagment.randomData.getCharCode(saltKey, place);
            var charOffset = getCharCode(key, place);
            ret += (char)(data[place] - charOffset);
        }
        return ret;
    }

    public static string encrypt(string data, string password)
    {

        var saltedData = salt(data, password);
        var key = salt(password, password);

        var ret = "";
        for (var place = 0; place < data.Length; place++)
        {
            //dataManagment.randomData.getCharCode(saltKey, place);
            var charOffset = getCharCode(key, place);
            ret += (char)(data[place] + charOffset);
        }
        return ret;
    }
}

public class LoginService : MonoBehaviour
{
    public string sessionKey = "N/A";
    public static LoginService lastLoginClientUsed;
    bool debounce = false;
    internal WebSocket ws;
    private string key;
    [HideInInspector]
    public bool connected = false;

    public static Action<string> onLogin = (sessionKey) => { };
    public static Action onAccountRegistered = () => { };
    public static Action<string> onAccountFailedLogin = (reason) => { };
    public static Action<string> onAccountFailedRegistration = (reason) => { };

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        key = "";
        lastLoginClientUsed = this;
        string ipAddress = "192.168.0.100";
        ws = new WebSocket("ws://" + ipAddress + ":27080");

        ws.OnOpen += (sender, e) =>
        {
            //Just because we opened the socket, doesnt mean we are connected, we still have to wait for the acceptedKey
            //packet action, to verify that we have received the encryption key from the server securely
        };

        ws.OnMessage += (sender, e) =>
        {
            if (debounce == false)
            {
                debounce = true;

                foreach (char letter in e.Data)
                {
                    key += (char)(letter / 2);
                }
                ws.Send(key);
            }
            else
            {
                string data = LoginDataManipulator.decrypt(e.Data, key);
                LoginPacket packet = JsonUtility.FromJson<LoginPacket>(data);
                switch (packet.action)
                {
                    case "acceptedKey":
                        //Called once the client has successfully accepted the authentication process for the login server
                        connected = true;
                        break;
                    case "accountRegistered":
                        Debug.Log("Account registered");
                        onAccountRegistered.Invoke();
                        break;
                    case "accountExists":
                        Debug.Log("Account exists");
                        onAccountFailedRegistration.Invoke("Account already exists");
                        break;
                    case "loginFailed":
                        Debug.Log("Login failed");
                        onAccountFailedLogin.Invoke("Invalid Login");
                        break;
                    case "AccountNotRegistered":
                        Debug.Log("Account not registered");
                        onAccountFailedLogin.Invoke("Account not registered");
                        break;
                    case "InvalidLogin":
                        Debug.Log("Invalid login");
                        onAccountFailedLogin.Invoke("Invalid Login");
                        break;
                    case "SuccessfulLogin":
                        LoginPacketSuccessfulLogin _loginpacket = JsonUtility.FromJson<LoginPacketSuccessfulLogin>(data);
                        sessionKey = _loginpacket.sessionKey;
                        onLogin.Invoke(_loginpacket.sessionKey);
                        break;

                }
            }
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("OnClose");
            connected = false;
        };

        ws.Connect();
    }

    public bool VerifyUserKey(string sessionKey, Action<string> executedAction)
	{
        StartCoroutine(GetRequest("http://homesteadgamedevelopment.com:8081/" + sessionKey, executedAction));
        return false;
    }

    IEnumerator GetRequest(string uri, Action<string> executedAction)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;
            string output = "";
            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                output = webRequest.downloadHandler.text;
            }
            executedAction.Invoke(output);
        }
    }

    public void Register(string username, string password)
    {
        if (connected)
        {
            LoginPacketGeneric d = new LoginPacketGeneric(LoginDataManipulator.encrypt(username, key), LoginDataManipulator.encrypt(password, key), "Register");
            string l = LoginDataManipulator.encrypt(JsonUtility.ToJson(d), key);
            ws.Send(l);
		}
		else
		{
            Debug.Log("Please wait, still connecting...");
		}
    }

    public void Login(string username, string password)
    {
        if (connected)
        {
            LoginPacketGeneric d = new LoginPacketGeneric(LoginDataManipulator.encrypt(username, key), LoginDataManipulator.encrypt(password, key), "Login");
            string l = LoginDataManipulator.encrypt(JsonUtility.ToJson(d), key);
            ws.Send(l);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}