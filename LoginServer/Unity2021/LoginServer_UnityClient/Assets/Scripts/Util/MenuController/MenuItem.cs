using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rater193.utils.menucontroller
{
    public class MenuItem : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

		internal void Deselect()
        {
            //gameObject.SetActive(false);
            transform.position = new Vector3(Screen.width*2, 0);
        }

		internal void Select()
        {
            transform.position = new Vector3(Screen.width/2, Screen.height/2);
            //gameObject.SetActive(true);
        }
	}
}