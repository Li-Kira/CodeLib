using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoginInput : MonoBehaviour
{
    private EventSystem m_EventSystem;

    public Selectable firstInput;
    public Button submitButton;

    private void Start()
    {
        m_EventSystem = EventSystem.current;
        firstInput.Select();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
        {
            Selectable previous = m_EventSystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
            if (previous != null)
            {
                previous.Select();                
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = m_EventSystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null)
            {
                next.Select();                
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            submitButton.onClick.Invoke();
            Debug.Log("Button Pressed");
        }
    }
}
