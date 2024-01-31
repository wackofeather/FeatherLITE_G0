using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateFactory : MonoBehaviour
{
    PlayerStateMachine _context;


    public PlayerStateFactory( PlayerStateMachine context)
    {
        _context = context;
    }

    
}
