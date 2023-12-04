using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObject : MonoBehaviour
{
    public Vector3 position;
    public bool isVisible = false;

    private Camera m_Camera;
    
    private void Awake()
    {
        m_Camera = Camera.main;
        
    }

    private void Update()
    {
        position = transform.position;
        isVisible = IsVisibleAndCloseEnough(m_Camera, TargetSystem.instance.maxVisibleDistance);
    }
    
    public bool IsVisibleAndCloseEnough(Camera camera, float maxDistance)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);

        if (!GeometryUtility.TestPlanesAABB(planes, GetComponent<Renderer>().bounds))
        {
            return false;
        }
        
        float distanceToCamera = Vector3.Distance(transform.position, camera.transform.position);
        return distanceToCamera <= maxDistance;
        
    }
    
}
