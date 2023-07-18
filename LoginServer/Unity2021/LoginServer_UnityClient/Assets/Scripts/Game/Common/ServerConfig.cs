using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace rater193.scb.common
{
	[System.Serializable]
	public class ServerConfigData
	{
		public string worldName = "DEFAULT";
		public string password = "";
		public int port = 8000;
	}

	public class ServerConfig : MonoBehaviour
	{
		//Public variables
		public static ServerConfigData serverConfigData = new ServerConfigData();

		//For loading the config data
		public static int port { get { return serverConfigData.port; } set { serverConfigData.port = value; } }
		public static string password { get { return serverConfigData.password; } set { serverConfigData.password = value; } }
		public static string WorldName { get { return serverConfigData.worldName; } set { serverConfigData.worldName = value; } }


		//For getting the path to the world
		public static string BasePath { get { return "./SAVEDATA/"; } }
		public static string PathConfig { get { return BasePath + "/server-config.json"; } }
		public static string PathWorld { get { return BasePath + "/" + WorldName + "/"; } }
		public static string PathWorldServer { get { return PathWorld + "/Server/"; } }
		public static string PathWorldServerPlayerData { get { return PathWorldServer + "/Players/"; } }
		public static string PathWorldServerConfig { get { return PathWorldServer + "/Config/"; } }

		//If this is false, when the server loads, then the client will not be loaded directly after. This is for dedicated server support
		public static bool hostAndPlay = false;

		//Saving the config
		public static void save()
		{
			File.WriteAllText(PathConfig, JsonUtility.ToJson(serverConfigData, true));
		}

		//Loading the server config
		public static void load()
		{

			if(!File.Exists(PathConfig) || hostAndPlay==true)
			{
				save();
			}
			else
			{
				serverConfigData = JsonUtility.FromJson<ServerConfigData>(File.ReadAllText(PathConfig));
			}

			DirCheck(PathWorld);
			DirCheck(PathWorldServer);
			DirCheck(PathWorldServerPlayerData);
			DirCheck(PathWorldServerConfig);
		}

		//A quick utility method to create directories if they do not exsist
		private static void DirCheck(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}
	}
}