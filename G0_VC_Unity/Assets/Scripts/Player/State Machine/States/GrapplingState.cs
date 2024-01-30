using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerBase
{
    public class GrapplingState : BasePlayerState
    {
        public GrapplingState(PlayerScript player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
        {
        }

        public override void AnimationTriggerEvent()
        {
            base.AnimationTriggerEvent();
        }

        public override void EnterState()
        {
            base.EnterState();
        }

        public override void ExitState()
        {
            base.ExitState();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}

