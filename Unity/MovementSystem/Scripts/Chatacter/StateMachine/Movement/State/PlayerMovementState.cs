using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PlayerMovementState : IState
{
    protected PlayerMovementStateMachine StateMachine;
    
    protected Vector2 movementInput;
    protected float baseSpeed = 5f;
    protected float speedModifier = 1f;

    protected Vector3 currentTargetRotation;
    protected Vector3 timeToReachTargetRotation;
    protected Vector3 dampedTargetRotationCurrentVelocity;
    protected Vector3 dampedTargetRotationPassedTime;

    protected bool shouldWalk;
    
    public PlayerMovementState(PlayerMovementStateMachine playerMovementStateMachine)
    {
        StateMachine = playerMovementStateMachine;

        InitializeData();
    }

    private void InitializeData()
    {
        timeToReachTargetRotation.y = 0.14f;
    }

    #region IState Methods
    public virtual void Enter()
    {
        Debug.Log("Current State: " + GetType().Name);

        AddInputActionsCallbacks();
    }

    public virtual void Exit()
    {
        RemoveInputActionsCallbacks();
    }

    public virtual void HandleInput()
    {
        ReadMovementInput();
    }
    
    public virtual void Update()
    {
        
    }

    public virtual void PhysicsUpdate()
    {
        Move();
    }
    #endregion

    #region Main Methods
    private void ReadMovementInput()
    {
        movementInput = StateMachine.Player.Input.PlayerActions.Movement.ReadValue<Vector2>();
    }

    private void Move()
    {
        if (movementInput == Vector2.zero || speedModifier == 0f)
        {
            return;
        }

        Vector3 movementDirection = GetMovementInputDirection();

        float targetRotationYAngle = Rotate(movementDirection);
        Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);
        
        float moveSpeed = GetMoveSpeed();
        Vector3 currentPlayerHorizontalVelocity = GetPlayerHorizontalVelocity();
        
        StateMachine.Player.Rigidbody.AddForce(targetRotationDirection * moveSpeed - currentPlayerHorizontalVelocity, ForceMode.VelocityChange);
    }
    
    private float Rotate(Vector3 direction)
    {
        float directionAngle = UpdateTargetRotation(direction);
        RotateTowardsTargetRotation();

        return directionAngle;
    }
    
    private float GetDirectionAngle(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        
        if (angle < 0f)
        {
            angle += 360f;
        }

        return angle;
    }

    private float AddCameraRotationToAngle(float angle)
    {
        angle += StateMachine.Player.MainCameraTransform.eulerAngles.y;

        if (angle > 360f)
        {
            angle -= 360f;
        }

        return angle;
    }
    
    private void UpdateTargetRotationData(float targetAngle)
    {
        currentTargetRotation.y = targetAngle;
        dampedTargetRotationPassedTime.y = 0f;
    }
    #endregion

    #region Reusable Methods
    protected Vector3 GetMovementInputDirection()
    {
        return new Vector3(movementInput.x, 0f, movementInput.y);
    }
    
    protected float GetMoveSpeed()
    {
        return baseSpeed * speedModifier;
    }
    
    protected Vector3 GetPlayerHorizontalVelocity()
    {
        Vector3 playerHorizontalVelocity = StateMachine.Player.Rigidbody.velocity;
        playerHorizontalVelocity.y = 0f;
        return playerHorizontalVelocity;
    }

    protected void RotateTowardsTargetRotation()
    {
        float currentYAngle = StateMachine.Player.Rigidbody.rotation.eulerAngles.y;
        if (currentYAngle == currentTargetRotation.y)
        {
            return;
        }

        float smoothYAngle = Mathf.SmoothDampAngle(currentYAngle, currentTargetRotation.y,
            ref dampedTargetRotationCurrentVelocity.y,
            timeToReachTargetRotation.y - dampedTargetRotationPassedTime.y);

        dampedTargetRotationPassedTime.y += Time.deltaTime;
        
        Quaternion targetRotation = Quaternion.Euler(0f, smoothYAngle, 0f);
        StateMachine.Player.Rigidbody.MoveRotation(targetRotation);
    }
    
    protected float UpdateTargetRotation(Vector3 direction, bool shouldConsiderCameraRotation = true)
    {
        float directionAngle = GetDirectionAngle(direction);
        
        if (shouldConsiderCameraRotation)
        {
            directionAngle = AddCameraRotationToAngle(directionAngle);
        }

        if (directionAngle != currentTargetRotation.y)
        {
            UpdateTargetRotationData(directionAngle);
        }

        return directionAngle;
    }
    
    protected Vector3 GetTargetRotationDirection(float targetAngle)
    {
        return Quaternion.Euler(0F, targetAngle, 0f) * Vector3.forward;
    }

    protected void ResetVelocity()
    {
        StateMachine.Player.Rigidbody.velocity = Vector3.zero;
    }    
    
    protected virtual void AddInputActionsCallbacks()
    {
        StateMachine.Player.Input.PlayerActions.WalkToggle.started += OnWalkToggleStarted;
    }
    
    protected virtual void RemoveInputActionsCallbacks()
    {
        StateMachine.Player.Input.PlayerActions.WalkToggle.started -= OnWalkToggleStarted;
    }
    #endregion
    
    #region InputMethods
    protected virtual void OnWalkToggleStarted(InputAction.CallbackContext context)
    {
        shouldWalk = !shouldWalk;
    }
    #endregion
}

