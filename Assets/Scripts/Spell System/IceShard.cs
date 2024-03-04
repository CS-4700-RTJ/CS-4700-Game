using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceShard : Projectile
{
    [Header("Freeze Info")] 
    public float freezeTime = 5f;
    public Color freezeColor = Color.blue;
    public GameObject icePrefab;
    public GameObject iceBreakVfx;

    private void FixedUpdate()
    {
        // Rotate object with velocity so that it always points in the direction it is moving
        transform.rotation = Quaternion.LookRotation(rb.velocity);
    }

    protected override void OnImpact(Collision collision)
    {
        base.OnImpact(collision);

        if (collision.transform.TryGetComponent(out Enemy enemy))
        {
            enemy.Freeze(freezeTime, freezeColor);
            
            // Create the Ice object and transfer all damage from the ice to the enemy
            Transform enemyTransform = enemy.transform;
            DamageTransfer iceTransfer = Instantiate(icePrefab, enemyTransform.position, enemyTransform.rotation, enemyTransform).GetComponentInChildren<DamageTransfer>();
            iceTransfer.target = enemy;

            iceTransfer.StartCoroutine(BreakIce(iceTransfer.transform.parent, freezeTime));
        }
    }

    private IEnumerator BreakIce(Transform iceTransform, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);

        Instantiate(iceBreakVfx, iceTransform.position, iceTransform.rotation);
        
        Destroy(iceTransform.gameObject);
    }
}
