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

        bool includedHitObject = false;
        
        // print("fireball hit " + collision.gameObject.name);
        
        foreach (var hitObject in hitObjects)
        {
            Debug.Log(hitObject, hitObject);
            if (hitObject.gameObject.Equals(collision.gameObject)) includedHitObject = true;
            
            if (hitObject.TryGetComponent(out Damageable damageable)) damageable.ApplyDamage(damage);
        }

        if (!includedHitObject)
        {
            if (collision.gameObject.TryGetComponent(out Damageable damageable)) damageable.ApplyDamage(damage);
        }
    }
}