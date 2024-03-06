using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSprintingState : PlayerMovingState
{
    private PlayerSprintData SprintData;
    private float startTime;
    private bool keepSprinting;
    private bool shouldResetSprintState;
    
    public PlayerSprintingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        SprintData = movementData.SprintData;
    }

    #region IState Methods
    public override void Enter()
    {
        stateMachine.ReusableData.MovementSpeedModifier = SprintData.SpeedModifier;
        
        base.Enter();
        
        StartAnimation(stateMachine.Player.AnimationData.SprintParameterHash);
        
        stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StrongForce;

        shouldResetSprintState = true;
        
        startTime = Time.time;
    }

    public override void Update()
    {
        base.Update();

        if (keepSprinting)
        {
            return;
        }

        if (Time.time < startTime + SprintData.SprintToRunTime)
        {
            return;
        }

        StopSprint();
    }

    public override void Exit()
    {
        base.Exit();
        
        StopAnimation(stateMachine.Player.AnimationData.SprintParameterHash);
        
        if (shouldResetSprintState)
        {
            keepSprinting = false;
            stateMachine.ReusableData.ShouldSprint = false;
        }
    }

    #endregion

    #region Main Methods
    private void StopSprint()
    {
        if (stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.IdlingState);
            return;
        }
        
        stateMachine.ChangeState(stateMachine.RunningState);
    }
    #endregion
    
    #region Reusable Methods
    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();
        stateMachine.Player.Input.PlayerActions.Sprint.performed += OnSpringPerformed;
    }
    
    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
    }

    protected override void OnFall()
    {
        shouldResetSprintState = false;
        
        base.OnFall();
    }

    #endregion

    #region Input Methods
    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.HardStoppingState);
        
        base.OnMovementCanceled(context);
    }

    protected override void OnJumpStarted(InputAction.CallbackContext context)
    {
        shouldResetSprintState = false;
        base.OnJumpStarted(context);
    }

    private void OnSpringPerformed(InputAction.CallbackContext context)
    {
        keepSprinting = true;

        stateMachine.ReusableData.ShouldSprint = true;
    }
    #endregion
}

