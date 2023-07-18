using com.rater193.boxnet;
using LiteNetLib;
using LiteNetLib.Utils;
using rater193.scb.common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rater193.scb.client
{
    public class ClientNetworkEntityLocalPlayer : ClientNetworkEntity
    {
        private float movementUpdateTime = 0;//A variable used to let us determine if we want to send an update for the player's movement to the server, used to cap it to about 20 times a second

        public BoxNetClient client { get { return GameClient.gameClient.client; } }

        public override void Start()
        {
            base.Start();
            //Here we are setting the camera's target object to follow to this object
            Camera.main.transform.GetComponent<ClientCameraController>().targetObject = gameObject;
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();

            //Here we are handling data for updating the player on the server
            movementUpdateTime += Time.deltaTime;
            HandleUpdatingPlayer();

            //Here we are getting our horizontal and vertical movement
            float hor = Input.GetAxis("Horizontal");
            float vert = Input.GetAxis("Vertical");

            //Here we are handling moving the object
            transform.position += (transform.forward * vert * Time.deltaTime) + (transform.right * hor * Time.deltaTime);

            //Here we are going to rotate towards the camera
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Camera.main.transform.rotation, 45 * Time.deltaTime);
        }

        //This method handles checking if we ca
        private void HandleUpdatingPlayer()
		{
            //Here we are updating our position on the server
            if (movementUpdateTime >= 1f / 15f)
            {
                //If our update time has elapsed, then we want to go ahead and update
                movementUpdateTime = 0f;

                //Here we are going to update the player
                NetDataWriter writer = new NetDataWriter();
                writer.Put(MessageIDs.server.ClientMovePlayer);
                writer.Put(transform.position.x);
                writer.Put(transform.position.y);
                writer.Put(transform.position.z);
                writer.Put(transform.rotation.eulerAngles.x);
                writer.Put(transform.rotation.eulerAngles.y);
                writer.Put(transform.rotation.eulerAngles.z);
                client.peer.Send(writer, DeliveryMethod.Unreliable);
            }
        }
    }
}
