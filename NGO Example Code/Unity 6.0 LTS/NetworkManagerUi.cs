using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace com.rater193.twodspacefun
{
    public class NetworkManagerUi : MonoBehaviour
	{
		[SerializeField] private Button serverBtn;
		[SerializeField] private Button hostBtn;
		[SerializeField] private Button clientBtn;


		private void Awake()
		{
			Debug.Log("NetworkManagerUIStart");
		}

		public void StartServer()
		{
			Debug.Log("Click Server");
			NetworkManager.Singleton.StartServer();
			Destroy(gameObject);
		}

		public void StartClient()
		{
			Debug.Log("Click Client");
			NetworkManager.Singleton.StartClient();
			Destroy(gameObject);
		}

		public void StartHost()
		{
			Debug.Log("Click Host");
			NetworkManager.Singleton.StartHost();
			Destroy(gameObject);
		}
	}
}
