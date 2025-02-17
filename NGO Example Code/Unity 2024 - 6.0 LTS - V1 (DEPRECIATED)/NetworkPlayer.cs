using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace com.rater193.twodspacefun
{
	[GenerateSerializationForType(typeof(String))]
	public class NetworkPlayer : NetworkBehaviour
	{

		private Transform networkInstantiatedObjectReference;

		public NetworkVariable<float> shipPrefabName = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

		public NetworkVariable<EntityData> entityData = new NetworkVariable<EntityData>(new EntityData
		{
			shipPrefabId = -1,
			shipDisplayName = "",
		}, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


		public struct EntityData : INetworkSerializable
		{
			public int shipPrefabId;
			public FixedString512Bytes shipDisplayName;

			public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
			{
				serializer.SerializeValue(ref shipPrefabId);
				serializer.SerializeValue(ref shipDisplayName);
			}
		}

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		public override void OnNetworkSpawn()
        {
			//UpdateShipPrefab(shipPrefabName.Value);
			shipPrefabName.OnValueChanged = ValUpdateShipPrefab;
			entityData.OnValueChanged = ValUpdateEntityData;

		}
		private void ValUpdateEntityData(EntityData previousValue, EntityData newValue)
		{
			Debug.Log(OwnerClientId + ": EntityData changed from " + previousValue + " to " + newValue);
			//UpdateShipPrefab(newValue);
		}

		private void ValUpdateShipPrefab(float previousValue, float newValue)
		{
			Debug.Log(OwnerClientId + ": ShipPrefabName changed from " + previousValue + " to " + newValue);
			//UpdateShipPrefab(newValue);
		}

		void UpdateShipPrefab(String newPrefab)
		{
			Debug.Log("Changed ship for : " + OwnerClientId + " to " + newPrefab);
		}

		// Update is called once per frame
		void Update()
		{
			if (IsOwner)
			{
				float hspd = 0f;
				float vspd = 0f;

				if (Input.GetKeyDown(KeyCode.T))
				{
					shipPrefabName.Value = Random.Range(0f, 1000f);
				}
				if (Input.GetKeyDown(KeyCode.Y))
				{
					entityData.Value = new EntityData
					{
						shipPrefabId = 1,
						shipDisplayName = "Display Name"
					};
				}
				if (Input.GetKeyDown(KeyCode.U))
				{
					TestServerRpc();
				}
				if (Input.GetKeyDown(KeyCode.I))
				{
					Test2ServerRpc(new ServerRpcParams());
				}
				if (Input.GetKeyDown(KeyCode.O))
				{
					Test1ClientRpc(OwnerClientId);
				}
				if (Input.GetKeyDown(KeyCode.P))
				{
					Test2ClientRpc(new ClientRpcParams
					{
						Send = new ClientRpcSendParams
						{
							TargetClientIds = new List<ulong> { 1 },
						}
					}); ;
				}

				//This can really only instantiate on the server, If the client runs it, it will error. You need to run NetworkObject.Spawn on
				//a server RPC
				if (Input.GetMouseButtonDown(1))
				{
					TestSpawnNetworkObjectServerRpc();
				}
				if (Input.GetMouseButtonDown(2))
				{
					TestDestroyObjectServerRpc();
				}

				if (Input.GetKey(KeyCode.W))
				{
					vspd = 1f;
				}
				if (Input.GetKey(KeyCode.S))
				{
					vspd = -1f;
				}
				if (Input.GetKey(KeyCode.D))
				{
					hspd = 1f;
				}
				if (Input.GetKey(KeyCode.A))
				{
					hspd = -1f;
				}

				transform.Translate(hspd * Time.deltaTime, vspd * Time.deltaTime, 0);
			}
		}

		/////////////////////////////////////////////////////////////
		//                         TESTING                         //
		/////////////////////////////////////////////////////////////


		///////////////////////////////////////////////////
		//               SERVER RPC CALLS                //
		///////////////////////////////////////////////////
		//Server authorative RPC call
		//If you run it on the client, it interupts it and tells it to run on the server.
		[ServerRpc]
		public void TestServerRpc()
		{
			Debug.Log("TestServerRpc ran: " + OwnerClientId);
		}

		//This lets you see what client had sent the message
		[ServerRpc]
		public void Test2ServerRpc(ServerRpcParams rpcParams)
		{
			Debug.Log("rpcParams ran: " + rpcParams.Receive.SenderClientId);
		}

		//This lets you see what client had sent the message
		[ServerRpc]
		public void TestSpawnNetworkObjectServerRpc()
		{
			if (networkInstantiatedObjectReference)
			{
				Destroy(networkInstantiatedObjectReference.gameObject);
			}

			networkInstantiatedObjectReference = Instantiate(Resources.Load<Transform>("Prefabs/TestNetworkSpawnObject"));
			networkInstantiatedObjectReference.GetComponent<NetworkObject>().Spawn(true);
		}

		//This lets you see what client had sent the message
		[ServerRpc]
		public void TestDestroyObjectServerRpc()
		{
			if (networkInstantiatedObjectReference)
			{
				Destroy(networkInstantiatedObjectReference.gameObject);
			}
		}


		///////////////////////////////////////////////////
		//               CLIENT RPC CALLS                //
		///////////////////////////////////////////////////

		//This client RPC can only be called from the server
		[ClientRpc]
		public void Test1ClientRpc(ulong senderId)
		{
			Debug.Log("TestClientRpc1 has ran by " + senderId);
		}

		//This lets you specify what clients receive the message
		/*
		 * Example:
					Test2ClientRpc(new ClientRpcParams
					{
						Send = new ClientRpcSendParams
						{
							TargetClientIds = new List<ulong> { 1 },
						}
					}); ;
		 * */
		[ClientRpc]
		public void Test2ClientRpc(ClientRpcParams rpcParams)
		{
			Debug.Log("TestClientRpc2 has been received");
		}

		[ClientRpc]
		public void Test3ClientRpc()
		{

		}
	}
}
