using Steamworks.Data;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MapButtonHelpers : MapButton
{
    public TMP_Text TextMeshPro;
    //public LobbyManager LobbyManager;
    //public Toggle toggle1;
    //public Toggle toggle2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void TextController()
    {
        //Debug.Log(LobbyManager.LobbyManager_Instance.countDown.Value);
        int countDownTime = Mathf.FloorToInt(LobbyManager.LobbyManager_Instance.countDown.Value);

        TextMeshPro.text = countDownTime.ToString();
    }


    void Start()
    {
        
        InvokeRepeating("TextController", 0, 1);
    }
}
