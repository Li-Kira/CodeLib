using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_02_Shoot : MonoBehaviour
{
    public Transform ShootingPoint;
    public GameObject DamageOrb;

    public Character _Character;

    private void Awake()
    {
        _Character = GetComponent<Character>();
    }

    public void ShootTheDamageOrb()
    {
        Instantiate(DamageOrb, ShootingPoint.position, Quaternion.LookRotation(ShootingPoint.forward));
    }


    private void Update()
    {
        _Character.RotateToPlayer();
    }
}
