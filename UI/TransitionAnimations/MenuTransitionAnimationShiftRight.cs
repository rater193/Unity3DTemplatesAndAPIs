using System.Collections;
using UnityEngine;

public class MenuTransitionAnimationShiftRight : MenuTransitionAnimation
{
	public float horizontalOffset = 128f;
	public float animationSpeed = 1f;

	public override IEnumerator Animate(Vector3 menuInPos, CanvasGroup menuIn, Vector3 menuOutPos, CanvasGroup menuOut)
	{
		yield return base.Animate(menuInPos, menuIn, menuOutPos, menuOut);


		for (float _ = 0; _ < 1f; _+=Time.deltaTime* animationSpeed)
		{
			//Alpha multipliers
			float menuAlphaIn = _;
			float menuAlphaInOut = 1-_;

			//Fading the menus
			menuIn.alpha = menuAlphaIn;
			menuOut.alpha = menuAlphaInOut;

			//Shifting the menus
			menuIn.transform.position = menuInPos + (new Vector3(horizontalOffset, 0, 0) * menuAlphaInOut);
			menuOut.transform.position = menuOutPos + (new Vector3(horizontalOffset, 0, 0) * menuAlphaIn);

			yield return new WaitForEndOfFrame();
		}
		menuIn.alpha = 1;
		menuOut.alpha = 0;

	}
}
