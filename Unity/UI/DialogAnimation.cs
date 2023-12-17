using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogAnimation : MonoBehaviour
{
    public CanvasGroup Background;
    public Transform DialogTransform;
   
    
    private void OnEnable()
    {
        Background.alpha = 0;
        Background.LeanAlpha(1, 0.5f);

        DialogTransform.localPosition = new Vector2(0, -Screen.height);
        DialogTransform.LeanMoveLocalY(0, 0.5f).setEaseOutExpo().delay = 0.1f;
    }

    public void CloseDialog()
    {
        Background.LeanAlpha(0, 0.5f);
        DialogTransform.LeanMoveLocalY(-Screen.height, 0.5f).setEaseInExpo().setOnComplete(OnCompelete);
    }

    private void OnCompelete()
    {
        gameObject.SetActive(false);
    }
}
