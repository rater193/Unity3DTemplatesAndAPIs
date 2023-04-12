using UnityEngine;
using System.IO;
using System.Collections.Generic;
using JetBrains.Annotations;

public static class SaveDataEngine
{

	// Class for the saved object data
	[System.Serializable]
	private class SavedObjectData
	{
		public string objectName;
		public string objectJson;
		public string UnityObjectName;
	}

	public static string DIRECTORY = "/SaveData/";
	public static string PROFILE_NAME = "SaveDataTest";

	public static void SAVE()
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
		if (!Directory.Exists(savePath))
		{
			Directory.CreateDirectory(savePath);
		}

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
			savedObjectData.objectName = obj.GetType().FullName;
			savedObjectData.objectJson = JsonUtility.ToJson(obj);
			savedObjectData.UnityObjectName = obj.name;


			string newjson = JsonUtility.ToJson(savedObjectData, true);
			File.WriteAllText(Path.Join(savePath, index + ".json"), newjson);
			Debug.Log("New Json: " + newjson);
		}
	}

	public static void LOAD()
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

				System.Type objectType = System.Type.GetType(savedObjectData.objectName);
				Debug.Log(objectType.FullName + " : " + savedObjectData.objectJson);
				GameObject newObject = new GameObject();
				newObject.name = savedObjectData.UnityObjectName;

				//First we are looking up our component we are trying to instantiate
				System.Type componentType = System.Type.GetType(savedObjectData.objectName);

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
