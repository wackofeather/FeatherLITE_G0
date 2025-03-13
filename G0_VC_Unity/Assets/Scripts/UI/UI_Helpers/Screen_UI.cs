using System.Threading.Tasks;
using UnityEngine;

public class Screen_UI : MonoBehaviour
{
    public GameObject Container;
    public Screen_UI_Controller controller;
    public int Key;
    public CanvasGroup canvasGroup;
    public async virtual Task<bool> EnableScreen()
    {
        Container.SetActive(true);
        await Task.Yield();
        Debug.LogWarning("Work");
        return true;
    }
    public async virtual Task<bool> DisableScreen()
    {
        Container.SetActive(false);
        await Task.Yield();
        return true;
    }
    public virtual void OnScreenSwitch(int newScreen)
    {
        controller.SwitchScreens(newScreen);
        //GameObject.FindObjectOfType<UI_Manager>().instance.SwitchScreens(newScreen);
        //Debug.Log(UI_Manager.instance);
        //Debug.Log(newScreen);
    }
}