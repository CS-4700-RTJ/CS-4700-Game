using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(EnemyBehavior))]
public class Enemy : Damageable
{
    [Header("Scoring")] 
    public int pointValue = 0;

    protected bool isFrozen;
    protected bool isPoisoned;

    private Renderer _renderer;
    private Color _originalColor;

    private Animator _animator;
    private Rigidbody _rigidbody;
    private EnemyBehavior _behavior;
    
    private static readonly int AnimatorDeathTrigger = Animator.StringToHash("Death");
    private static readonly int AnimatorDamageTrigger = Animator.StringToHash("Damage");
    private static readonly int AnimatorFreezeTrigger = Animator.StringToHash("Freeze");
    
    protected override void Start()
    {
        base.Start();

        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _renderer = GetComponent<Renderer>();
        _behavior = GetComponent<EnemyBehavior>();
        
        _originalColor = _renderer.material.color;

        EventManager.OnPlayerDeath += _behavior.OnDeath;
    }

    public override void ApplyDamage(float amount)
    {
        base.ApplyDamage(amount);
        
        _behavior.OnDamage();
        _animator.SetTrigger(AnimatorDamageTrigger);
        StartCoroutine(FlashColor(_renderer, Color.red));
    }

    protected override void Death()
    {
        PlayDeathSound();
        
        // Give the player points
        GameManager.IncreaseScore(pointValue);
        
        // Replace with death animation
        _animator.SetTrigger(AnimatorDeathTrigger);
        PlayDeathSound();

        enabled = false;
        StopAllCoroutines();

        _rigidbody.detectCollisions = false;
        _renderer.material.color = _originalColor;

        EventManager.OnPlayerDeath -= _behavior.OnDeath;
        
        _behavior.OnDeath();
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
        _animator.SetTrigger(AnimatorFreezeTrigger);
        _behavior.OnFreeze(freezeDuration);
        StartCoroutine(HandleFreeze(freezeDuration, freezeColor));
    }

    private IEnumerator HandleFreeze(float freezeDuration, Color freezeColor)
    {
        isFrozen = true;
        _animator.enabled = false;
        print("Disabling animator");
        
        if (!isPoisoned) _renderer.material.color = freezeColor;
        
        yield return new WaitForSeconds(freezeDuration);

        print(_animator.enabled);
        
        if (!isPoisoned) _renderer.material.color = _originalColor;
        
        isFrozen = false;
        _animator.enabled = true;
    }
}