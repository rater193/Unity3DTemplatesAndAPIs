using LiteNetLib;
using LiteNetLib.Utils;
using System;

namespace com.rater193.boxnet
{
    public class BoxNetClient
    {
        //This is our network listener
        public EventBasedNetListener listener;
        //This is our client manager
        public NetManager client;
        //The server peer
        public NetPeer peer;

        //---------------------------
        //--  Registerable events  --
        //---------------------------
        //This is called when the client receives data from the server
        public Action<NetDataReader>
            onDataReceived = (dataReader) => { };
        //This is called when the client updates
        public Action
            onUpdate = () => { };
        //This is called then the client stops
        public Action
            onStop = () => { };
        //This is called then the client stops
        public Action
            onDisconnected = () => { };

        // Start is called before the first frame update
        public void Start(string ip, int port)
        {
            //Setting up the client
            listener = new EventBasedNetListener();

            //Called when the client receives a message from the server
            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
                NetDataReader reader = new NetDataReader(dataReader.GetRemainingBytes());
                onDataReceived.Invoke(reader);
                //dataReader.Recycle();
                //dataReader.Recycle();
            };

            listener.PeerDisconnectedEvent += (peer, disconnectReason) =>
            {
                onDisconnected.Invoke();
            };
            
            //Setting up the actual client
            client = new NetManager(listener);
            client.Start();

            UnityEngine.Debug.Log("SENDING CONNECTION");
            //Now to connect the client
            peer = client.Connect(ip, port, "SCBMP");

        }

        public void Update()
        {
            //Updating the client
            client.PollEvents();
            onUpdate.Invoke();
        }

		public void Stop()
        {
            //Stopping the client
            client.Stop();
            onStop.Invoke();
            onDisconnected.Invoke();
        }
	}
}
