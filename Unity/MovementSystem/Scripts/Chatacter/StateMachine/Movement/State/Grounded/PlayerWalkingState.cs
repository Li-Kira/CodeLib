using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalkingState : PlayerGroundedState
{
    public PlayerWalkingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    #region IStateMethods
    public override void Enter()
    {
        base.Enter();

        speedModifier = 0.225f;
    }   
    #endregion
    
    #region Input Methods
    protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
    {
        base.OnWalkToggleStarted(context);
        
        StateMachine.ChangeState(StateMachine.RunningState);
    }
    #endregion

}