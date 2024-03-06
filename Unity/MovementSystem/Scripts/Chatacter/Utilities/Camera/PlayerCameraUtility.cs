using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[Serializable]
public class PlayerCameraUtility
{
    [field: SerializeField] public CinemachineVirtualCamera VirtualCamera { get; private set; }
    [field: SerializeField] public float DefaultHorizontalWaitTime { get; private set; } = 0f;
    [field: SerializeField] public float DefaultHorizontalRecenteringTime { get; private set; } = 4f;
    
    private CinemachinePOV m_CinemachinePov;

    public void Initialize()
    {
        m_CinemachinePov = VirtualCamera.GetCinemachineComponent<CinemachinePOV>();
    }

    public void EnableRecentering(float waitTime = -1f, float recenteringTime = -1f, float baseMovementSpeed = 1f, float movementSpeed = 1f)
    {
        m_CinemachinePov.m_HorizontalRecentering.m_enabled = true;
        m_CinemachinePov.m_HorizontalRecentering.CancelRecentering();

        if (waitTime == -1f)
        {
            waitTime = DefaultHorizontalWaitTime;
        }
        
        if (recenteringTime == -1f)
        {
            recenteringTime = DefaultHorizontalRecenteringTime;
        }

        recenteringTime = recenteringTime * baseMovementSpeed / movementSpeed;

        m_CinemachinePov.m_HorizontalRecentering.m_WaitTime = waitTime;
        m_CinemachinePov.m_HorizontalRecentering.m_RecenteringTime = recenteringTime;
    }

    public void DisableRecentering()
    {
        m_CinemachinePov.m_HorizontalRecentering.m_enabled = false;
    }
}
