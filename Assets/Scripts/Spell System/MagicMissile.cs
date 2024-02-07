using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissile : Projectile
{
    [Header("Homing Info")] 
    public float homingStrength = 1f;
    public LayerMask homingLayerMask;
    public int homingAngleCutoff = 90;
    
    private Transform target;
    
    public override void Cast(Vector3 castDirection, Transform casterTransform)
    {
        // base.Cast(castDirection, casterTransform);
        Destroy(gameObject, lifetime);

        var potentialTargets = Physics.OverlapSphere(transform.position, launchSpeed * lifetime, homingLayerMask);

        List<Transform> availableTargets = new List<Transform>();
        float sqrMinDistance = launchSpeed * launchSpeed * lifetime * lifetime; // faster operation later
        
        foreach (var potentialTarget in potentialTargets)
        {
            // See if target is in front of player
            Vector3 playerToTarget = potentialTarget.transform.position - transform.position;
            
            print("Player to target: " + playerToTarget);

            float angle = Vector3.Angle(playerToTarget, transform.forward);
            print(angle);

            if (!(angle <= homingAngleCutoff)) continue; // Skip targets outside the acceptable angle
            if (!(playerToTarget.sqrMagnitude < sqrMinDistance)) continue; // Skip targets farther than the current target
            
            // Set the potentialTarget as the target
            target = potentialTarget.transform;
            sqrMinDistance = playerToTarget.sqrMagnitude;
        }
    }
    
    protected override void OnImpact(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Damageable damageable)) damageable.ApplyDamage(damage);
    }

    private void FixedUpdate()
    {
        Vector3 toTarget = target.position - transform.position;
        //toTarget.y = 0;
        //toTarget.Normalize();
        
        //float rotateAmount = Vector3.Cross(toTarget, transform.up).z;
        
        //print(rotateAmount);

        //rb.angularVelocity = new Vector3(0, rotateAmount, 0);
        
        rb.AddForce((transform.forward + toTarget * homingStrength).normalized * launchSpeed, ForceMode.Acceleration);
    }
}
