using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIJump : MonoBehaviour
{
    public void StartJumping()
    {
        transform.LeanMoveLocal(new Vector2(200, 100), 1).setEaseOutQuart().setLoopPingPong();
    }
}
