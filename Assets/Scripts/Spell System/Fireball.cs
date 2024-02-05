using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour, ICastable
{
    [Header("Explosion Info")]
    public float damage = 2f;
    public float explosionRadius = 2f;
    public LayerMask explosionLayerMask;
    
    [Header("Life")]
    public float moveSpeed = 3f;
    public float lifetime = 5f;
    
    [Header("Effects")]
    public GameObject explosionVfx;
    public AudioClip explosionSfx;

    private Vector3 moveDir;
    private Rigidbody rb;
    
    public void Cast(Vector3 castDirection)
    {
        moveDir = castDirection.normalized;
        Destroy(gameObject, lifetime);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.deltaTime));
    }

    private void OnCollisionEnter(Collision collision)
    {
        // TODO - sphere cast to get everything within explosion radius

        var explosion = Instantiate(explosionVfx);

        Transform explosionLocation = transform;
        explosion.transform.SetPositionAndRotation(explosionLocation.position, explosionLocation.rotation);

        if (explosionSfx) explosion.AddComponent<AudioSource>().PlayOneShot(explosionSfx);
        
        
        var hitObjects = Physics.OverlapSphere(explosionLocation.position, explosionRadius, explosionLayerMask);
       
        foreach (var hitObject in hitObjects)
        {
            if (hitObject.TryGetComponent(out Damageable damageable)) damageable.ApplyDamage(damage);
        }
        
        Destroy(gameObject);
    }
}