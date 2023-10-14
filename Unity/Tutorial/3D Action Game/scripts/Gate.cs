using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public GameObject _GameObject;
    private Collider _collider;

    private float OpenDuration = 2f;
    private float OpenTarget_Y = -2f;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }


    IEnumerator OpenGateAnimation()
    {
        float currentDuration = 0;
        Vector3 startPos = _GameObject.transform.position;
        Vector3 targetPos = startPos + Vector3.up * OpenTarget_Y;

        while (currentDuration <= OpenDuration)
        {
            currentDuration += Time.deltaTime;
            _GameObject.transform.position = Vector3.Lerp(startPos,targetPos,currentDuration / OpenDuration);
            yield return null;
        }

        _collider.enabled = false;
    }

    public void OpenGate()
    {
        StartCoroutine(OpenGateAnimation());
    }
    
    
}
