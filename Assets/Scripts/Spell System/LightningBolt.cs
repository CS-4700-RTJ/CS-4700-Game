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
        List<GameObject> hitObjects = new List<GameObject>();
        
        // Remove the initially hit collider, if it is in the list
        if (hitColliders.Contains(collision.collider)) hitColliders.Remove(collision.collider);
        
        // Now determine which ones get hit
        foreach (var coll in hitColliders)
        {
            if (coll.attachedRigidbody)
            {
                GameObject hitObject = coll.attachedRigidbody.gameObject;
                
                if (!hitObjects.Contains(hitObject)) hitObjects.Add(hitObject);
            }
        }

        // Remove the initially hit game object, if its in the list
        if (hitObjects.Count > 0 && hitObjects.Contains(collision.gameObject)) hitObjects.Remove(collision.gameObject);
        
        if (hitObjects.Count == 0) return;

        // var chainedObjects = new List<Collider>();
        var chainedObjs = new List<GameObject>();
        var startingTransform = hitObjects[0].transform;

        while (chainedObjs.Count < maxAdditionalEnemies)
        {
            // Get the next chained enemy
            // var nextCollider = GetNextEnemyInChain(startingTransform, hitColliders);
            var nextObject = GetNextEnemyInChain(startingTransform, hitObjects);

            if (nextObject)
            {
                // Add the next chained enemy to the list
                chainedObjs.Add(nextObject);
                startingTransform = nextObject.transform;
                hitObjects.Remove(nextObject);
            } 
            else break;
        }

        foreach (var chainedObject in chainedObjs)
        {
            if (chainedObject.TryGetComponent(out Damageable damageable))
            {
                // Damage the object and play the VFX
                damageable.ApplyDamage(damage);
                Instantiate(impactVfx, damageable.transform);
            }
        }
    }

    private GameObject GetNextEnemyInChain(Transform startTransform, List<GameObject> hitObjects)
    {
        // Find the closest enemy and add it to the chained list
        GameObject closestObject = null;
        float closestDistance = -1f;

        foreach (var hitObj in hitObjects)
        {
            if (closestObject == null)
            {
                closestObject = hitObj;

                closestDistance = Vector3.Distance(closestObject.transform.position, startTransform.position);
            }
            else
            {
                float distance = Vector3.Distance(hitObj.transform.position, startTransform.position);

                if (distance < closestDistance)
                {
                    closestObject = hitObj;
                    closestDistance = distance;
                }
            }

        }

        if (closestDistance <= lightningChainDistance) return closestObject;
        else return null;
    }
}
