using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    public Rigidbody Rigidbody { get; private set; }
    public PlayerInput Input { get; private set; }
    public Transform MainCameraTransform { get; private set; }
    
    private PlayerMovementStateMachine MovementStateMachine;
    private SkinnedMeshRenderer MeshRenderer;
    
    private void Awake()
    {
        MeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        
        Rigidbody = GetComponent<Rigidbody>();
        Input = GetComponent<PlayerInput>();
        MovementStateMachine = new PlayerMovementStateMachine(this);
        MainCameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        // Get Player Size
        // Debug.Log("MeshRenderer Size: " + MeshRenderer.bounds.size);
        
        MovementStateMachine.ChangeState(MovementStateMachine.IdlingState);
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
}

