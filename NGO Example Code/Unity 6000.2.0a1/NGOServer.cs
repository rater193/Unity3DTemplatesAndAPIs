using System.Threading.Tasks;
using UnityEngine;

namespace com.multigame.multiplayer
{
	public class NGOServer
	{
		public async static Task Start()
		{
			NGOUtil.SpawnNetworkManager();
		}
	}
}