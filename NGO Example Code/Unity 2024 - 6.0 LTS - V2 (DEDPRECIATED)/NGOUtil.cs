using Unity.Netcode;
using UnityEngine;

namespace com.scb.arena
{
    public class NGOUtil
	{
		public static NetworkManager SpawnNetworkManager()
		{
			if (NetworkManager.Singleton)
			{
				Debug.LogError("The network manager has already been initialized. Disconnect from game first.");
				return NetworkManager.Singleton;
			}
			//Here we are spawning
			NetworkManager manager = GameObject.Instantiate<NetworkManager>(Resources.Load<NetworkManager>("Prefabs/Multiplayer/NetworkManager"));

			return manager;
		}
		public static NetworkManager SpawnNetworkManagerRelay()
		{
			if (NetworkManager.Singleton)
			{
				Debug.LogError("The network manager has already been initialized. Disconnect from game first.");
				return NetworkManager.Singleton;
			}
			//Here we are spawning
			NetworkManager manager = GameObject.Instantiate<NetworkManager>(Resources.Load<NetworkManager>("Prefabs/Multiplayer/NetworkManagerRelay"));

			return manager;
		}
	}
}
