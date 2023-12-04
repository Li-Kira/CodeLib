using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetSystem : MonoBehaviour
{
    public static TargetSystem instance;
    private Camera m_Camera;
    
    [Header("Setting")]
    public float maxVisibleDistance = 15f;
    
    [Header("Target")]
    public TargetObject currentTarget;
    public List<TargetObject> TargetList;
     
    [Header("UI")]
    public Image TargetPointUI;
    private Vector3 m_TargetScale;

    [Header("TargetMode")] 
    public bool isLock = false;
    public TargetMode currentTargetMode = TargetMode.FreeMode;
    public enum TargetMode
    {
        FreeMode, SelectionMode  
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        m_Camera = Camera.main;
    }

    private void Start()
    {
        m_TargetScale = TargetPointUI.rectTransform.localScale;
        
        if (currentTargetMode == TargetMode.SelectionMode)
        {
            InitiateTarget();
        }
    }

    private void Update()
    {
        switch (currentTargetMode)
        {
            case TargetMode.FreeMode:
                ClearTargetList();
                AddTarget();
                ChangeCurrentTarget();
                SetUI();
                break;
            case TargetMode.SelectionMode:
                SetUI();
                break;
        }
        
    }

    /// <summary>
    /// Free Mode
    /// </summary>
    
    private void ClearTargetList()
    {
        TargetList.Clear();
    }

    private void AddTarget()
    {
        TargetObject[] allTargetObjects = FindObjectsOfType<TargetObject>();
        
        foreach (var target in allTargetObjects)
        {
            if (target.isVisible)
            {
                if (!TargetList.Contains(target))
                {
                    TargetList.Add(target);
                }
            }
        }
    }

    private void ChangeCurrentTarget()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        float minDistance = float.MaxValue;

        TargetObject closestTargetObject = null;
        
        if (TargetList.Count > 0)
        {
            foreach (var target in TargetList)
            {
                // 将 Target 的世界坐标转换为屏幕坐标
                Vector3 targetScreenPos = m_Camera.WorldToScreenPoint(target.position);
                float distanceToScreenCenter = Vector3.Distance(screenCenter, targetScreenPos);

                if (distanceToScreenCenter < minDistance)
                {
                    closestTargetObject = target;
                    minDistance = distanceToScreenCenter;
                }
            }
            
        }
        
        if (closestTargetObject != currentTarget)
        {
            StartCoroutine(ScaleAnimation());
        }

        currentTarget = closestTargetObject;
    }

    private void SetUI()
    {
        if (currentTarget != null)
        {
            TargetPointUI.rectTransform.position = ClampedScreenPosition(currentTarget.position);
            TargetPointUI.gameObject.SetActive(true);
        }
        else
        {
            TargetPointUI.gameObject.SetActive(false);
        }

        
        float distanceToCamera = Vector3.Distance(currentTarget.position, m_Camera.transform.position);
        if (distanceToCamera > maxVisibleDistance)
        {
            TargetPointUI.gameObject.SetActive(false);
        }
    }
    
    Vector3 ClampedScreenPosition(Vector3 targetPos)
    {
        Vector3 WorldToScreenPos = Camera.main.WorldToScreenPoint(targetPos);
        Vector3 clampedPosition = new Vector3(Mathf.Clamp(WorldToScreenPos.x, 0, Screen.width), Mathf.Clamp(WorldToScreenPos.y, 0, Screen.height), WorldToScreenPos.z);
        return clampedPosition;
    }


    /// <summary>
    /// Selection Mode 
    /// </summary>
    public void InitiateTarget()
    {
        ClearTargetList();
        AddTarget();
        ChangeCurrentTarget();
        SetUI();
    }


    /// <summary>
    /// Animation
    /// </summary>
    private IEnumerator ScaleAnimation()
    {
        float duration = 0.25f;
        float elapsed = 0.0f;

        Vector3 startScale = TargetPointUI.rectTransform.localScale + new Vector3(2.0f, 2.0f, 2.0f);
        Vector3 targetScale = m_TargetScale;
        
        while (elapsed < duration)
        {
            TargetPointUI.rectTransform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 确保最终大小为目标大小
        TargetPointUI.rectTransform.localScale = targetScale;
    }
    
    
}
