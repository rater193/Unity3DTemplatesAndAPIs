using LiteNetLib;
using LiteNetLib.Utils;
using rater193.scb.common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rater193.scb.server
{
    public abstract class ServerEntity
    {
        public int renderMode = EnumRenderMode.Box;

        public bool isDestroied = false;
        public bool isPlayer = false;
        public NetPeer socket;

        //Sprite render mode variables
        public string SpriteName = "";

        //Voxel render mode variables
        public string VoxelCreation = "Empty";

        //Box render mode variables
        public Vector3 BoxSize = new Vector3(UnityEngine.Random.Range(50f, 150f) / 100f, UnityEngine.Random.Range(50f, 150f) / 100f, UnityEngine.Random.Range(50f, 150f) / 100f);

        public Vector3 lastPos, lastRotation;

        public static Dictionary<GameObject, ServerEntity> entityStorage = new Dictionary<GameObject, ServerEntity>();

        //Static
        private static int lastEntityID = 0;

        private static int getEntityID()
		{
            return lastEntityID++;
		}

        //This removes the server entity from the clients, used when transfering rooms, or when deleting the object
		internal void removeFromPlayerMaps()
        {
            //Here we are checking if the entity was attached to an old map
            GameServerMap previousMap = GameServerMap.GetMapFromScene(gameObject.scene);
            if (previousMap!=null)
            {
                foreach (ServerEntityPlayer player in previousMap.getPlayers())
                {
                    NetDataWriter writer = new NetDataWriter();

                    writer.Put(MessageIDs.client.EntityRemove);
                    writer.Put(entityID);

                    player.socket.Send(writer, DeliveryMethod.ReliableUnordered);
                }
            }
        }

		//Local
		public GameObject gameObject;
        public int entityID;
        
        public ServerEntity(GameServerMap map, Vector3 position, float rotation=0, ServerEntity parent=null)
		{
            entityID = getEntityID();
            gameObject = new GameObject("ServerEntity-" + entityID);

            if(parent!=null)
			{
                gameObject.transform.SetParent(parent.gameObject.transform);
			}

            gameObject.transform.localPosition = position;
            gameObject.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.forward);
            map.AttachServerEntity(this);
            entityStorage.Add(gameObject, this);
            lastPos = gameObject.transform.localPosition;
            lastRotation = gameObject.transform.rotation.eulerAngles;
        }

        public void Update()
		{
            OnUpdate();
            //Here we are handling position updates
            if ((lastPos- gameObject.transform.localPosition).magnitude >= 0.1f)
			{
                //Here we are only updating if our position has changed
                lastPos = gameObject.transform.localPosition;
                Vector3 pos = gameObject.transform.position;
                foreach(ServerEntityPlayer player in GameServerMap.GetMapFromScene(gameObject.scene).getPlayers())
                {
                    //if(player.socket != socket)
                    //{
                    NetDataWriter writer = new NetDataWriter();
                    writer.Put(MessageIDs.client.EntityUpdatePosition);
                    writer.Put(entityID);
                    writer.Put(pos.x);
                    writer.Put(pos.y);
                    writer.Put(pos.z);
                    player.socket.Send(writer, DeliveryMethod.Unreliable);
                    //}
                }
            }
            //Here we are handling rotation updates, we only send updates, if the object has rotated more than 1 degree
            if(Vector3.Angle(gameObject.transform.rotation.eulerAngles,lastRotation) >= 1f)
            {
                lastRotation = gameObject.transform.rotation.eulerAngles;
                Vector3 angle = gameObject.transform.rotation.eulerAngles;
                foreach (ServerEntityPlayer player in GameServerMap.GetMapFromScene(gameObject.scene).getPlayers())
                {
                    //if(player.socket != socket)
                    //{
                    NetDataWriter writer = new NetDataWriter();
                    writer.Put(MessageIDs.client.EntityUpdateRotation);
                    writer.Put(entityID);
                    writer.Put(angle.x);
                    writer.Put(angle.y);
                    writer.Put(angle.z);
                    player.socket.Send(writer, DeliveryMethod.Unreliable);
                    //}
                }
            }

        }
        public void Destroy()
        {
            isDestroied = true;
		}

        public void DestroyImediate()
		{
            entityStorage.Remove(gameObject);
            removeFromPlayerMaps();
            GameObject.Destroy(gameObject);
            OnDelete();
        }

        public abstract void OnUpdate();
        public abstract void OnDelete();

        public static ServerEntity getEntityFromGameObject(GameObject target)
		{
            ServerEntity ret = null;
			entityStorage.TryGetValue(target, out ret);
            return ret;
        }

        public void WriterWrite(NetDataWriter writer)
        {
            writer.Put(entityID);
            writer.Put(renderMode);
            writer.Put(gameObject.transform.localPosition.x);
            writer.Put(gameObject.transform.localPosition.y);
            writer.Put(gameObject.transform.localPosition.z);

            //Here we are writing render mode variables, based off our render mode, so we dont send to much data to the  client
            switch (renderMode)
			{
                case EnumRenderMode.Box:
                    writer.Put(BoxSize.x);
                    writer.Put(BoxSize.y);
                    writer.Put(BoxSize.z);
                    break;

                case EnumRenderMode.Sprite:

                    break;

                case EnumRenderMode.Voxel:

                    break;
			}
        }
    }
}
