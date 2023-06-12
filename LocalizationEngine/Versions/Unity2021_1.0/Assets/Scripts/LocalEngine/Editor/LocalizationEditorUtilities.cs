using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LocalizationEditorUtilities : LocalizationUtilities
{
	[MenuItem("rater193/debug/updateDefaultLocalizationFile")]
	static void UpdateDefaultLocalizationFile()
	{
		LoadOrDefaultLocalizationPath();
	}

	[MenuItem("rater193/debug/update localization directories")]
	static void initializeLocalizationDirectories()
	{
		string[] dirs = GetLocalizations();
		foreach (string dir in dirs)
		{
			Debug.Log(dir);
			LoadOrDefaultLocalizationPath(dir + "/");
		}
	}

	[MenuItem("rater193/debug/test")]
	static void testtesttest()
	{
		string[] locals = GetLocalizationKeys();
		Debug.Log(locals);
	}
}
