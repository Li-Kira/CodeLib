using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerTriggerColliderData
{
    [field: SerializeField] public BoxCollider GroudCheckCollider { get; private set; }

    public Vector3 GroundCheckColliderExtents { get; private set; }

    public void Initialize()
    {
        GroundCheckColliderExtents = GroudCheckCollider.bounds.extents;
    }
}
