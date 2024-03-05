using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fireball : Projectile
{
    [Header("Explosion Info")]
    public float explosionRadius = 2f;
    public LayerMask explosionLayerMask;

    protected override void OnImpact(Collision collision)
    {
        var hitObjects = Physics.OverlapSphere(transform.position, explosionRadius, explosionLayerMask);
       
        foreach (var hitObject in hitObjects)
        {
            if (hitObject.TryGetComponent(out Damageable damageable)) damageable.ApplyDamage(damage);
        }
    }
}