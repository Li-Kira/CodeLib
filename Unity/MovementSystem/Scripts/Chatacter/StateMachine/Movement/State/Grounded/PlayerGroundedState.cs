using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGroundedState : PlayerMovementState
{
    private SlopeData SlopeData;
    
    public PlayerGroundedState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        SlopeData = stateMachine.Player.ColliderUtility.SlopeData;
    }

    #region IState Methods

    public override void Enter()
    {
        base.Enter();
        
        StartAnimation(stateMachine.Player.AnimationData.GroundedParameterHash);

        UpdateShouldSprintState();
        
        UpdateCameraRecenteringState(stateMachine.ReusableData.MovementInput);
    }

    public override void Exit()
    {
        base.Exit();
        
        StopAnimation(stateMachine.Player.AnimationData.GroundedParameterHash);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        FloatCapsule();
    }
    #endregion

    #region Main Methods
    private void UpdateShouldSprintState()
    {
        if (!stateMachine.ReusableData.ShouldSprint)
        {
            return;
        }

        if (stateMachine.ReusableData.MovementInput != Vector2.zero)
        {
            return;            
        }

        stateMachine.ReusableData.ShouldSprint = false;
    }
    
    private void FloatCapsule()
    {
        Vector3 capsuleColliderCenterInWorldSpace =
            stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;
        // Vector3.down Always Down in WolrdSpace
        Ray downardRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);
        if (Physics.Raycast(downardRayFromCapsuleCenter, out RaycastHit hit, SlopeData.FloatRayDistance, 
                stateMachine.Player.PlayerLayerData.GroundLayer, QueryTriggerInteraction.Ignore))
        {
            float groudAngle = Vector3.Angle(hit.normal, -downardRayFromCapsuleCenter.direction);
            float slopeSpeedModifier = SetSlopeSpeedModifierOnAngle(groudAngle);
            if (slopeSpeedModifier == 0f)
            {
                return;
            }
            
            float distanceToFloatingPoint =
                stateMachine.Player.ColliderUtility.CapsuleColliderData.ColliderCenterInLocalSpace.y *
                stateMachine.Player.transform.localScale.y - hit.distance;

            if (distanceToFloatingPoint == 0f)
            {
                return;
            }

            float amountToLift = distanceToFloatingPoint * SlopeData.StepReachForce - GetPlayerVerticalVelocity().y;
            Vector3 liftForce = new Vector3(0f, amountToLift, 0f);
            stateMachine.Player.Rigidbody.AddForce(liftForce, ForceMode.VelocityChange);
        }
        
    }

    private float SetSlopeSpeedModifierOnAngle(float angle)
    {
        float slopeSpeedModifier = movementData.SlopeSpeedAngles.Evaluate(angle);

        if (stateMachine.ReusableData.MovementOnSlopeSpeedModifier != slopeSpeedModifier)
        {
            stateMachine.ReusableData.MovementOnSlopeSpeedModifier = slopeSpeedModifier;
            
            UpdateCameraRecenteringState(stateMachine.ReusableData.MovementInput);
        }
        
        
        return slopeSpeedModifier;
    }
    
    private bool IsThereGroundUnderneath()
    {
        BoxCollider groundCheckCollider = stateMachine.Player.ColliderUtility.TriggerColliderData.GroudCheckCollider;
        Vector3 groundColliderCenterInWorldSpace = groundCheckCollider.bounds.center;
    
        Collider[] overlappedGroundColliders = Physics.OverlapBox(
            groundColliderCenterInWorldSpace,
            stateMachine.Player.ColliderUtility.TriggerColliderData.GroundCheckColliderExtents,
            groundCheckCollider.transform.rotation, 
            stateMachine.Player.PlayerLayerData.GroundLayer,
            QueryTriggerInteraction.Ignore);
    
        return overlappedGroundColliders.Length > 0;
    }


    #endregion

    #region Reusable Methods
    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();
        
        stateMachine.Player.Input.PlayerActions.Dash.started += OnDashStarted;
        stateMachine.Player.Input.PlayerActions.Jump.started += OnJumpStarted;
    }
    
    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
        
        stateMachine.Player.Input.PlayerActions.Dash.started -= OnDashStarted;
        stateMachine.Player.Input.PlayerActions.Jump.started -= OnJumpStarted;
    }
    
    protected virtual void OnMove()
    {
        if (stateMachine.ReusableData.ShouldSprint)
        {
            stateMachine.ChangeState(stateMachine.SprintingState);
            return;
        }
        
        if (stateMachine.ReusableData.ShouldWalk)
        {
            stateMachine.ChangeState(stateMachine.WalkingState);
            return;
        }
        
        stateMachine.ChangeState(stateMachine.RunningState);
    }

    protected override void OnContactWithGroundExited(Collider collider)
    {
        base.OnContactWithGroundExited(collider);

        if (IsThereGroundUnderneath())
        {
            return;
        }

        Vector3 colliderCenterInWorldSpace =
            stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;
        // 从胶囊碰撞体的底部发射射线
        Ray downwardsRayFromCapsuleBottom =
            new Ray(
                colliderCenterInWorldSpace -
                stateMachine.Player.ColliderUtility.CapsuleColliderData.ColliderVerticalExtents, Vector3.down);
        //  out _表示改变量弃置，没有值
        if (!Physics.Raycast(downwardsRayFromCapsuleBottom, out _, movementData.GroundToFallRayDistance,
                stateMachine.Player.PlayerLayerData.GroundLayer, QueryTriggerInteraction.Ignore))
        {
            OnFall();
        }
            
        
    }
    
    protected virtual void OnFall()
    {
        stateMachine.ChangeState(stateMachine.FallingState);
    }
    #endregion    
    
    #region Input Methods

    protected virtual void OnDashStarted(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.DashingState);
    }
    
    protected virtual void OnJumpStarted(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.JumpingState);
    }
    #endregion
}
