using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunningState : PlayerMovingState
{
    private PlayerSprintData SprintData;
    private float startTime;
    
    public PlayerRunningState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        SprintData = movementData.SprintData;
    }

    #region IStateMethods
    public override void Enter()
    {
        stateMachine.ReusableData.MovementSpeedModifier = movementData.RunData.SpeedModifier;
        
        base.Enter();
        
        StartAnimation(stateMachine.Player.AnimationData.RunParameterHash);
        
        stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.MediumForce;

        startTime = Time.time;
    }

    public override void Exit()
    {
        base.Exit();
        
        StopAnimation(stateMachine.Player.AnimationData.RunParameterHash);
    }

    public override void Update()
    {
        base.Update();

        if (!stateMachine.ReusableData.ShouldWalk)
        {
            return;
        }

        if (Time.time < startTime + SprintData.SprintToRunTime)
        {
            return;
        }

        StopRunning();
    }
    #endregion

    #region Main Methods
    private void StopRunning()
    {
        if (stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.IdlingState);
            return;
        }
        
        stateMachine.ChangeState(stateMachine.WalkingState);
    }
    #endregion

    #region Input Methods
    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.MediumStoppingState);
        
        base.OnMovementCanceled(context);
    }
    
    protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
    {
        base.OnWalkToggleStarted(context);
        
        stateMachine.ChangeState(stateMachine.WalkingState);
    }
    #endregion
}