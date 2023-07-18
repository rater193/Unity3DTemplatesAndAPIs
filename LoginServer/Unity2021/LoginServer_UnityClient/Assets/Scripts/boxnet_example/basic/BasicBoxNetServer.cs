using com.rater193.boxnet;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class BasicBoxNetServer : MonoBehaviour
{
    private BoxNetServer server;
    void Start()
    {
        server = new BoxNetServer();
        server.onClientConnected += OnClientConnected;
        server.Start(8000);
    }

	private void OnClientConnected(NetPeer peer)
	{

        Debug.Log("We got connection: " + peer.EndPoint);           // Show peer ip
        NetDataWriter writer = new NetDataWriter();                 // Create writer class
        writer.Put("Hello client!");                                // Put some string
        peer.Send(writer, DeliveryMethod.ReliableOrdered);          // Send with reliability
    }

	// Update is called once per frame
	void Update()
    {
        server.Update(Time.deltaTime);
    }

	private void OnDestroy()
	{
        server.Stop();
	}
}
