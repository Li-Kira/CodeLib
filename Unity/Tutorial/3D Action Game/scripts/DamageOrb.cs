using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOrb : MonoBehaviour
{
    public float speed = 2f;
    public int Damage = 10;
    public ParticleSystem HitVFX;
    private Rigidbody _Rigidbody;


    private void Awake()
    {
        _Rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _Rigidbody.MovePosition(transform.position + transform.forward * speed *Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Character _character = other.gameObject.GetComponent<Character>();

        if (_character != null && _character.isPlayer)
        {
            _character.ApplyDamage(Damage,transform.position);
            
        }

        Instantiate(HitVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);

        
    }
}
