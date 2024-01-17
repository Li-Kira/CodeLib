using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] [Range(0f, 10f)] private float defaultDistance = 6f;
    [SerializeField] [Range(0f, 10f)] private float minDistance = 1f;
    [SerializeField] [Range(0f, 10f)] private float maxDistance = 6f;
    
    [SerializeField] [Range(0f, 10f)] private float smoothing = 5;
    [SerializeField] [Range(0f, 10f)] private float zoomSensitivity = 5;

    private CinemachineFramingTransposer FramingTransposer;
    private CinemachineInputProvider InputProvider;

    private float currentTargetDistance;
    
    private void Awake()
    {
        FramingTransposer = GetComponent<CinemachineVirtualCamera>()
            .GetCinemachineComponent<CinemachineFramingTransposer>();
        InputProvider = GetComponent<CinemachineInputProvider>();

        currentTargetDistance = defaultDistance;
    }

    private void Update()
    {
        Zoom();
    }

    private void Zoom()
    {
        float zoomValue = InputProvider.GetAxisValue(2) * zoomSensitivity;
        currentTargetDistance = Mathf.Clamp(currentTargetDistance + zoomValue, minDistance, maxDistance);

        float currentDistance = FramingTransposer.m_CameraDistance;
        if (currentDistance == currentTargetDistance)
        {
            return;
        }
        float lerpedZoomValue = Mathf.Lerp(currentDistance, currentTargetDistance, smoothing * Time.deltaTime);
        FramingTransposer.m_CameraDistance = lerpedZoomValue;
    }
}
