using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicShield : MonoBehaviour, ICastable
{
    [Header("Shield Info")]
    public float shieldDuration = 1.5f;
    public Vector3 shieldOffset = new Vector3(-1.57f, 0, 1.6f);

    private Collider col;
    private SpellHandler playerSpellHandler;

    private void Start()
    {
        col = GetComponent<Collider>();
    }

    public void Cast(Vector3 castDirection, Transform casterTransform)
    {
        playerSpellHandler = casterTransform.GetComponentInParent<SpellHandler>();
        transform.SetParent(casterTransform);
        transform.SetLocalPositionAndRotation(shieldOffset, Quaternion.identity);

        StartCoroutine(ShieldLifetime());
    }

    private IEnumerator ShieldLifetime()
    {
        playerSpellHandler.StartCoroutine(playerSpellHandler.DisableCastingForTime(shieldDuration));

        yield return new WaitForSeconds(shieldDuration);

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Projectile projectile))
        {
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            // Prevents any more collisions between the projectile and this shield.
            // Useful if the projectile passed all the way and would return through the shield 
            Physics.IgnoreCollision(col, other);

            Vector3 oldVelocity = rb.velocity;
            Vector3 newVelocity = new Vector3(-oldVelocity.x, oldVelocity.y, -oldVelocity.z);
            rb.velocity = newVelocity;
        }
        else
        {
            Debug.LogWarning("Shield hit by something other than a projectile", other);
        }
    }
}
