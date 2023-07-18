using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rater193.scb.client
{
    public class ClientCameraController : MonoBehaviour
    {

        public GameObject targetObject = null;

        private float zoomLevel = 10;
        private float aizmuth = 45f;
        private float elevation = 0f;

        private bool isDraggingCamera = false;
        private Vector2 dragPos;

        // Start is called before the first frame update
        void Start()
        {

        }

		void Update()
        {

            //Here we are going to pivot around an object 
            if (targetObject)
            {
                Camera cam = Camera.main;
                cam.transform.rotation = Quaternion.Euler(elevation, aizmuth, 0);
                cam.transform.position = targetObject.transform.position + (cam.transform.rotation * new Vector3(0, 0, -zoomLevel));
            }


            //Camera drag controls
            if (isDraggingCamera==false)
			{
                //Waiting to start dragging
                if(Input.GetMouseButton(1)==true)
				{
                    dragPos = Input.mousePosition;
                    isDraggingCamera = true;
                }
			}
			else
            {
                //Waiting to stop dragging, otherwise it will rotate the camera
                if (Input.GetMouseButton(1) == false)
                {
                    isDraggingCamera = false;
                }
				else
                {
                    Vector2 dragDif = (Vector2)Input.mousePosition - dragPos;
                    aizmuth += dragDif.x;
                    elevation -= dragDif.y;
                    dragPos = Input.mousePosition;
                }
            }
		}

		private void FixedUpdate()
		{
        }

		private void OnPreRender()
        {

        }
	}
}
