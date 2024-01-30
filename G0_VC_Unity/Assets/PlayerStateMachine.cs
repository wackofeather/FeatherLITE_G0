using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    public PlayerBase.BasePlayerState CurrentPlayerState;

    public void Initialize(PlayerBase.BasePlayerState startingState)
    {
        CurrentPlayerState = startingState;
        CurrentPlayerState.EnterState();
        Debug.Log("STAAAAAAAAAAAAAAAAAAAAAAAAAARRRRRRRRRRRRRRRRTTTTTIIIINNNNNNGGGGGGGGG");
    }

    public void ChangeState(PlayerBase.BasePlayerState newState)
    {
        CurrentPlayerState.ExitState();
        CurrentPlayerState = newState;
        CurrentPlayerState.EnterState();
    }
}
