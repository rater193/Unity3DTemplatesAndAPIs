using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rater193.utils
{
	public class PersistantGameObject : MonoBehaviour
	{
		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}
	}
}