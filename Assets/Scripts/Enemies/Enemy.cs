using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    }

    protected override void Death()
    {
        PlayDeathSound();

        // Replace with death animation
        
        StartCoroutine(DestroyAfterSfx());
    }

    public void HandlePoisonFlashing(int numFlashes, float flashFrequency, Color poisonColor)
    {
        StartCoroutine(HandlePoisonColor(numFlashes, flashFrequency, poisonColor));
    }

    private IEnumerator HandlePoisonColor(int numFlashes, float flashFrequency, Color poisonColor)
    {
        isPoisoned = true;
        
        yield return StartCoroutine(FlashColor(renderer, poisonColor, numFlashes, flashFrequency));

        isPoisoned = false;

        if (!isFrozen) renderer.material.color = originalColor;
    }
    
    public void Freeze(float freezeDuration, Color freezeColor)
    {
        StartCoroutine(HandleFreeze(freezeDuration, freezeColor));
    }

    private IEnumerator HandleFreeze(float freezeDuration, Color freezeColor)
    {
        isFrozen = true;

        renderer.material.color = freezeColor;
        
        yield return new WaitForSeconds(freezeDuration);

        renderer.material.color = originalColor;
        
        isFrozen = false;
    }
}