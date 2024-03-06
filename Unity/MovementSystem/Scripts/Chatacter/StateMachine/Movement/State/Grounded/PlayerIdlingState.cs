using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdlingState : PlayerGroundedState
{
    private PlayerIdleData m_IdleData;
    
    public PlayerIdlingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        m_IdleData = movementData.IdleData;
    }

    #region IStateMethods
    public override void Enter()
    {
        stateMachine.ReusableData.MovementSpeedModifier = 0f;

        stateMachine.ReusableData.BackwardsCameraRecenteringData = m_IdleData.BackwardsCameraRecenteringData;

        base.Enter();
        
        StartAnimation(stateMachine.Player.AnimationData.IdleParameterHash);
        
        stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StationaryForce;
        
        ResetVelocity();
    }

    public override void Exit()
    {
        base.Exit();
        
        StopAnimation(stateMachine.Player.AnimationData.IdleParameterHash);
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            return;
        }

        OnMove();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!IsMovingHorizontally())
        {
            return;
        }
        
        ResetVelocity();
    }

    #endregion

}