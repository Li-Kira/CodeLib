using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunningState : PlayerGroundedState
{
    public PlayerRunningState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    #region IStateMethods
    public override void Enter()
    {
        base.Enter();

        speedModifier = 1f;
    }
    #endregion

    #region Input Methods
    protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
    {
        base.OnWalkToggleStarted(context);
        
        StateMachine.ChangeState(StateMachine.WalkingState);
    }
    #endregion
}