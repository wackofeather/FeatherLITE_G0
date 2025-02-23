using UnityEngine;

public class Screen_UI : MonoBehaviour
{
    public GameObject Container;
    public Screen_UI_Controller controller;
    public int Key;

    public virtual void EnableScreen()
    {
        Container.SetActive(true);
    }
    public virtual void DisableScreen()
    {
        Container.SetActive(false);
    }
    public virtual void OnScreenSwitch(int newScreen)
    {
        controller.SwitchScreens(newScreen);
        //GameObject.FindObjectOfType<UI_Manager>().instance.SwitchScreens(newScreen);
        //Debug.Log(UI_Manager.instance);
        //Debug.Log(newScreen);
    }
}