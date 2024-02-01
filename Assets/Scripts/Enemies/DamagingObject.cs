using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingObject : MonoBehaviour
{
    [SerializeField]
    private float damage = 1f;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Damageable hitObject))
        {
            hitObject.ApplyDamage(damage);
        }
    }
}
