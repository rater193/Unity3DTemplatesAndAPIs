using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class SaveObject : MonoBehaviour
{
	//This is called before the saveobject has been saved
	public abstract void OnSave();

	//This is called after the saveobject has been loaded
	public abstract void OnLoad();
}
