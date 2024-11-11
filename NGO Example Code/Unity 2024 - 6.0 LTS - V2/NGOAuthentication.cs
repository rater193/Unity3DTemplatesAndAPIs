using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.scb.arena
{
	public class NGOAuthentication
	{
		public static string DisplayName = "NULL";

		public async static Task Start(string DisplayName)
		{
			Debug.Log("Initializing Unity Services");
			//Here we initiate the connection to the unity services
			await UnityServices.InitializeAsync();

			//Now we configure the local user profile
			InitializationOptions options = GetInitOptions(DisplayName);

			//Here we are setting up the 
			AuthenticationService.Instance.SignedIn += () => {
				Debug.Log("Authenticated / Player ID: " + AuthenticationService.Instance.PlayerId);
			};

			//Here we are signing in.
			await AuthenticationService.Instance.SignInAnonymouslyAsync();
		}

		public static InitializationOptions GetInitOptions(string DisplayName)
		{

			InitializationOptions options = new InitializationOptions();

#if UNITY_EDITOR
			//Here we are doing some unity editor tweaking so we can specify playernames based
			//off the player instance that was used
			string tag = Unity.Multiplayer.Playmode.CurrentPlayer.ReadOnlyTags()[0];

			//Here we can set some custom display names
			if (tag == "P1")
			{
				DisplayName = "PlayerInstP1";
			}
			else if (tag == "P2")
			{
				DisplayName = "PlayerInstP2";
			}
			else if (tag == "P3")
			{
				DisplayName = "PlayerInstP3";
			}
			else if (tag == "P4")
			{
				DisplayName = "PlayerInstP4";
			}
#endif

			//Here we are setting the profile
			options.SetProfile(DisplayName);
			NGOAuthentication.DisplayName = DisplayName;

			return options;
		}
	}
}