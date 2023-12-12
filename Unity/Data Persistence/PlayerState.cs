using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerState
{
    public int Health;
    public string Name;
    public float BaseAttackSpeed;
    public int Level;
    public bool HasUnlockedSomething;
    public bool HasUnlockedSomethingElse;
    public EquipmentData Equipment;
}

[Serializable]
public class EquipmentData
{
    public EquipmentItem Head;
}

[Serializable]
public class EquipmentItem
{
    public int Defense;
    public int FireResist;
    public int PoisonResist;
    public int ElectricResist;
    public int WaterResist;
    public int EquippedSlot;
    public string Name;
    public int Rarity;
    public int Value;
    public int Quantity;
}