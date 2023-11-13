using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRotate : MonoBehaviour
{
    public float rotationSpeed = 10f;
    
    private void Update()
    {
        // 获取当前的欧拉角
        Vector3 currentEulerAngles = transform.eulerAngles;

        // 只更新Y轴的旋转
        currentEulerAngles.y += rotationSpeed * Time.deltaTime;

        // 应用旋转
        transform.eulerAngles = currentEulerAngles;
    }
}
