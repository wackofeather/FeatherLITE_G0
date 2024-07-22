using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_GeneralManager : GeneralManager
{
    new public static Game_GeneralManager instance { get; set; }

    public Dictionary<float, PlayerStateMachine> Player_LookUp { get; private set; }


    public void AddPlayer(float key, PlayerStateMachine player)
    {
        Player_LookUp.Add(key, player);
    }

    public void RemovePlayer(float key, PlayerStateMachine player)
    {
        Player_LookUp.Add(key, player);
    }





    public override void ConstructSingleton()
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
