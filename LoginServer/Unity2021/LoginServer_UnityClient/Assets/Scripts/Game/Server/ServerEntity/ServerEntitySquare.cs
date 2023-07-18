using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rater193.scb.server
{
	public class ServerEntitySquare : ServerEntity
	{
		public ServerEntitySquare(GameServerMap map, Vector3 position, float rotation = 0, ServerEntity parent = null) : base(map, position, rotation, parent)
		{
			this.gameObject.name = "ServerEntitySquare-" + this.entityID;
		}

		public override void OnDelete()
		{

		}

		public override void OnUpdate()
		{

		}
	}
}
