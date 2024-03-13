using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicWeapon : MonoBehaviour, ICastable
{
    private const float MeleeAnimationTime = 0.75f;

    public float damage = 3f;
    public Vector3 startPositionOffset;
    public Quaternion startRotation;

    public AudioClip weaponAttackSfx;

    private SpellHandler _playerSpellHandler;
    private AudioSource _audioSource;
    private Animator _animator;
    private static readonly int AnimatorMeleeAttack = Animator.StringToHash("MeleeAttack");

    private List<GameObject> _hitObjects;

    public void Cast(Vector3 castDirection, Transform casterTransform)
    {
        _playerSpellHandler = casterTransform.GetComponentInParent<SpellHandler>();
        transform.SetParent(casterTransform);
        transform.SetLocalPositionAndRotation(startPositionOffset, startRotation);

        _audioSource = GetComponent<AudioSource>();
        _animator = _playerSpellHandler.GetComponent<Animator>();

        _hitObjects = new List<GameObject>();
        
        StartCoroutine(DoMeleeAttack());
    }

    private IEnumerator DoMeleeAttack()
    {
        _animator.SetTrigger(AnimatorMeleeAttack);
        _playerSpellHandler.StartCoroutine(_playerSpellHandler.DisableCastingForTime(MeleeAnimationTime));

        yield return new WaitForSeconds(MeleeAnimationTime / 3);

        _audioSource.PlayOneShot(weaponAttackSfx);
        transform.SetLocalPositionAndRotation(startPositionOffset, startRotation);

        yield return new WaitForSeconds(MeleeAnimationTime * 2f / 3f);

        Destroy(gameObject);
    }

    // Have to user OnCollisionEnter instead of OnTriggerEnter due to potentially hitting compound colliders
    //  NOTE
    //  collision.collider = physical collider (child collider in compound)
    //  collision.gameObject = rigidbody object (parent in compound)
    // OnTriggerEnter only provides the hit collider, not the hit gameObject
    private void OnCollisionEnter(Collision collision)
    {
        // Prevent the melee attack from hitting the same object more than once
        if (_hitObjects.Contains(collision.gameObject)) return;
        
        // print("Melee hit " + collision.collider + " (" + collision.gameObject.name + ")");
        if (collision.gameObject.TryGetComponent(out Damageable damageable))
        {
            if (damageable is PlayerHealth) return;
            
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            damageable.ApplyDamage(damage);
            
            // add the object to the list of hit objects
            _hitObjects.Add(collision.gameObject);
        }
    }
}