using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    public PlayerBase.BasePlayerState CurrentPlayerState { get; private set; }

    public void Initialize(PlayerBase.BasePlayerState startingState)
    {
        CurrentPlayerState = startingState;
        CurrentPlayerState.EnterState();
    }

    public void ChangeState(PlayerBase.BasePlayerState newState)
    {
        CurrentPlayerState.ExitState();
        CurrentPlayerState = newState;
        CurrentPlayerState.EnterState();
    }
}
