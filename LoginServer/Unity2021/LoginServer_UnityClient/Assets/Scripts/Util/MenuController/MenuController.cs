using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rater193.utils.menucontroller
{
    public class MenuController : MonoBehaviour
    {
        public MenuItem currentMenu = null;
        public MenuItem debugTestSelectedMenu = null;

        // Start is called before the first frame update
        void Start()
        {
            selectedMenu(currentMenu);
        }

        public void selectedMenu(MenuItem menu)
		{
            //Disable previous menu
            if(currentMenu!=null)
			{
                currentMenu.Deselect();
            }
            //Enable current menu
            if (menu != null)
            {
                menu.Select();
            }
            //Store menu for later use
            currentMenu = menu;
            debugTestSelectedMenu = menu;

        }

		public void Update()
        {
            if (debugTestSelectedMenu != null)
            {
                selectedMenu(debugTestSelectedMenu);
                debugTestSelectedMenu = null;
            }
        }
	}
}
