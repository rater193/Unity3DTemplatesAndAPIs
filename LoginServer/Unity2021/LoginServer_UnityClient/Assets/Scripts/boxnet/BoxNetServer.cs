using LiteNetLib;
using LiteNetLib.Utils;
using System;

namespace com.rater193.boxnet
{
    public class BoxNetServer
    {
        //The network listener
        public static EventBasedNetListener listener;
        //The server manager
        public static NetManager server;

        //---------------------------
        //--  Registerable events  --
        //---------------------------
        //This is called when the server receieves data from the client
        public Action<NetPeer, NetDataReader>
            onDataReceived = (client, dataReader) => { };
        //This is called when the server updates
        public Action
            onUpdate = () => { };
        //This is called when the server stops
        public Action
            onStop = () => { };
        //This is called when a client connects
        public Action<NetPeer>
            onClientConnected = (client) => { };
        //This is called when a client disconnects
        public Action<NetPeer>
            onClientDisconnected = (client) => { };

        //Called to start the server
        public void Start(int port)
        {
            //This initializes the network listener
            listener = new EventBasedNetListener();

            //Here we are handling weather or not we accept requests
            listener.ConnectionRequestEvent += request =>
            {
                request.AcceptIfKey("SCBMP");//Accepts if the connection sends the correct key
            };

            //This is called when a player successfully connects
            listener.PeerConnectedEvent += peer =>
            {
                onClientConnected.Invoke(peer);
            };

            //Called when the client is sending data to the server
            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
                NetDataReader reader = new NetDataReader(dataReader.GetRemainingBytes());
                onDataReceived.Invoke(fromPeer, reader);
                //dataReader.Recycle();//We recycle the data to make sure it is not ran in the loop
            };
            //This is called when a player successfully connects
            listener.PeerDisconnectedEvent += (peer, disconnectReason) =>
            {
                onClientDisconnected.Invoke(peer);
            };

            //Now we are starting the actual server
            server = new NetManager(listener);
            server.Start(port /* port */);
        }

        //Called to update the server
        public void Update(float deltaTime)
        {
            server.PollEvents();
            onUpdate.Invoke();
        }

        //Called to stop the server
		public void Stop()
		{
            server.Stop();
            onStop.Invoke();
        }
	}
}
