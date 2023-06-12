using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace com.rater193.api.localization {
	[System.Serializable]
	public class LOCALIZATION : LOCALIZATION_VALUES
	{

		//This is used for getting key valyues
		public string GetKey(string key)
		{
			try
			{
				BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
				string ret = (string)GetType().GetField(key, flags).GetValue(this);
				return ret;
			}
			catch(Exception e)
			{
				Debug.LogError(e);
				return null;
			}
		}

		public string[] GetKeys()
		{
			BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
			FieldInfo[] fields = this.GetType().GetFields(flags);
			List<string> keys = new List<string>();

			foreach (FieldInfo field in fields)
			{
				keys.Add(field.Name);
				Debug.Log("Getting localization key: " + field.Name + " - " + this.GetKey(field.Name));
			}

			return keys.ToArray();
		}

		//This is used for setting key values
		public void SetKey(string key, string value)
		{
			BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
			GetType().GetField(key, flags).SetValue(this, value);

		}
	}
}