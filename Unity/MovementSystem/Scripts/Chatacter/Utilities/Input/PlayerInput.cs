using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerInput : MonoBehaviour
{
    public PlayerInputAction InputAction { get; private set; }
    public PlayerInputAction.PlayerActions PlayerActions { get; private set; }

    private void Awake()
    {
        InputAction = new PlayerInputAction();
        PlayerActions = InputAction.Player;
    }

    // 如果不想使用玩家对象，那么不应该启用它
    private void OnEnable()
    {
        InputAction.Enable();
    }

    private void OnDisable()
    {
        InputAction.Disable();
    }
}

