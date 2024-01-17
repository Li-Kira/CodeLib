using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGroundedState : PlayerMovementState
{
    public PlayerGroundedState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }
    
    #region Reusable Methods
    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();

        StateMachine.Player.Input.PlayerActions.Movement.canceled += OnMovementCanceled;
    }
    
    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
        
        StateMachine.Player.Input.PlayerActions.Movement.canceled -= OnMovementCanceled;
    }
    
    protected virtual void OnMove()
    {
        if (shouldWalk)
        {
            StateMachine.ChangeState(StateMachine.WalkingState);
            return;
        }
        
        StateMachine.ChangeState(StateMachine.RunningState);
    }
    #endregion    
    
    #region Input Methods
    protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
    {
        StateMachine.ChangeState(StateMachine.IdlingState);
    }
    #endregion
}
