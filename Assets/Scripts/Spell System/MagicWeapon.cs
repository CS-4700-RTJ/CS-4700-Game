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

    public void Cast(Vector3 castDirection, Transform casterTransform)
    {
        _playerSpellHandler = casterTransform.GetComponentInParent<SpellHandler>();
        transform.SetParent(casterTransform);
        transform.SetLocalPositionAndRotation(startPositionOffset, startRotation);

        _audioSource = GetComponent<AudioSource>();
        _animator = _playerSpellHandler.GetComponent<Animator>();

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

    private void OnCollisionEnter(Collision collision)
    {
        print("Melee hit " + collision.collider);
        if (collision.collider.TryGetComponent(out Damageable damageable))
        {
            if (damageable is PlayerHealth) return;
            
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            damageable.ApplyDamage(damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Melee hit " + other);
        if (other.TryGetComponent(out Damageable damageable))
        {
            if (damageable is PlayerHealth) return;
         
            Physics.IgnoreCollision(other, GetComponent<Collider>());
            damageable.ApplyDamage(damage);
        }
    }
}