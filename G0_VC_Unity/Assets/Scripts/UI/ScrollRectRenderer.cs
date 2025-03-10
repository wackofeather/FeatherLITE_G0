using UnityEngine;
using UnityEngine.UI;

public class ScrollRectRenderer : MonoBehaviour
{
    public static ScrollRectRenderer scrollRectRendererInstance{ get; set; }
    public Transform scrollVector;
    public GameObject MenuPrefabButton;
    public VerticalLayoutGroup VerticalLayoutGroup;
    public void CreateButton(ulong friend)
    {
        GameObject Button;
        Button = Instantiate(MenuPrefabButton, scrollVector);
        Button.GetComponent<Lobby_Player_Buttons_Helpers>().ConstructTeamButton(friend);
    }

    private void Awake()
    {
        CounstructRectRendererInstance();
    }
    public void CounstructRectRendererInstance()
    {
        if (scrollRectRendererInstance != null && scrollRectRendererInstance != this)
        {
            Destroy(this);
        }
        else
        {
            scrollRectRendererInstance = this;
        }
    }
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
