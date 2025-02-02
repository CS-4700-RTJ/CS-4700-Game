using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoisonDart : Projectile
{
    // mana cost - 20
    [Header("Poison Info")] 
    public float damagePerSecond = 1f;
    public float poisonDuration = 10f;
    public float poisonFlashFrequncy = 2f;
    public Color poisonFlashColor = Color.green;
    
    protected override void OnImpact(Collision collision)
    {
        if (collision.transform.TryGetComponent(out Damageable damageable))
        {
            damageable.ApplyDamage(damage);

            if (damageable is Enemy) damageable.StartCoroutine(PoisonDamageable(damageable));
            else if (damageable is PlayerHealth) damageable.StartCoroutine(PoisonDamageable(damageable));
        }
    }

    // Applies DoT poison damage to the damageable
    private IEnumerator PoisonDamageable(Damageable damageable)
    {
        float timer = 0f;
        int numFlashes = (int)(poisonDuration / poisonFlashFrequncy);

        if (damageable is Enemy enemy) enemy.HandlePoisonFlashing(numFlashes, poisonFlashFrequncy, poisonFlashColor);
        else damageable.StartCoroutine(Damageable.FlashColor(damageable.GetComponent<Renderer>(), poisonFlashColor, numFlashes, poisonFlashFrequncy));
        
        while (timer < poisonDuration)
        {
            // Apply damage based on the DPS
            damageable.ApplyDamageWithoutEffects((damagePerSecond * Time.deltaTime));

            // Increment the timer
            timer += Time.deltaTime;

            yield return null;
        }
    }
}