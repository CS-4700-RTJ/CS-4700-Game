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
<<<<<<< HEAD
       
        foreach (var hitObject in hitObjects)
        {
            if (hitObject.TryGetComponent(out Damageable damageable)) damageable.ApplyDamage(damage);
        }
=======

        bool includedHitObject = false;
        
        print("fireball hit " + collision.gameObject.name);
        
        foreach (var hitObject in hitObjects)
        {
            // Debug.Log(hitObject, hitObject);
            if (hitObject.gameObject.Equals(collision.gameObject)) includedHitObject = true;
            
            if (hitObject.TryGetComponent(out Damageable damageable)) damageable.ApplyDamage(damage);
        }

        if (!includedHitObject)
        {
            if (collision.gameObject.TryGetComponent(out Damageable damageable)) damageable.ApplyDamage(damage);
        }
>>>>>>> ee62877142338c7d80e7badf05f02c61c1e2a88c
    }
}