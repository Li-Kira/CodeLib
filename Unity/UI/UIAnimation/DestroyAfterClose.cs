using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterClose : MonoBehaviour
{
    public void OnClose()
    {
        Destroy(gameObject);
    }
}
