using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Custom/Characters/Player")]
public class PlayerSO : ScriptableObject
{ 
    [field: SerializeField] public PlayerGroundedData GroundedData { get; private set; }
}
