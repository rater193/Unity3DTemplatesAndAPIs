using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using Unity.Networking.Transport.Relay;
using System.Threading.Tasks;

namespace com.scb.arena
{
	public class NGOHost
	{
		public async static Task Start()
		{
			//This starts the relay
			NGOUtil.SpawnNetworkManagerRelay();

			try
			{
				//Here we are initiating our connection to the relay service
				Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
				string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
				Debug.Log($"Join code: {joinCode}");

				NGOHostData.JoinCode = joinCode;

				//Now we setup our relay data
				//This automatically determines the relay ip and port to connect to
				RelayServerData data = new RelayServerData(allocation, "dtls");

				//Here we are creating the connection
				NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(data);


				//Now that we have finished the main relay connection, we are now going to connect to the relay
				NetworkManager.Singleton.StartHost();

			}
			catch (RelayServiceException e)
			{
				Debug.Log(e);
			}
		}
	}
}