using UnityEngine;
using UnityEngine.UI;

public class ToggleScriptOnManager : MonoBehaviour
{
    public Toggle toggle;
    public Toggle toggle2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        if(LobbyManager.LobbyManager_Instance.CurrentGameMode_Int.Value == 0&&LobbyManager.LobbyManager_Instance.CurrentGameMode_Int!=null)
        {
            toggle.isOn = true;
            toggle2.isOn = false;
        }
        else
        {
            toggle.isOn = false;
            toggle2.isOn = true;
        }
    }
}
