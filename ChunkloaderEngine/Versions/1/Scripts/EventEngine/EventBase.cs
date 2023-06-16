using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class EventBase
{
	private bool isCancled = false;

	public virtual bool CanExecute()
	{
		return !isCancled;
	}

	public virtual void Cancel()
	{
		isCancled = true;
	}

	public virtual bool IsCanceled()
	{
		return isCancled;
	}
}

public class EventOnChunkSaved : EventBase { }
public class EventOnChunkLoaded : EventBase { }
