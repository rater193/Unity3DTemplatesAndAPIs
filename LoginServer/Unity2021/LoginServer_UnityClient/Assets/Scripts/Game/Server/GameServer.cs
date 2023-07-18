using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rater193.scb.common;
using com.rater193.boxnet;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine.SceneManagement;
using System.Threading;
using System;
using Unity.Jobs;
using rater193.scb.global;
using System.IO;

namespace rater193.scb.server
{
    public class UserPacket
	{
        public NetDataReader dataReader;
        public int messageID;
        public UserPacket(int messageID, NetDataReader dataReader)
		{
            this.dataReader = dataReader;
            this.messageID = messageID;
		}
    }

    public class User
	{
        public List<UserPacket> netmessages = new List<UserPacket>();
        public NetPeer socket;
        public float hor = 0f, vert = 0f;
        public PlayerStats stats = new PlayerStats();
        public bool
            connected = false
            ;

        public string
            Username,
            SessionKey
        ;

        public string pathPlayerData
		{
            get { return ServerConfig.PathWorldServerPlayerData + "/" + Username + ".json"; }
		} 

        public ServerEntityPlayer Entity;

        public void OnLogin()
        {
            LoadData();

            //Sending the map load start message to the client
            NetDataWriter writer = new NetDataWriter();
            writer.Put(MessageIDs.client.MapLoadStart);
            socket.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public void OnLogout()
		{
            if(Entity!=null)
			{
                SaveData();
                Entity.Destroy();
			}
		}

        //Loading user data
        public bool LoadData()
		{
            string filePath = pathPlayerData;

            if (File.Exists(filePath))
            {
                JsonUtility.FromJson<PlayerStats>(File.ReadAllText(filePath));
                return true;
            }

            return false;
        }

        //Saving user data
        public void SaveData()
        {
            string data = JsonUtility.ToJson(stats);
            File.WriteAllText(pathPlayerData, data);

        }

        public void Update()
		{
            foreach (UserPacket pkt in netmessages) { HandleNetMessage(pkt); }
            netmessages.Clear();

        }

        public void HandleNetMessage(UserPacket message)
		{
            switch(message.messageID)
			{
                case MessageIDs.server.ClientRequestEntities:
                    //Write order
                    /*
                     * int:MessageID
                     * int:EntityCount
                     * [Variable amount of data, per entity]
                     * Entity order:
                     *  int:EntityID
                     *  string:GameObjectName
                     *  float:x
                     *  float:y
                     * */
                    //Loading the map for the client
                    //This is an expensive call, only ever use this
                    NetDataWriter writer = new NetDataWriter();
                    List<ServerEntity> mapEntities = GameServerMap.GetMapFromScene(Entity.gameObject.scene).getServerEntities();
                    
                    //Here we are sending all the entities in the room to the client
                    writer.Put(MessageIDs.client.MapLoadReceiveEntities);
                    
                    writer.Put(mapEntities.Count);//the amount of map entities
                    writer.Put(Entity.entityID);//The player's target entity id for who they are

                    //Compiling the list itself
                    foreach(ServerEntity entity in mapEntities)
                    {
                        entity.WriterWrite(writer);
                    }

                    socket.Send(writer, DeliveryMethod.ReliableOrdered);
                    break;

                case MessageIDs.server.ClientMovePlayer:
                    //Handling player movement and rotations
                    try
                    {
                        //movement
                        float x = message.dataReader.GetFloat();
                        float y = message.dataReader.GetFloat();
                        float z = message.dataReader.GetFloat();

                        //rotation
                        Vector3 angle = new Vector3(message.dataReader.GetFloat(), message.dataReader.GetFloat(), message.dataReader.GetFloat());
                        if (Entity!=null)
                        {
                            //Here we are updating the local player's position and rotation
                            Entity.gameObject.transform.position = new Vector3(x, y, z);
                            Entity.gameObject.transform.rotation = Quaternion.Euler(angle);
                        }
                    }catch(Exception e)
					{
                        Debug.LogError(e);
					}
                    break;

                default:
                    Debug.Log("Invalid message ID: " + message.messageID);
                    break;
            }
            message.dataReader.Clear();

        }
	}

    public class GameServer : MonoBehaviour
    {
        public bool deb = false;
        public float time;
        GameServerMap map1;
        GameServerMap map2;
        ServerEntity
            ent1, ent2, ent3, ent4;
        private BoxNetServer server;
        private bool running = false;
        public Dictionary<NetPeer, User> users = new Dictionary<NetPeer, User>();
        public List<User> usersToInitiate = new List<User>();
        public List<User> usersToLogout = new List<User>();

