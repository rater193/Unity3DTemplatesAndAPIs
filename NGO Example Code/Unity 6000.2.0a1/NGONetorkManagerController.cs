using Unity.Netcode;
using UnityEngine;

namespace com.multigame.multiplayer
{
    public class NGONetorkManagerController : MonoBehaviour
    {
        NetworkManager networkManager;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //Here we can set the reference programtically if it has not been set.
            if(networkManager == null) networkManager = GetComponent<NetworkManager>();

            if(networkManager)
            {
                networkManager.OnServerStarted += () =>
                {
                    gameObject.AddComponent<GameServer>();
                };
			}

		}
    }
}
