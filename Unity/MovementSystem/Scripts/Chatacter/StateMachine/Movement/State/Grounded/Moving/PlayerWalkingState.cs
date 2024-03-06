using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalkingState : PlayerMovingState
{
    private PlayerWalkData m_WalkData;
    
    public PlayerWalkingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        m_WalkData =  movementData.WalkData;
    }

    #region IStateMethods
    public override void Enter()
    {
        stateMachine.ReusableData.MovementSpeedModifier = m_WalkData.SpeedModifier;
        
        stateMachine.ReusableData.BackwardsCameraRecenteringData = m_WalkData.BackwardsCameraRecenteringData;
        
        base.Enter();
        
        StartAnimation(stateMachine.Player.AnimationData.WalkParameterHash);
        
        stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.WeakForce;
    }

    public override void Exit()
    {
        base.Exit();
        
        StopAnimation(stateMachine.Player.AnimationData.WalkParameterHash);

        SetBaseCameraRecenteringData();
    }

    #endregion
    
    #region Input Methods
    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.LightStoppingState);
        
        base.OnMovementCanceled(context);
    }

    protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
    {
        base.OnWalkToggleStarted(context);
        
        stateMachine.ChangeState(stateMachine.RunningState);
    }
    #endregion

}