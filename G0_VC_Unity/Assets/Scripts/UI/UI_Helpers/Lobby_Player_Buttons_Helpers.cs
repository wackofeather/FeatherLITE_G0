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
    public ulong ButtonId;
    public int Team_Index;
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
    public void ConstructFriendButton(Friend friend)
    {
        tmpText.text = friend.Name;
        ButtonId = friend.Id;
    }
    public void ConstructTeamButton()
    {
        //Color randomColor = new Color(Random.value, Random.value, Random.value);
        //ColorBlock colorBlock = button.colors;
        //colorBlock.normalColor = randomColor;
        //button.colors = colorBlock;
        /*tmpText.text = "Switch to Team " + Team_Index + 1;*/
    }
}
