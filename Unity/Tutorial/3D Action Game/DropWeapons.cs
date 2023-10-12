using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropWeapons : MonoBehaviour
{
    public List<GameObject> Weapons;

    public void DropSwords()
    {
        foreach (var weapon in Weapons)
        {
            //为剑提供物理效果并且让他的父类不存在
            weapon.AddComponent<Rigidbody>();
            weapon.AddComponent<BoxCollider>();
            weapon.transform.parent = null;
        }
    }
    
}
