using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour, ICastable
{
    [Header("Damage Info")] 
    public float damage;
    
    [Header("Launch")]
    public float launchSpeed = 3f;
    public float lifetime = 5f;
    [Tooltip("If true, the projectile will run its OnImpact code when the lifetime expires as if it hit something. If false, it will simply destroy itself")]
    public bool impactAfterLifetime = true;
    public ForceMode launchMode = ForceMode.Impulse;
    
    [Header("Effects")]
    public GameObject impactVfx;
    // public AudioClip impactSfx;

    protected Rigidbody rb;
    protected AudioSource audioSource;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (!TryGetComponent(out audioSource))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0.75f;
        }
        
    }

    /// <summary>
    /// Launches the Projectile in the specified direction.
    /// Change launchSpeed to increase the speed of the projectile.
    /// </summary>
    /// <param name="castDirection">The direction in which the spell is cast</param>
    public virtual void Cast(Vector3 castDirection, Transform casterTransform)
    {
        rb.AddForce(castDirection.normalized * launchSpeed, launchMode);
        
        //Destroy(gameObject, lifetime);
        StartCoroutine(SelfDestruct());
    }

    /// <summary>
    /// Causes the Projectile to destroy itself when its lifetime ends
    /// </summary>
    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(lifetime);
        
        if (impactAfterLifetime) DoImpactEffects();
        else Destroy(gameObject);
    }
    
    protected void OnCollisionEnter(Collision collision)
    {
        OnImpact(collision);
        
        DoImpactEffects();
    }

    private void DoImpactEffects()
    {
        if (impactVfx)
        {
            var spawnTransform = transform;
            
            var vfx = Instantiate(impactVfx, spawnTransform.position, spawnTransform.rotation);
        }
        
        // if (impactSfx)
        // {
        //     audioSource.PlayOneShot(impactSfx);
        //     StartCoroutine(DestroyAfterSfx());
        // }
        // else
        // {
        Destroy(gameObject);
        // }
    }

    /// <summary>
    /// Called after a Collision, before doing any visual or audio effects and before destruction.
    /// Implement this to handle damage or debuffs.
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnImpact(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Damageable damageable)) damageable.ApplyDamage(damage);
    }

    /// <summary>
    /// Destroys this GameObject after any playing sound effects finish.
    /// In the meantime, it will be "soft" destroyed, with rendering and collisions disabled.
    /// </summary>
    protected IEnumerator DestroyAfterSfx()
    {
        rb.isKinematic = true;
        rb.detectCollisions = false;
        GetComponentInChildren<Renderer>().enabled = false;
        enabled = false;
        
        while (audioSource.isPlaying) yield return null;
        
        Destroy(gameObject);
    }
}
