using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Enemy : Damageable
{
    protected bool isFrozen;
    protected bool isPoisoned;

    private Renderer _renderer;
    private Color _originalColor;

    private Animator _animator;
    private Rigidbody _rigidbody;
    
    private static readonly int AnimatorDeathTrigger = Animator.StringToHash("Death");
    private static readonly int AnimatorDamageTrigger = Animator.StringToHash("Damage");
    
    protected override void Start()
    {
        base.Start();

        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _renderer = GetComponent<Renderer>();
        _originalColor = _renderer.material.color;
    }

    public override void ApplyDamage(float amount)
    {
        base.ApplyDamage(amount);
        
        _animator.SetTrigger(AnimatorDamageTrigger);
    }

    protected override void Death()
    {
        PlayDeathSound();
        
        // Replace with death animation
        _animator.SetTrigger(AnimatorDeathTrigger);
        PlayDeathSound();

        enabled = false;
        StopAllCoroutines();

        _rigidbody.detectCollisions = false;
        _renderer.material.color = _originalColor;
    }

    public void HandlePoisonFlashing(int numFlashes, float flashFrequency, Color poisonColor)
    {
        StartCoroutine(HandlePoisonColor(numFlashes, flashFrequency, poisonColor));
    }

    private IEnumerator HandlePoisonColor(int numFlashes, float flashFrequency, Color poisonColor)
    {
        isPoisoned = true;
        
        yield return StartCoroutine(FlashColor(_renderer, poisonColor, numFlashes, flashFrequency));

        isPoisoned = false;

        if (!isFrozen) _renderer.material.color = _originalColor;
    }
    
    public void Freeze(float freezeDuration, Color freezeColor)
    {
        StartCoroutine(HandleFreeze(freezeDuration, freezeColor));
    }

    private IEnumerator HandleFreeze(float freezeDuration, Color freezeColor)
    {
        isFrozen = true;

        if (!isPoisoned) _renderer.material.color = freezeColor;
        
        yield return new WaitForSeconds(freezeDuration);

        if (!isPoisoned) _renderer.material.color = _originalColor;
        
        isFrozen = false;
    }
}