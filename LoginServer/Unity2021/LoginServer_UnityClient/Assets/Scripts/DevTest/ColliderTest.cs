using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ColliderTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Collider2D myCollider = GetComponent<Collider2D>();
        Collider2D[] colliders = GameObject.FindObjectsOfType<Collider2D>();
        List<Collider2D> compiledList = new List<Collider2D>();
        foreach(Collider2D collider in colliders)
		{
            if(collider!= myCollider && collider.gameObject.name.Equals("ObjC"))
			{
                compiledList.Add(collider);
            }
		}

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetDepth(-0.1f,0.1f);

        Debug.Log(gameObject.name + ": " + myCollider.OverlapCollider(filter, new List<Collider2D>()));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
