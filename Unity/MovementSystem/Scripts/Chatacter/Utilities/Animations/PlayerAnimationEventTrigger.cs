using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEventTrigger : MonoBehaviour
{
    private Player m_Player;

    private void Awake()
    {
        m_Player = transform.parent.GetComponent<Player>();
    }

    public void TriggerOnMovementStateAnimationEnterEvent()
    {
        if (IsInAnimationTransition())
        {
            return;
        }
        
        m_Player.OnMovementStateAnimationEnterEvent();
    }
    
    public void TriggerOnMovementStateAnimationExitEvent()
    {
        if (IsInAnimationTransition())
        {
            return;
        }
        
        m_Player.OnMovementStateAnimationExitEvent();
    }
    
    public void TriggerOnMovementStateAnimationTransitionrEvent()
    {
        if (IsInAnimationTransition())
        {
            return;
        }
        
        m_Player.OnMovementStateAnimationTransitionEvent();
    }

    private bool IsInAnimationTransition(int layerIndex = 0)
    {
        return m_Player.Animator.IsInTransition(layerIndex);
    }
}
