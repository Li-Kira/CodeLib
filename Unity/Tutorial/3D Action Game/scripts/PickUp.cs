using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public enum PickUpType
    {
        Heal,Coin
    }

    //Heal
    public PickUpType Type;
    public int Value = 20;

    //Coin
    public ParticleSystem CollectedVFX;
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<Character>().PickUpItem(this);

            if (CollectedVFX != null)
            {
                Instantiate(CollectedVFX, transform.position, Quaternion.identity);
            }
            
            Destroy(gameObject);
            
        } 
    }
}
