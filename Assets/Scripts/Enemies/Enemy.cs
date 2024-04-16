using System.Collections;
using System.Collections.Generic;
using UnityEngine;
<<<<<<< HEAD
using UnityEngine.XR;

public class Enemy : Damageable
{
    protected bool isFrozen;
    protected bool isPoisoned;

    protected Renderer renderer;
    protected Color originalColor;

    protected override void Start()
    {
        base.Start();
        renderer = GetComponent<Renderer>();
        originalColor = renderer.material.color;
=======

[RequireComponent(typeof(Animator), typeof(Rigidbody))]
public class Enemy : Damageable
{
    [Header("Scoring")] 
    public int pointValue = 0;
    
    [Header("Death")]
    public Material dissolveMaterial;
    public GameObject dissolveVfx;

    protected bool isFrozen;
    protected bool isPoisoned;

    private Renderer _renderer;
    private Color _originalColor;

    private Animator _animator;
    private Rigidbody _rigidbody;
    private EnemyBehavior _behavior;
    private Coroutine _poisonFlashRoutine;
    
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
    }

    public override void ApplyDamage(float amount)
    {
        base.ApplyDamage(amount);
        
        _behavior.OnDamage();
        _animator.SetTrigger(AnimatorDamageTrigger);
        StartCoroutine(FlashColor(_renderer, Color.red));
>>>>>>> ee62877142338c7d80e7badf05f02c61c1e2a88c
    }

    protected override void Death()
    {
        PlayDeathSound();
<<<<<<< HEAD

        // Replace with death animation
        
        StartCoroutine(DestroyAfterSfx());
    }

    public void HandlePoisonFlashing(int numFlashes, float flashFrequency, Color poisonColor)
    {
        StartCoroutine(HandlePoisonColor(numFlashes, flashFrequency, poisonColor));
=======
        
        // Give the player points
        GameManager.IncreaseScore(pointValue);        
        // Replace with death animation
        
        // _animator.enabled = true;
        _animator.SetTrigger(AnimatorDeathTrigger);
        PlayDeathSound();

        enabled = false;
        if (_poisonFlashRoutine != null) StopCoroutine(_poisonFlashRoutine);

        _rigidbody.detectCollisions = false;
        _renderer.material.color = _originalColor;
        
        _behavior.OnDeath();
        
        if (dissolveMaterial && dissolveVfx) StartCoroutine(DissolveAndDestroy(4f));
    }
    
    /// <summary>
    /// Makes the enemy dissolve into nothing, and then destroys it.
    /// Provides a neat and clean way to remove bodies from the scene without them just dissappearing.
    /// </summary>
    private IEnumerator DissolveAndDestroy(float dissolveEffectTime) {
        var originalMaterial = _renderer.material;
        _renderer.material = dissolveMaterial;
        
        Instantiate(dissolveVfx, transform);
        
        // Dissolve effect
        AnimationCurve fadeIn = AnimationCurve.EaseInOut(0, 0, 1, 1);

        int shaderProperty = Shader.PropertyToID("_cutoff");
        
        float timer = 0;
        while (timer <= dissolveEffectTime)
        {
            timer += Time.deltaTime;
            
            _renderer.material.SetFloat(shaderProperty, fadeIn.Evaluate( Mathf.InverseLerp(0, dissolveEffectTime, timer)));

            yield return null;
        }
                
        _renderer.material = originalMaterial;
        Destroy(gameObject);
    }

    /// <summary>
    /// Makes the <b>Enemy</b> flash due to being poisoned.
    /// </summary>
    /// <param name="numFlashes">How many times should the <b>Enemy</b> flash?</param>
    /// <param name="flashFrequency">How often should each flash occur?</param>
    /// <param name="poisonColor">What color should the <b>Enemy</b> flash?</param>
    public void HandlePoisonFlashing(int numFlashes, float flashFrequency, Color poisonColor)
    {
        _poisonFlashRoutine = StartCoroutine(HandlePoisonColor(numFlashes, flashFrequency, poisonColor));
>>>>>>> ee62877142338c7d80e7badf05f02c61c1e2a88c
    }

    private IEnumerator HandlePoisonColor(int numFlashes, float flashFrequency, Color poisonColor)
    {
        isPoisoned = true;
        
<<<<<<< HEAD
        yield return StartCoroutine(FlashColor(renderer, poisonColor, numFlashes, flashFrequency));

        isPoisoned = false;

        if (!isFrozen) renderer.material.color = originalColor;
    }
    
    public void Freeze(float freezeDuration, Color freezeColor)
    {
        StartCoroutine(HandleFreeze(freezeDuration, freezeColor));
=======
        yield return StartCoroutine(FlashColor(_renderer, poisonColor, numFlashes, flashFrequency));

        isPoisoned = false;

        if (!isFrozen) _renderer.material.color = _originalColor;

        _poisonFlashRoutine = null;
    }
    
    /// <summary>
    /// Freezes the <b>Enemy</b> in place, stopping all actions and preventing movement.
    /// </summary>
    /// <param name="freezeDuration">How long should the <b>Enemy</b> be frozen?</param>
    /// <param name="freezeColor">What color should the <b>Enemy</b> be while frozen?</param>
    /// <returns>Was the <b>Enemy</b> already frozen before this?</returns>
    public bool Freeze(float freezeDuration, Color freezeColor)
    {
        bool startedFrozen = isFrozen;
        
        _animator.SetTrigger(AnimatorFreezeTrigger);
        _behavior.OnFreeze(freezeDuration);
        StartCoroutine(HandleFreeze(freezeDuration, freezeColor));

        return startedFrozen;
>>>>>>> ee62877142338c7d80e7badf05f02c61c1e2a88c
    }

    private IEnumerator HandleFreeze(float freezeDuration, Color freezeColor)
    {
        isFrozen = true;
<<<<<<< HEAD

        renderer.material.color = freezeColor;
        
        yield return new WaitForSeconds(freezeDuration);

        renderer.material.color = originalColor;
        
        isFrozen = false;
=======
        _animator.enabled = false;
        print("Disabling animator");
        
        if (!isPoisoned) _renderer.material.color = freezeColor;
        
        yield return new WaitForSeconds(freezeDuration);

        print(_animator.enabled);
        
        if (!isPoisoned) _renderer.material.color = _originalColor;
        
        isFrozen = false;
        _animator.enabled = true;
>>>>>>> ee62877142338c7d80e7badf05f02c61c1e2a88c
    }
}