using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveObjectTest : SaveObject
{
	public string uniqueProperty1;

	private bool HasLoaded = false;

	public override void OnSave() { }
	public override void OnLoad() { HasLoaded = true; }

	void Start()
    {
		if (!HasLoaded)
		{
			uniqueProperty1 = "SOMEDATAVALUE" + Random.Range(100, 999);
			gameObject.name = uniqueProperty1;
		}
		else
		{
			transform.SetParent(GameObject.Find("SaveDataObjects").transform);
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
