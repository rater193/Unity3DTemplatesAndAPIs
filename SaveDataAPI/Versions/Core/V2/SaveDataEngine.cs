using UnityEngine;
using System.IO;
using System.Collections.Generic;
using JetBrains.Annotations;

public static class SaveDataEngine
{

	//This is the savedata class for handling dynamic objects in a reflection style setup
	[System.Serializable]
	private class SavedObjectData
	{
		public string className;//This is the object's class name
		public string objectJson;//This is the data for our object
		public string UnityObjectName;//And here we are storing the unity gameobject name
	}

	//Modifyable data for saving/loading data
	public static string DIRECTORY = "/SaveData/";

	public static void SAVE(string PROFILE_NAME)
	{
		// Create a list of objects to save
		List<SaveObject> objectsToSave = new List<SaveObject>();

		foreach (SaveObject obj in Object.FindObjectsByType<SaveObject>(FindObjectsSortMode.InstanceID))
		{
			Debug.Log("Saving " + obj.name);
			objectsToSave.Add(obj);
		}

		//Constructing the savepath
		string savePath = Path.Join(Application.persistentDataPath, DIRECTORY,  PROFILE_NAME + "/Data");

		//Deleting the path if it exists(To ensure that we save the latest data)
		if(Directory.Exists(savePath))
		{
			Directory.Delete(savePath, true);
		}

		//Constructing the new savedata
		Directory.CreateDirectory(savePath);

		//Debug log so we know whats happening
		Debug.Log("Saving data to " + savePath);

		//The index
		int index = 0;
		// Loop through the objects to save and add their data to the list
		foreach (SaveObject obj in objectsToSave)
		{
			obj.OnSave();

			index++;
			Debug.Log("Saving object data: " +  obj);
			SavedObjectData savedObjectData = new SavedObjectData();
			savedObjectData.className = obj.GetType().FullName;
			savedObjectData.objectJson = JsonUtility.ToJson(obj);
			savedObjectData.UnityObjectName = obj.name;


			string newjson = JsonUtility.ToJson(savedObjectData, true);
			File.WriteAllText(Path.Join(savePath, index + "_" + obj.name + ".json"), newjson);
			Debug.Log("New Json: " + newjson);
		}
	}

	public static void LOAD(string PROFILE_NAME)
	{
		string savePath = Path.Join(Application.persistentDataPath, DIRECTORY, PROFILE_NAME + "/Data");
		if (Directory.Exists(savePath))
		{
			//Here we are getting rid of old objects
			foreach(SaveObject obj in Object.FindObjectsByType<SaveObject>(FindObjectsSortMode.None))
			{
				GameObject.Destroy(obj.gameObject);
			}

			//Here we are iterating through all the stored json files in order to spawn our saved objects in
			string[] files = Directory.GetFiles(savePath);
			foreach(string file in files)
			{
				string fileData = File.ReadAllText(file);
				SavedObjectData savedObjectData = JsonUtility.FromJson<SavedObjectData>(fileData);

				System.Type objectType = System.Type.GetType(savedObjectData.className);
				Debug.Log(objectType.FullName + " : " + savedObjectData.objectJson);
				GameObject newObject = new GameObject();
				newObject.name = savedObjectData.UnityObjectName;

				//First we are looking up our component we are trying to instantiate
				System.Type componentType = System.Type.GetType(savedObjectData.className);

				//Next we are adding our component
				newObject.AddComponent(componentType);

				//Then we are overwriting its values with whats stored in the files
				JsonUtility.FromJsonOverwrite(savedObjectData.objectJson, newObject.GetComponent(componentType));

				if(newObject.GetComponent(componentType) is SaveObject)
				{
					((SaveObject)newObject.GetComponent(componentType)).OnLoad();
				}

			}
		}
	}
}
