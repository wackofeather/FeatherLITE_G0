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
    public virtual void ChildAwake()
    {

    }
    private void Awake()
    {
        Debug.Log("IAM WOrKING");
        ConstructSingleton();
        List<Screen_UI> ScreenList = Resources.FindObjectsOfTypeAll<Screen_UI>().ToList();
        Debug.Log(ScreenList.Count);
        foreach (Screen_UI screen in ScreenList)
        {
            ScreenDict.Add(screen.Key, screen);
            if (screen.Key==0)//Default Screen is 0, that's why its 0.
            {
                Debug.Log("ScreenKEy=0");
                screen.EnableScreen();
                currentScreen = 0;

            }
        }
        ChildAwake();
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
