using com.rater193.boxnet;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicBoxNetClient : MonoBehaviour
{
    BoxNetClient client;
    // Start is called before the first frame update
    void Start()
    {
        client = new BoxNetClient();
        client.Start("127.0.0.1", 8000);

        client.onDataReceived = OnDataReceived;
        client.onDisconnected = OnDisconnected;
    }

    // Update is called once per frame
    void Update()
    {
        client.Update();
    }

	private void OnDestroy()
	{
        client.Stop();
    }

    private void OnDataReceived(NetDataReader dataReader)
    {
        Debug.Log("We got: " + dataReader.GetString());
    }

    private void OnDisconnected()
    {
        SceneManager.LoadScene(0);
    }
}
