using LiteNetLib;
using LiteNetLib.Utils;
using rater193.scb.common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace rater193.scb.server
{
    public class GameServerMap
    {
        private static Dictionary<Scene, GameServerMap> mapStorage = new Dictionary<Scene, GameServerMap>();

        CreateSceneParameters csp;
        public Scene scene;
        PhysicsScene2D physics;
        GameObject entityStorage;

        public GameServerMap(string worldID)
		{
            csp = new CreateSceneParameters(LocalPhysicsMode.Physics2D);
            scene = SceneManager.CreateScene("ServerMap-"+worldID, csp);
            physics = scene.GetPhysicsScene2D();
            mapStorage.Add(scene, this);
            entityStorage = new GameObject("EntityStorage");

        }

        public void AttachServerEntity(ServerEntity entity)
        {


            entity.removeFromPlayerMaps();

            //Moving it to the new scene
            SceneManager.MoveGameObjectToScene(entity.gameObject, scene);

            //Here we are attaching it to the new map
            foreach (ServerEntityPlayer player in getPlayers())
			{
                if(player.socket!=null)
				{
                    NetDataWriter writer = new NetDataWriter();
                    writer.Put(MessageIDs.client.EntityCreate);//Here we are creating the entity
                    entity.WriterWrite(writer);//Here we are sending the creation message
                    player.socket.Send(writer, DeliveryMethod.ReliableUnordered);//Sending the new entity to the clients
				}
			}

        }

        public void FixedUpdate(float deltaTime)
		{
            physics.Simulate(deltaTime);
        }


        //Gets all the ServerEntities in the room
        public List<ServerEntity> getServerEntities()
        {
            List<ServerEntity> ret = new List<ServerEntity>();

            foreach (GameObject obj in scene.GetRootGameObjects())
            {
                ServerEntity ent = ServerEntity.getEntityFromGameObject(obj);

                if (ent != null)
                {
                    ret.Add(ent);
                }
            }
            return ret;
        }


        //Gets all the player entities in the room
        public List<ServerEntityPlayer> getPlayers()
        {
            //This is our return list of players
            List<ServerEntityPlayer> ret = new List<ServerEntityPlayer>();

            foreach (GameObject obj in scene.GetRootGameObjects())
            {
                //Here wea re just getting the server entity from the game object
                ServerEntity ent = ServerEntity.getEntityFromGameObject(obj);

                //Here we are checking to make sure that we have a valid entity
                if (ent != null)
                {
                    //Here we are checking to make sure that this entity is a player, if so then we want to continue with processing it
                    if (ent.isPlayer)
                    {
                        //Here we are checking to make sure the connection is still active
                        if (((ServerEntityPlayer)ent).socket.ConnectionState != ConnectionState.ShutdownRequested)
                        {
                            ret.Add((ServerEntityPlayer)ent);
                        }
                    }
                }
            }
            return ret;
        }

        //Gets the GameServerMap from the scene
        public static GameServerMap GetMapFromScene(Scene scene)
		{
            GameServerMap ret;
            mapStorage.TryGetValue(scene, out ret);
            return ret;
		}
    }
}
