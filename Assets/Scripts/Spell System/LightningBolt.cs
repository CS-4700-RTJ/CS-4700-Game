using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBolt : Projectile
{
    [Header("Lightning Info")] 
    [Tooltip("What is the maximum distance that the lightning can chain between enemies?")]
    public float lightningChainDistance = 1f;
    [Tooltip("What layers can the lightning chain to?")]
    public LayerMask lightningChainLayerMask;
    [Tooltip("What is the maximum number of enemies that the lighting can chain to?")]
    public int maxAdditionalEnemies = 2;

    private void FixedUpdate()
    {
        // Rotate object with velocity so that it always points in the direction it is moving
        transform.rotation = Quaternion.LookRotation(rb.velocity);
    }
    
    protected override void OnImpact(Collision collision)
    {
        base.OnImpact(collision);

        // Get all potential objects that the lightning can chain to
        List<Collider> hitColliders = new List<Collider>(Physics.OverlapSphere(transform.position, lightningChainDistance * maxAdditionalEnemies, lightningChainLayerMask));
        
        // Remove the initially hit collider, if it is in the list
        if (hitColliders.Contains(collision.collider)) hitColliders.Remove(collision.collider);
        
        // Now determine which ones get hit
        
        var chainedObjects = new List<Collider>();
        var startingTransform = collision.collider.transform;

        while (chainedObjects.Count < maxAdditionalEnemies)
        {
            // Get the next chained enemy
            var nextCollider = GetNextEnemyInChain(startingTransform, hitColliders);

            if (nextCollider)
            {
                // Add the next chained enemy to the list
                chainedObjects.Add(nextCollider);
                startingTransform = nextCollider.transform;

                hitColliders.Remove(nextCollider);
            } 
            else break;
        }
        
        // Now try to damage each hit object
        foreach (var chainedObject in chainedObjects)
        {
            if (chainedObject.TryGetComponent(out Damageable damageable))
            {
                // Damage the object and play the VFX
                damageable.ApplyDamage(damage);
                Instantiate(impactVfx, damageable.transform);
            }
        }
    }

    private Collider GetNextEnemyInChain(Transform startTransform, List<Collider> hitColliders)
    {
        // Find the closest enemy and add it to the chained list
        Collider closestObject = null;
        float closestDistance = -1f;
        foreach (var hitCol in hitColliders)
        {
            if (closestObject == null)
            {
                closestObject = hitCol;
                
                // Subtract the extents of the bounding box from the distance so that
                // it is the distance from the edge of the collider to the start transform
                closestDistance = Vector3.Distance(closestObject.bounds.center, startTransform.position) - closestObject.bounds.extents.magnitude;
            }
            else
            {
                float distance = Vector3.Distance(hitCol.bounds.center, startTransform.position) - closestObject.bounds.extents.magnitude;
                
                if (distance < closestDistance)
                {
                    closestObject = hitCol;
                    closestDistance = distance;
                }
            }
        }
        
        // Now we have the closest collider
        if (closestDistance <= lightningChainDistance) return closestObject;
        else return null;
    }
}
