using rater193.utils.menucontroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuLoginServiceEventListener : MonoBehaviour
{
    private static bool hasLoggedIn = false;
    public rater193.utils.menucontroller.MenuController menuController;
    public MenuItem
        failedLoginMenu,
        failedRegistrationMenu,
        mainMenu,
        loginMenu
        ;
    // Start is called before the first frame update
    void Start()
    {
        if (hasLoggedIn == false)
        {
            LoginService.onAccountFailedLogin = (reason) =>
            {
                menuController.debugTestSelectedMenu = failedLoginMenu;
            };
            LoginService.onAccountFailedRegistration = (reason) =>
            {
                menuController.debugTestSelectedMenu = failedLoginMenu;
            };
            LoginService.onAccountRegistered = () =>
            {
                menuController.debugTestSelectedMenu = loginMenu;
            };
            LoginService.onLogin = (sessionKey) =>
            {
                hasLoggedIn = true;
                menuController.debugTestSelectedMenu = mainMenu;
                LoginService.lastLoginClientUsed.ws.Close();
            };
		}
		else
        {
            menuController.debugTestSelectedMenu = mainMenu;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
