using rater193.scb.common;
using rater193.scb.global;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rater193.scb.server
{
	public class ServerEntityPlayer : ServerEntity
	{
		public User user;
		public float input_hor = 0f;
		public float input_vert = 0f;

		public ServerEntityPlayer(GameServerMap map, Vector3 position, float rotation = 0, ServerEntity parent = null) : base(map, position, rotation, parent)
		{
			gameObject.name = "ServerEntityPlayer-" + this.entityID;
			renderMode = EnumRenderMode.Box;
		}

		public override void OnDelete()
		{

		}

		public override void OnUpdate()
		{
			//throw new System.NotImplementedException();
		}
	}
}
