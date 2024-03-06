using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDashingState : PlayerGroundedState
{
    private PlayerDashData DashData;
    private float startTime;
    private int consecutiveDashesUsed;
    private bool shouldKeepRotating;
    public PlayerDashingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        DashData = movementData.DashData;
    }

    #region IState Methods
    public override void Enter()
    {
        stateMachine.ReusableData.MovementSpeedModifier = DashData.SpeedModifier;
        
        base.Enter();
        
        StartAnimation(stateMachine.Player.AnimationData.DashParameterHash);

        stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StrongForce;
        
        stateMachine.ReusableData.RotationData = DashData.RotationData;

        Dash();
        
        shouldKeepRotating = stateMachine.ReusableData.MovementInput != Vector2.zero;
        
        UpdateConsecutiveDashes();
        
        startTime = Time.time;
    }

    public override void Exit()
    {
        base.Exit();
        
        StopAnimation(stateMachine.Player.AnimationData.DashParameterHash);
        
        SetBaseRotationData();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!shouldKeepRotating)
        {
            return;
        }
        
        RotateTowardsTargetRotation();
    }

    public override void OnAnimationTransitionEvent()
    {
        if (stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.HardStoppingState);
            return;
        }
        
        stateMachine.ChangeState(stateMachine.SprintingState);
    }
    #endregion

    #region Main Methods
    private void Dash()
    {
        Vector3 dashDirection = stateMachine.Player.transform.forward;
        dashDirection.y = 0f;

        UpdateTargetRotation(dashDirection, false);
        
        if (stateMachine.ReusableData.MovementInput != Vector2.zero)
        {
            UpdateTargetRotation(GetMovementInputDirection());

            dashDirection = GetTargetRotationDirection(stateMachine.ReusableData.CurrentTargetRotation.y);
        }
        
        stateMachine.Player.Rigidbody.velocity = dashDirection * GetMovementSpeed(false);
    }
    
    private void UpdateConsecutiveDashes()
    {
        if (!IsConsecutive())
        {
            consecutiveDashesUsed = 0;
        }

        ++consecutiveDashesUsed;

        if (consecutiveDashesUsed == DashData.ConsecutiveDashesLimitAmount)
        {
            consecutiveDashesUsed = 0;
            stateMachine.Player.Input.DisableActionFor(stateMachine.Player.Input.PlayerActions.Dash, DashData.DashLimitReachedCooldown);
        }
    }

    private bool IsConsecutive()
    {
        return Time.time < startTime + DashData.TimeToBeConsideredConsecutive;
    }

    #endregion

    #region Reusable Methods

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();
        stateMachine.Player.Input.PlayerActions.Movement.performed += OnMovementPerformed;
    }

    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
        stateMachine.Player.Input.PlayerActions.Movement.performed -= OnMovementPerformed;
    }

    #endregion
    
    #region Input Methods

    private void OnMovementPerformed(InputAction.CallbackContext obj)
    {
        shouldKeepRotating = true;
    }
    
    protected override void OnDashStarted(InputAction.CallbackContext context)
    {
    }
    #endregion

}
