using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventShouldChunkUnload : EventBase
{
	private bool canUnload = false;

	public virtual void Allow()
	{
		canUnload = true;
	}

	public virtual void Deny()
	{
		canUnload = false;
	}

	public virtual bool CanUnload()
	{
		return canUnload;
	}
}
