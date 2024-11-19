using System.Collections;
using UnityEngine;

public class MenuTransitionAnimation : MonoBehaviour
{
	public virtual IEnumerator Animate(Vector3 menuInPos, CanvasGroup menuIn, Vector3 menuOutPos, CanvasGroup menuOut)
	{
		yield return null;
	}
}
