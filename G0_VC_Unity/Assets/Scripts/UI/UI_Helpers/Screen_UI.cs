using UnityEngine;

public class Screen_UI : MonoBehaviour
{
    [SerializeField] public GameObject Container;

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
        UI_Manager.instance.SwitchScreens(newScreen);
        //GameObject.FindObjectOfType<UI_Manager>().instance.SwitchScreens(newScreen);
        Debug.Log(UI_Manager.instance);
        Debug.Log(newScreen);
    }
}