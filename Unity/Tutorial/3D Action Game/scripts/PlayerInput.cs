using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float HorizontalInput;
    public float VerticalInput;

    public bool MouseButtonDown;
    public bool SpaceButtonDown;
    
    
    void Update()
    {
        //在之后将Time.deltaTime设置为0，因为不希望暂停菜单激活的时候仍然可以更新输入数据
        if (!MouseButtonDown && Time.deltaTime != 0)
        {
            //0代表鼠标左键被按下
            MouseButtonDown = Input.GetMouseButtonDown(0);
        }

        if (!SpaceButtonDown && Time.timeScale != 0)
        {
            SpaceButtonDown = Input.GetKeyDown(KeyCode.Space);
        }
        
        
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");
        
    }


    private void OnDisable()
    {
        ClearCache();
    }

    public void ClearCache()
    {
        MouseButtonDown = false;
        SpaceButtonDown = false;
        HorizontalInput = 0;
        VerticalInput = 0;
    }
    
}
