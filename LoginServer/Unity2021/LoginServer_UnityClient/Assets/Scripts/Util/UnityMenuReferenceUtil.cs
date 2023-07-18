using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using rater193.scb.common;

public class UnityMenuReferenceUtil : MonoBehaviour
{
    public void Game_Quit()
	{
        // save any game data here
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void HostGame(GameObject hostGameMenu)
    {
        try
        {
            //Here we are getting the input fields
            TMP_InputField serverPort = hostGameMenu.transform.Find("ServerPort").GetComponent<TMP_InputField>();
            TMP_InputField serverPassword = hostGameMenu.transform.Find("Password").GetComponent<TMP_InputField>();

            //Here wea re getting values from those input fields
            ServerConfig.port = (serverPort.text=="") ? 8000 : int.Parse(serverPort.text);
            ServerConfig.password = serverPassword.text;
            ServerConfig.hostAndPlay = true;

            //Here we are configuring the client to connect to what we have configured here
            ClientConfig.ip = "127.0.0.1";
            ClientConfig.port = ServerConfig.port;
            ClientConfig.password = ServerConfig.password;

            //Here we are loading the scene
            SceneManager.LoadScene("server");

        }catch(Exception e)
		{
            Debug.LogError(e);
		}
    }

    public void Register(GameObject registerAccountMenu)
    {
        Transform username = registerAccountMenu.transform.Find("Username");
        Transform password = registerAccountMenu.transform.Find("Password");
        LoginService.lastLoginClientUsed.Register(username.GetComponent<TMP_InputField>().text, password.GetComponent<TMP_InputField>().text);
    }

    public void Login(GameObject loginMenu)
    {
        Transform username = loginMenu.transform.Find("Username");
        Transform password = loginMenu.transform.Find("Password");
        LoginService.lastLoginClientUsed.Login(username.GetComponent<TMP_InputField>().text, password.GetComponent<TMP_InputField>().text);
    }

    public void JoinGame(GameObject joinGameMenu)
    {
        try
        {
            //Here we are getting the input fields
            TMP_InputField serverIP = joinGameMenu.transform.Find("ServerIP").GetComponent<TMP_InputField>();
            TMP_InputField serverPort = joinGameMenu.transform.Find("ServerPort").GetComponent<TMP_InputField>();
            TMP_InputField serverPassword = joinGameMenu.transform.Find("ServerPassword").GetComponent<TMP_InputField>();

            //Here wea re getting values from those input fields
            ClientConfig.ip = (serverIP.text.Equals("")) ?"127.0.0.1" : serverIP.text;
            ClientConfig.port = (serverPort.text.Equals("")) ? 8000 : int.Parse(serverPort.text);
            ClientConfig.password = serverPassword.text;

            //This is to make sure we dont use this
            ServerConfig.hostAndPlay = false;

            //Here we are just displaying the fields to the log to debug
            Debug.Log("Client ip: " + ClientConfig.ip + ", Client port: " + ClientConfig.port + ", Server password: " + ClientConfig.password);

            //Here we are loading the scene
            SceneManager.LoadScene("client");

        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
