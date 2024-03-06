using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallingState : PlayerAirborneState
{
    private PlayerFallData m_FallData;
    private Vector3 playerPositionOnEnter;
    
    public PlayerFallingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        m_FallData = airborneData.FallData;
    }

    #region IState Methods

    public override void Enter()
    {
        base.Enter();
        
        StartAnimation(stateMachine.Player.AnimationData.FallParameterHash);
        
        stateMachine.ReusableData.MovementSpeedModifier = 0f;
        
        playerPositionOnEnter = stateMachine.Player.transform.position;
        
        ResetVerticalVelocity();
    }

    public override void Exit()
    {
        base.Exit();
        
        StopAnimation(stateMachine.Player.AnimationData.FallParameterHash);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        LimitVerticalVelocity();
    }
    
    #endregion

    #region Reusable Methods

    protected override void ResetSprintState()
    {
    }

    protected override void OnContactWithGround(Collider collider)
    {
        float fallDistance = playerPositionOnEnter.y - stateMachine.Player.transform.position.y;
        if (fallDistance < m_FallData.MinimumDistanceToBeConsideredHardFall)
        {
            stateMachine.ChangeState(stateMachine.LightLandingState);
            return;
        }

        if (stateMachine.ReusableData.ShouldWalk && !stateMachine.ReusableData.ShouldSprint || stateMachine.ReusableData.MovementInput ==  Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.HardLandingState);
            return;
        }
        
        stateMachine.ChangeState(stateMachine.RollingState);
        
    }

    #endregion
    
    #region Main Methods

    private void LimitVerticalVelocity()
    {
        Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();
        if (playerVerticalVelocity.y >= -m_FallData.FallSpeedLimit)
        {
            return;
        }

        Vector3 limitVelocity =
            new Vector3(0f, -m_FallData.FallSpeedLimit - playerVerticalVelocity.y, 0f);
        
        stateMachine.Player.Rigidbody.AddForce(limitVelocity, ForceMode.VelocityChange);
    }

    #endregion

}
