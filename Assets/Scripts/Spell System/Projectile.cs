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
    public ForceMode launchMode = ForceMode.Impulse;
    
    [Header("Effects")]
    public GameObject impactVfx;
    public AudioClip impactSfx;

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
        
        Destroy(gameObject, lifetime);
    }

    protected void OnCollisionEnter(Collision collision)
    {
        OnImpact(collision);
        
        if (impactVfx)
        {
            var spawnTransform = transform;
            
            var vfx = Instantiate(impactVfx, spawnTransform.position, spawnTransform.rotation);
        }
        
        if (impactSfx) audioSource.PlayOneShot(impactSfx);

        StartCoroutine(DestroyAfterSfx());
    }

    /// <summary>
    /// Called after a Collision, before doing any visual or audio effects and before destruction.
    /// Implement this to handle damage or debuffs.
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnImpact(Collision collision)
    {
        
    }

    /// <summary>
    /// Destroys this GameObject after any playing sound effects finish.
    /// In the meantime, it will be "soft" destroyed, with rendering and collisions disabled.
    /// </summary>
    protected IEnumerator DestroyAfterSfx()
    {
        rb.isKinematic = true;
        rb.detectCollisions = false;
        GetComponent<Renderer>().enabled = false;
        enabled = false;
        
        while (audioSource.isPlaying) yield return null;
        
        Destroy(gameObject);
    }
}