        void Start()
        {
            if (running == false)
            {
                //This starts the actual server
                server = new BoxNetServer();
                server.onClientConnected += OnClientConnected;
                server.onClientDisconnected += OnClientDisconnected;
                server.onDataReceived += OnDataReceived;

                //Called for when the server comes to a stop
                server.onStop += OnStop;

                //Here we are disabling simulating physics automatically, so we can deligate physics to only simulate in rooms players are in
                Physics.autoSimulation = false;


                //Now we load the client after we are done
                if (ServerConfig.hostAndPlay == true)
                {
                    SceneManager.LoadScene("client", LoadSceneMode.Additive);
                }

                //Here we are creating some test entities for the new server maps
                //Creating server maps
                map1 = new GameServerMap("TestMap1");
                map2 = new GameServerMap("TestMap2");

                //Creating entities on map1
                ent1 = new ServerEntitySquare(map1, new Vector3(0, 0));
                ent2 = new ServerEntitySquare(map1, new Vector3(0, 2));
                ent3 = new ServerEntitySquare(map1, new Vector3(2, 1));
                new ServerEntitySquare(map1, new Vector3(-2, 1));
                //Generating 100 entities
                for (var i = 0; i < 100; i++)
                {
                    new ServerEntitySquare(map1, new Vector3(2, 2, 0) + new Vector3(UnityEngine.Random.Range(0, 20), UnityEngine.Random.Range(0, 20), UnityEngine.Random.Range(0, 20)));
                }

                //Creating a single entity on map2
                ent4 = new ServerEntitySquare(map2, new Vector3(0, 1));

                server.Start(ServerConfig.port);

#if UNITY_EDITOR
                //Subscribing to an editor event for changing the play mode.
                UnityEditor.EditorApplication.playModeStateChanged += onPlayModeChanged;
#endif
            }
			else
			{
                Destroy(gameObject);
			}
        }

#if UNITY_EDITOR
        private void onPlayModeChanged(UnityEditor.PlayModeStateChange state)
		{
            //If we stop playing the game inside the editor then we are going to clean up the code so that way we dont have memory leaks
            if(state == UnityEditor.PlayModeStateChange.ExitingPlayMode)
			{
                server.onStop.Invoke();
                server.Stop();
			}
		}
#endif

        private void OnClientDisconnected(NetPeer peer)
		{
            //Removing user from the database
            User user;
            if(users.TryGetValue(peer, out user))
			{
                usersToLogout.Add(user);
            }
		}

		private void OnClientConnected(NetPeer peer)
        {
            User user = new User();

            //Adding a user to the database
            users.Add(peer, user);
            user.socket = peer;

            //Sending a message to the client, requesting login session data
            NetDataWriter writer = new NetDataWriter();
            writer.Put(MessageIDs.client.RequestLogin);
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        private void OnDataReceived(NetPeer peer, NetDataReader dataReader)
        {
            //Here we are handling reading network messages from the client
            int msgID = dataReader.GetInt();
            switch (msgID)
            {
                case MessageIDs.server.InitVerifyAccount:
                    string sessionKey = dataReader.GetString();
                    LoginService.lastLoginClientUsed.VerifyUserKey(sessionKey, (bindedUsername) =>
                    {
                        if(bindedUsername.Equals("<UNDEFINED>"))
                        {
                            //Disconnecting unauthorized users
                            peer.Disconnect();
                        }
						else
						{
                            //Packet building
                            NetDataWriter writer = new NetDataWriter();
                            writer.Put(MessageIDs.client.JoinGame);
                            //Here we are writing all the initial information to the client, to let them know what they need to know
                            writer.Put(bindedUsername);
                            peer.Send(writer, DeliveryMethod.ReliableOrdered);

                            User usr;
                            if(users.TryGetValue(peer, out usr))
							{
                                //If the user exists, we are going to start building the user
                                usr.connected = true;
                                usr.SessionKey = sessionKey;
                                usr.Username = bindedUsername;
                                usersToInitiate.Add(usr);//Adding the new user to be initialized
                            }
                        }
                    });
                    break;

                default:
                    //Pass any other non login/authentiaction packets to the client
                    User usr;
                    NetDataReader newDataReader = new NetDataReader(dataReader.GetRemainingBytes());
                    if(users.TryGetValue(peer, out usr))
					{
                        usr.netmessages.Add(new UserPacket(msgID, newDataReader));
					}
                    break;
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            //Here we are teleporting the entities back and forth between server rooms
            time += Time.fixedDeltaTime;
            if(time >= 1)
			{
                if(deb==false)
                {
                    deb = true;
                    map2.AttachServerEntity(ent1);
                    map2.AttachServerEntity(ent2);
                    map2.AttachServerEntity(ent3);
                    //Teleporting entities to map 2

                }
				else
                {
                    deb = false;
                    map1.AttachServerEntity(ent1);
                    map1.AttachServerEntity(ent2);
                    map1.AttachServerEntity(ent3);
                    //Teleporting entities to map 1

                }
                
                time = 0;
			}

            //Here we are updating the server
            server.Update(Time.deltaTime);
            //Here we are going through the list of clients to initiate
            foreach(User user in usersToInitiate)
			{
                //Here we are just debug logging to see what is going on
                Debug.Log("Initializing user: " + user.Username);

                //Creating a new server entity
                ServerEntityPlayer newplayer = new ServerEntityPlayer(map1, new Vector3(3, 2));
                newplayer.user = user;
                user.Entity = newplayer;
                newplayer.isPlayer = true;
                newplayer.socket = user.socket;

                //Here we are starting to setup all of the new player properties
                user.OnLogin();

            }

            //Here we are handling cleaning up players
            foreach(User user in usersToLogout)
			{
                user.OnLogout();
                users.Remove(user.socket);
            }

            //Handling updating all the users
            foreach(User user in users.Values)
			{
                user.Update();
			}
            
            //Here we are checking if any of our server entities are trying to be removed, if so we are going to go ahead and proceed with removing them
            List<ServerEntity> entitiestodestroy = new List<ServerEntity>();

            foreach(ServerEntity ent in ServerEntity.entityStorage.Values)
			{
                ent.Update();
                if(ent.isDestroied)
				{
                    entitiestodestroy.Add(ent);
                }
			}


            //Destroying the entity game objects
            foreach(ServerEntity ent in entitiestodestroy)
			{
                ent.DestroyImediate();
			}

            //Clearing the list of users
            usersToInitiate.Clear();
        }

        private void OnStop()
		{

		}

        private void OnDestroy()
        {
            //Stopping the server when the object is destroied
            server.Stop();
        }
    }
}
