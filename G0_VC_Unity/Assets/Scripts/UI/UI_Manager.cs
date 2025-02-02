using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance { get; set; }
    Dictionary<int, Screen_UI> ScreenDict = new Dictionary<int, Screen_UI>();
    public int currentScreen; 
    private void Awake()
    {
        ConstructSingleton();
        List<Screen_UI> ScreenList = Resources.FindObjectsOfTypeAll<Screen_UI>().ToList();
        foreach (Screen_UI screen in ScreenList)
        {
            ScreenDict.Add(screen.Key, screen);
            if (screen.Key==0)//Default Screen is 0, that's why its 0.
            {
                screen.EnableScreen();
                currentScreen = 0;

            }
        }
    }
    public void SwitchScreens(int newScreen)
    {
        ScreenDict[currentScreen].DisableScreen();
        ScreenDict[newScreen].EnableScreen();
        currentScreen = newScreen;
    }
    public virtual void ConstructSingleton()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
}
