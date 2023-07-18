using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rater193.scb.client
{
    public class ClientNetworkEntity : MonoBehaviour
    {
        //Here are the values for handling what we are going to rotate towards smoothly
        public Vector3 targetPosition;
        public Vector3 targetRotation;


        // Start is called before the first frame update
        public virtual void Start()
        {
            targetPosition = transform.position;
        }

        // Update is called once per frame
        public virtual void Update()
        {
            //Here we are going to interpolate the movement for the client
            if (GameClient.gameClient.myNetGameObject.gameObject != gameObject)
            {
                //Handling moving
                Vector3 differenceInPosition = targetPosition - transform.position;
                if (differenceInPosition.magnitude >= 0.1f)
                {
                    transform.position += differenceInPosition * Time.deltaTime * 30f;//Here we are going to move relitive to the framerate
                }

                //Handling rotating
                float differenceInAngle = Vector3.Angle(transform.rotation.eulerAngles, targetRotation);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), differenceInAngle / 2f);
            }
        }
    }
}
