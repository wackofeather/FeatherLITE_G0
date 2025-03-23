using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Screen_UI_Controller : MonoBehaviour
{
    internal Dictionary<int, Screen_UI> ScreenDict = new Dictionary<int, Screen_UI>();
    public int currentScreen;
    public List<Screen_UI> ScreenList = new List<Screen_UI>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        Debug.Log("IAM WOrKING");
        Debug.Log(ScreenList.Count);
        foreach (Screen_UI screen in ScreenList)
        {
            ScreenDict.Add(screen.Key, screen);
            if (screen.Key == 0)//Default Screen is 0, that's why its 0.
            {
                screen.EnableScreen();
                currentScreen = 0;

            }
        }
    }
    public virtual void SwitchScreens(int newScreen)
    {
        Debug.Log(newScreen);
        ScreenDict[currentScreen].DisableScreen();
        ScreenDict[newScreen].EnableScreen();
        currentScreen = newScreen;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
