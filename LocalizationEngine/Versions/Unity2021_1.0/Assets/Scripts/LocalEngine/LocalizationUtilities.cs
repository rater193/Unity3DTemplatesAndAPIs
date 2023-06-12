using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using UnityEngine;

public class LocalizationUtilities
{
	//These are used to get the targeted path for loading localization values
	private static string GetLocalizationPath(string localName = "EN")
	{
		return Application.streamingAssetsPath + "/Data/Localization/" + localName + "/";
	}
	private static string GetDefaultLocalizationPath()
	{
		return Application.streamingAssetsPath + "/Data/Localization_Default/";
	}

	//This loads a specific localization folder
	public static LOCALIZATION LoadLocalization(string localName = "EN")
	{
		return LoadOrDefaultLocalizationPath(GetLocalizationPath(localName));
	}

	public static string[] GetLocalizations()
	{
		//This returns a stringarray of all the localizations.
		//Example: "EN", "DE", ETC.....
		string localsPath = GetLocalizationPath("");
		return Directory.GetDirectories(localsPath);
	}

	//This loads the default localization folder
	//This is used for loading the value and creating variables if they do not currently exist
	public static LOCALIZATION LoadOrDefaultLocalizationPath(string path=null)
	{
		//If we do not specify a path, then it will just use the default path
		path = (path==null) ? GetDefaultLocalizationPath() : path;
		string filePath = path + "/local.json";

		Debug.Log("Initializing file: " + filePath);

		//Now we will attempt to load the values
		LOCALIZATION localization = new LOCALIZATION();

		//This will determine if the path exists, if not, create it
		if(!Directory.Exists(path))
		{
			//Creating the directory if one wasnt found
			Directory.CreateDirectory(path);
		}

		//Here you can specify the json file to use
		if (File.Exists(filePath))
		{
			//This will add any new files to the json, then save it back to the file
			Debug.Log("Loading from path: " + filePath);
			localization = JsonUtility.FromJson<LOCALIZATION>(File.ReadAllText(filePath));
			string data = JsonUtility.ToJson(localization, true);
			File.WriteAllText(filePath, data);
		}
		else {
			//Creating the json file, if it doesnt exist
			string data = JsonUtility.ToJson(localization, true);
			File.WriteAllText(filePath, data);
		}

		return localization;
	}

	public static string[] GetLocalizationKeys(string localizationName = "EN")
	{
		LOCALIZATION local = LoadLocalization(localizationName);
		BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
		FieldInfo[] fields = local.GetType().GetFields(flags);
		List<string> keys = new List<string>();

		foreach (FieldInfo field in fields)
		{
			keys.Add(field.Name);
			Debug.Log("Getting localization key: " + field.Name + " - " + local.GetKey(field.Name));
		}

		return keys.ToArray();

		//return local.GetType().get
	}
}
