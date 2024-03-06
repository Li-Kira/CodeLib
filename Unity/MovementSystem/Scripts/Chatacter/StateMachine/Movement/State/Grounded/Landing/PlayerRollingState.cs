using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRollingState : PlayerLandingState
{
    private PlayerRollData RollData;
    public PlayerRollingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        RollData = movementData.RollData;
    }

    #region IState Methods

    public override void Enter()
    {
        stateMachine.ReusableData.MovementSpeedModifier = RollData.SpeedModifier;
        
        base.Enter();
        
        StartAnimation(stateMachine.Player.AnimationData.RollParameterHash);
        
        stateMachine.ReusableData.ShouldSprint = false;
    }

    public override void Exit()
    {
        base.Exit();
        
        StopAnimation(stateMachine.Player.AnimationData.RollParameterHash);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (stateMachine.ReusableData.MovementInput != Vector2.zero)
        {
            return;
        }

        RotateTowardsTargetRotation();
    }

    public override void OnAnimationTransitionEvent()
    {
        if (stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.MediumStoppingState);
            return;
        }
        
        OnMove();
    }

    #endregion

    #region Input Methods

    // 这里置空，因为我们不希望在滚动途中跳跃
    protected override void OnJumpStarted(InputAction.CallbackContext context)
    {
    }

    #endregion
    
    
}
