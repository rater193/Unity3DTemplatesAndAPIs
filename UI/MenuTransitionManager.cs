using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTransitionManager : MonoBehaviour
{
	public List<CanvasGroup> menus;
	private List<Vector3> menuPositions;
	private CanvasGroup selectedMenu = null;

	private IEnumerator Start()
	{
		menuPositions = new List<Vector3>();
		foreach(CanvasGroup cg in menus)
		{
			menuPositions.Add(cg.transform.position);
			cg.interactable = false;
			cg.alpha = 0f;
			cg.blocksRaycasts = false;
		}

		yield return SelectMenuEnumerator(menus[0]);
	}

	public void SelectMenu(CanvasGroup targetMenu)
	{
		StartCoroutine(SelectMenuEnumerator(targetMenu));
	}

	public IEnumerator SelectMenuEnumerator(CanvasGroup targetMenu)
	{
		Debug.Log($"Selecting {targetMenu}");
		yield return null;
		if(selectedMenu == null)
		{
			selectedMenu = targetMenu;
			selectedMenu.interactable = true;
			selectedMenu.blocksRaycasts = true;
			selectedMenu.alpha = 1f;
		}
		else
		{
			if(!GetComponent<MenuTransitionAnimation>())
			{
				Debug.LogError("Be sure to add a MenuTransitionAnimation controller onto this object.");
			}
			else
			{
				if(menus.Contains(targetMenu))
				{
					CanvasGroup menuCurrent = selectedMenu;
					CanvasGroup menuTarget = targetMenu;

					Vector3 menuCurrentPosition = menuPositions[menus.IndexOf(menuCurrent)];
					Vector3 menuTargetPosition = menuPositions[menus.IndexOf(targetMenu)];

					menuCurrent.interactable = false;
					menuCurrent.blocksRaycasts = false;
					menuTarget.interactable = false;
					menuTarget.blocksRaycasts = false;

					yield return GetComponent<MenuTransitionAnimation>().Animate(menuTargetPosition, menuTarget, menuCurrentPosition, menuCurrent);
					
					menuTarget.interactable = true;
					menuTarget.blocksRaycasts = true;

					selectedMenu = targetMenu;
				}
				else
				{
					Debug.LogError("Menu not stored within the menus list. Please add it before selecting it with this object.");
				}
			}
		}
	}
}
