using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [field: Header("References")]
    [field: SerializeField] public PlayerSO Data { get; private set; }
    
    [field: Header("Collisions")]
    [field: SerializeField] public PlayerCapsuleColliderUtility ColliderUtility { get; private set; }
    
    [field: Header("Camera")]
    [field: SerializeField] public PlayerCameraUtility CameraUtility { get; private set; }

    [field: SerializeField] public PlayerLayerData PlayerLayerData { get; private set; }
    
    [field: Header("Animation")]
    [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; }

    public Rigidbody Rigidbody { get; private set; }
    public Animator Animator { get; private set; }
    public PlayerInput Input { get; private set; }
    public Transform MainCameraTransform { get; private set; }
    
    private PlayerMovementStateMachine MovementStateMachine;
    private SkinnedMeshRenderer MeshRenderer;
    
    private void Awake()
    {
        MeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        
        Rigidbody = GetComponent<Rigidbody>();
        Animator = GetComponentInChildren<Animator>();
        Input = GetComponent<PlayerInput>();
        MovementStateMachine = new PlayerMovementStateMachine(this);
        MainCameraTransform = Camera.main.transform;
        
        ColliderUtility.Initialize(gameObject);
        ColliderUtility.CalculateCapsuleColliderDimensions();
        
        CameraUtility.Initialize();
        
        AnimationData.Initialize();
    }
    
    // 进入播放模式才会执行
    private void OnValidate()
    {
        ColliderUtility.Initialize(gameObject);
        ColliderUtility.CalculateCapsuleColliderDimensions();
    }

    private void Start()
    {
        // Get Player Size
        // Debug.Log("MeshRenderer Size: " + MeshRenderer.bounds.size);
        
        MovementStateMachine.ChangeState(MovementStateMachine.IdlingState);
    }

    private void OnTriggerEnter(Collider collider)
    {
        MovementStateMachine.OnTriggerEnter(collider);
    }
    
    private void OnTriggerExit(Collider collider)
    {
        MovementStateMachine.OnTriggerExit(collider);
    }

    private void Update()
    {
        MovementStateMachine.HandleInput();
        MovementStateMachine.Update();
    }

    private void FixedUpdate()
    {
        MovementStateMachine.PhysicsUpdate();
    }

    public void OnMovementStateAnimationEnterEvent()
    {
        MovementStateMachine.OnAnimationEnterEvent();
    }
    
    public void OnMovementStateAnimationExitEvent()
    {
        MovementStateMachine.OnAnimationExitEvent();
    }
    
    public void OnMovementStateAnimationTransitionEvent()
    {
        MovementStateMachine.OnAnimationTransitionEvent();
    }
}

