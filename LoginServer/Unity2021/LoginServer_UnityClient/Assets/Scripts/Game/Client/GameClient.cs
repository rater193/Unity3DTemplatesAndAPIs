using com.rater193.boxnet;
using LiteNetLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using rater193.scb.common;
using LiteNetLib.Utils;
using System;

namespace rater193.scb.client
{
    public class GameClient : MonoBehaviour
    {
        public static GameClient gameClient;
        public GameObject myNetGameObject;//The net object that belongs to the player
        public int myNetObjectID;//Your net object ID
        public GameObject networkedObjectStorage;//The object used to store the client's networked objects
        public GameObject menuLoadingScreen;//The loading screen for when you load a solar system
        public BoxNetClient client;//Our client
        public List<NetDataReader> queuedMessages = new List<NetDataReader>();//We queue the messages to run on the monoehaviour thread
        public Dictionary<int, GameObject> clientEntityStorage = new Dictionary<int, GameObject>();//Our dictionary used to get our network objects by id

        // Start is called before the first frame update
        void Start()
        {
            //Creating network entity storage and moving it to our scene
            networkedObjectStorage = new GameObject("ClientNetworkedEntityStorage");
            SceneManager.MoveGameObjectToScene(networkedObjectStorage, gameObject.scene);

            client = new BoxNetClient();

            client.onDataReceived = OnDataReceived;
            client.onDisconnected = OnDisconnected;

            client.Start(ClientConfig.ip, ClientConfig.port);
            gameClient = this;
        }

        // Update is called once per frame
        void Update()
        {
            updateMessages();
            client.Update();
        }

		private void FixedUpdate()
		{

        }

		void updateMessages()
		{
            foreach(NetDataReader dataReader in queuedMessages)
			{

                NetDataWriter writer = new NetDataWriter();
                //Debug.Log("We got: " + dataReader.GetString());
                switch (dataReader.GetInt())
                {
                    case MessageIDs.client.RequestLogin:
                        writer.Put(MessageIDs.server.InitVerifyAccount);
                        writer.Put(LoginService.lastLoginClientUsed.sessionKey);
                        client.peer.Send(writer, DeliveryMethod.ReliableOrdered);
                        break;

                    case MessageIDs.client.JoinGame:
                        string myUsername = dataReader.GetString();
                        break;

                    case MessageIDs.client.MapLoadStart:
                        menuLoadingScreen.SetActive(true);
                        //Letting the server know that we are ready to load the map
                        writer.Put(MessageIDs.server.ClientRequestEntities);
                        client.peer.Send(writer, DeliveryMethod.ReliableOrdered);
                        break;

                    case MessageIDs.client.MapLoadReceiveEntities:
                        int entityCount = dataReader.GetInt();//Reading the entity count
                        myNetObjectID = dataReader.GetInt();//Reading the player's entity ID

                        while (entityCount > 0)
						{
                            ReadClientEntity(dataReader);

                            entityCount -= 1;
                        }
                        menuLoadingScreen.SetActive(false);
                        break;

                    case MessageIDs.client.EntityCreate:
                        ReadClientEntity(dataReader);
                        break;

                    case MessageIDs.client.EntityRemove:
                        DestroyEntity(dataReader.GetInt());
                        break;

                    case MessageIDs.client.EntityUpdatePosition:
                        UpdateEntityPosition(dataReader);
                        break;

                    case MessageIDs.client.EntityUpdateRotation:
                        UpdateEntityRotation(dataReader);
                        break;
                }
                dataReader.Clear();
            }
            queuedMessages.Clear();
        }

        private void UpdateEntityPosition(NetDataReader dataReader)
        {
            int entityID = dataReader.GetInt();
            Vector3 pos = new Vector3(dataReader.GetFloat(), dataReader.GetFloat(), dataReader.GetFloat());
            Debug.Log("Client updating: " + entityID);
            GameObject targetEntity;
            clientEntityStorage.TryGetValue(entityID, out targetEntity);
            if (targetEntity)
            {
                if (targetEntity != myNetGameObject)
                {
                    targetEntity.GetComponent<ClientNetworkEntity>().targetPosition = pos;
                }
            }
        }

        private void UpdateEntityRotation(NetDataReader dataReader)
        {
            int entityID = dataReader.GetInt();
            Vector3 rotation = new Vector3(dataReader.GetFloat(), dataReader.GetFloat(), dataReader.GetFloat());
            Debug.Log("Client updating: " + entityID);
            GameObject targetEntity;
            clientEntityStorage.TryGetValue(entityID, out targetEntity);
            if (targetEntity)
            {
                if (targetEntity != myNetGameObject)
                {
                    targetEntity.GetComponent<ClientNetworkEntity>().targetRotation = rotation;
                }
            }
        }

        public void DestroyEntity(int entityID)
		{
            GameObject targetEntity;
            clientEntityStorage.TryGetValue(entityID, out targetEntity);
            if(targetEntity)
			{
                clientEntityStorage.Remove(entityID);
                Destroy(targetEntity);
			}

        }

        public void ReadClientEntity(NetDataReader dataReader)
		{
            //Parsing the entity data
            int entityID = dataReader.GetInt();
            int renderMode = dataReader.GetInt();
            Vector3 pos = new Vector3(
                dataReader.GetFloat(),
                dataReader.GetFloat(),
                dataReader.GetFloat()
                );

            //Here we are checking if we have the ID, or not
            if (clientEntityStorage.ContainsKey(entityID))
            {

            }
            else {
                GameObject clientEntity = new GameObject("NetObj-" + entityID);
                clientEntity.transform.parent = networkedObjectStorage.transform;
                clientEntity.transform.position = pos;

                //Adding the network component to the entity
                if (entityID == myNetObjectID)
                {
                    clientEntity.AddComponent<ClientNetworkEntityLocalPlayer>();
                }
                else
                {
                    clientEntity.AddComponent<ClientNetworkEntity>();
                }

                //Adding our new net entity to be referenced later
                clientEntityStorage.Add(entityID, clientEntity);

                //Here we are creating the entity's sprite
                switch (renderMode)
                {
                    case EnumRenderMode.Empty:

                        break;

                    case EnumRenderMode.Box:
                        GameObject sprite = Instantiate(Resources.Load<GameObject>("Prefabs/ShipPartHull"));
                        sprite.transform.parent = clientEntity.transform;
                        sprite.transform.localPosition = Vector3.zero;
                        sprite.transform.localScale = new Vector3(dataReader.GetFloat(), dataReader.GetFloat(), dataReader.GetFloat());
                        break;

                    case EnumRenderMode.Sprite:

                        break;

                    case EnumRenderMode.Voxel:

                        break;

                    default:
                        Debug.Log("Unhandled render mode: " + renderMode);
                        break;
                }

                if (entityID == myNetObjectID)
                {
                    if (clientEntity)
                    {
                        //If this is our entity's netID then we are going to update our client to reflect this
                        myNetGameObject = clientEntity;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            client.Stop();
        }

        private void OnDataReceived(NetDataReader dataReader)
        {
            NetDataReader newDataReader = new NetDataReader(dataReader.GetRemainingBytes());
            queuedMessages.Add(newDataReader);
        }

        private void OnDisconnected()
        {
            SceneManager.LoadScene(2);
        }
    }
}
