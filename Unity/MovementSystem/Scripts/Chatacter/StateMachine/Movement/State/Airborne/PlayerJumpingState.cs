using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJumpingState : PlayerAirborneState
{
    private PlayerJumpData JumpData;
    private bool shouldKeepRotating;
    private bool canStartFalling;
    
    public PlayerJumpingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        JumpData = airborneData.JumpData;
    }

    #region IState Methods

    public override void Enter()
    {
        base.Enter();

        stateMachine.ReusableData.MovementSpeedModifier = 0f;

        stateMachine.ReusableData.MovementDecelerationForce = JumpData.DecelerationForce;

        shouldKeepRotating = stateMachine.ReusableData.MovementInput != Vector2.zero;
        stateMachine.ReusableData.RotationData = JumpData.RotationData;
        
        Jump();
    }

    public override void Exit()
    {
        base.Exit();
        
        SetBaseRotationData();

        canStartFalling = false;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (shouldKeepRotating)
        {
            RotateTowardsTargetRotation();
        }

        if (IsMovingUp())
        {
            DecelerateVertically();
        }
    }

    public override void Update()
    {
        base.Update();

        if (!canStartFalling && IsMovingUp(0f))
        {
            canStartFalling = true;
        }
        
        if (!canStartFalling || GetPlayerVerticalVelocity().y > 0)
        {
            return;
        }
        
        stateMachine.ChangeState(stateMachine.FallingState);
    }

    #endregion

    #region Reusable Methods

    protected override void ResetSprintState()
    {
        
    }

    #endregion
    
    #region Main Methods

    private void Jump()
    {
        Vector3 jumpForce = stateMachine.ReusableData.CurrentJumpForce;
        Vector3 jumpDirection = stateMachine.Player.transform.forward;

        if (shouldKeepRotating)
        {
            UpdateTargetRotation(GetMovementInputDirection());
            jumpDirection = GetTargetRotationDirection(stateMachine.ReusableData.CurrentTargetRotation.y);
        }
        
        jumpForce.x *= jumpDirection.x;
        jumpForce.z *= jumpDirection.z;
        
        // 斜坡处理
        Vector3 capsuleColliderCenterInWorldSpace =
            stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;
        Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);
        if (Physics.Raycast(downwardsRayFromCapsuleCenter, out  RaycastHit hit, JumpData.JumpToGroundRayDistance, 
                stateMachine.Player.PlayerLayerData.GroundLayer, QueryTriggerInteraction.Ignore))
        {
            float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

            if (IsMovingUp())
            {
                float forceModifier = JumpData.JumpForceModifierOnSlopeUpwards.Evaluate(groundAngle);

                jumpForce.x *= forceModifier;
                jumpForce.z *= forceModifier;
            }

            if (IsMovingDown())
            {
                float forceModifier = JumpData.JumpForceModifierOnSlopeDownwards.Evaluate(groundAngle);

                jumpForce.y *= forceModifier;
            }
        }
        
        ResetVelocity();
        
        stateMachine.Player.Rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);
    }
    
    #endregion

    #region Input Methods

    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
    }

    #endregion
}
