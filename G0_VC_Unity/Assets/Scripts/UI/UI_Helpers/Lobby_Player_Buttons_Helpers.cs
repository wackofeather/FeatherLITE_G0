using Steamworks;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Lobby_Player_Buttons_Helpers : MonoBehaviour
{
    public Button button;
    public TMP_Text tmpText;
    public TMP_Text tmpText2;
    //public List<Color> textColor = new List<Color>(); // Corrected List declaration
    //// Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Awake()
    //{
    //    textColor.Add(new Color(0, 0, 0));
    //    textColor.Add(new Color(0.06f, 0.06f, 0.06f)); // Corrected color values
    //    textColor.Add(new Color(0.12f, 0.12f, 0.12f)); // Corrected color values
    //    textColor.Add(new Color(0.35f, 0.35f, 0.35f)); // Corrected color values
    //    textColor.Add(new Color(0.57f, 0.57f, 0.57f)); // Corrected color values
    //    textColor.Add(new Color(0.01f, 0.01f, 0.01f)); // Corrected color values
    //    textColor.Add(new Color(0.08f, 0.08f, 0.08f)); // Corrected color values
    //}
    public void ConstructButton(Friend friend)
    {
        tmpText.text = friend.Name;
    }
    public void ConstructTeamButton(ulong clasp)
    {
        //button.colors = textColor[0].ConvertTo<ColorBlock>();
        //textColor.RemoveAt(0);

        //button.gameObject.GetComponent<Renderer>().material.color = Color.white;
        //foreach (ulong friend in clasp.Friends)
        //{
            tmpText2.text += LobbyManager.LobbyManager_Instance.memberList.Find(x => x.Id == clasp).Name;
        //}
    }
    public void ConstructButton2(int TeamNumber)
    {
        tmpText2.gameObject.SetActive(true);
        tmpText2.text = TeamNumber.ToString();
    }
}
