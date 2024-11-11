using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;

namespace com.scb.arena
{
	public class NGOClient
	{
		//This connects you to dedicated servers but it requires port forwarding
		public async static Task JoinIp(string ip, ulong port)
		{
			//This starts the direct connect network manager
			NetworkManager networkManager = NGOUtil.SpawnNetworkManager();
			networkManager.GetComponent<UnityTransport>();


		}
		//This connects you to dedicated servers but it requires port forwarding
		public async static Task JoinSingleplayer()
		{
			//This starts the direct connect network manager
			NetworkManager networkManager = NGOUtil.SpawnNetworkManager();
		}

		//This starts the relay (essentially: host/play)
		public async static Task JoinRelay(string joinCode)
		{
			//This starts the relay
			NGOUtil.SpawnNetworkManagerRelay();

			try
			{
				Debug.Log($"Joining {joinCode}");
				JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
				Debug.Log($"Joined");

				RelayServerData data = new RelayServerData(allocation, "dtls");

				//Setting network data
				NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(data);

				NetworkManager.Singleton.StartClient();
			}
			catch (RelayServiceException e)
			{
				Debug.Log(e);
			}
		}
	}
}