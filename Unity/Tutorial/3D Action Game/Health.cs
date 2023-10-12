using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int MaxHealth;
    public int CurrentHealth;

    private Character _character;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
        _character = GetComponent<Character>();
    }


    public void ApplyDamage(int damage)
    {
        CurrentHealth -= damage;
        ChackHealth();
    }

    private void ChackHealth()
    {
        if (CurrentHealth <= 0)
        {
            _character.SwitchStateTo(Character.CharacterState.Dead);
        }
        
    }

    public void AddHealth(int heal)
    {
        CurrentHealth += heal;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
    }
    
}
