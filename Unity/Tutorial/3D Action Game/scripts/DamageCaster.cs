using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCaster : MonoBehaviour
{
    private Collider _damageCasterCollider;

    public int damage = 30;
    public string TargetTag;

    private List<Collider> DamageTargetList;

    private void Awake()
    {
        _damageCasterCollider = GetComponent<Collider>();
        _damageCasterCollider.enabled = false;
        DamageTargetList = new List<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == TargetTag && !DamageTargetList.Contains(other))
        {
            Character _character = other.GetComponent<Character>();
            if (_character != null)
            {
                _character.ApplyDamage(damage,transform.parent.position);
                
                PlayerVFXManager playerVFXManager = transform.parent.GetComponent<PlayerVFXManager>();
                
                if (playerVFXManager != null)
                {
                    RaycastHit hit;

                    Vector3 originPos = transform.position + (-_damageCasterCollider.bounds.extents.z) * transform.forward;

                    bool isHit = Physics.BoxCast(originPos, _damageCasterCollider.bounds.extents / 2, transform.forward, out hit,
                        transform.rotation, _damageCasterCollider.bounds.extents.z, 1 << 6);

                    if (isHit)
                    {
                        playerVFXManager.PlaySlash(hit.point + new Vector3(0, 0.5f, 0));
                    }
                }
            }
            
            DamageTargetList.Add(other);
            
        }
        
    }

    public void EnableDamageCaster()
    {
        DamageTargetList.Clear();
        _damageCasterCollider.enabled = true;
    }


    public void DisableDamageCaster()
    {
        DamageTargetList.Clear();
        _damageCasterCollider.enabled = false;
    }
    
    private void OnDrawGizmos()
    {
        //OnDrawGizmos运行在游戏模式和编辑模式中，在编辑器模式中，由于游戏未运行，Awake()将不会被执行，因此我们需要重新获取引用
        if (_damageCasterCollider == null)
        {
            _damageCasterCollider = GetComponent<Collider>();
        }

        RaycastHit hit;

        Vector3 originPos = transform.position + (-_damageCasterCollider.bounds.extents.z) * transform.forward;

        bool isHit = Physics.BoxCast(originPos, _damageCasterCollider.bounds.extents / 2, transform.forward, out hit,
            transform.rotation, _damageCasterCollider.bounds.extents.z, 1 << 6);
        
        //1 << 6 是特定层的值，在这里是Enemy层

        if (isHit)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(hit.point, 0.3f);
        }
        
        
    }
    
    
}
