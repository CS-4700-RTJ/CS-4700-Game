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
<<<<<<< HEAD
        damageable.StartCoroutine(Damageable.FlashColor(damageable.GetComponent<Renderer>(), poisonFlashColor, numFlashes, poisonFlashFrequncy));
=======

        if (damageable is Enemy enemy) enemy.HandlePoisonFlashing(numFlashes, poisonFlashFrequncy, poisonFlashColor);
        else damageable.StartCoroutine(Damageable.FlashColor(damageable.GetComponent<Renderer>(), poisonFlashColor, numFlashes, poisonFlashFrequncy));
>>>>>>> ee62877142338c7d80e7badf05f02c61c1e2a88c
        
        while (timer < poisonDuration)
        {
            // Apply damage based on the DPS
<<<<<<< HEAD
            damageable.ApplyDamage((damagePerSecond * Time.deltaTime));
=======
            damageable.ApplyDamageWithoutEffects((damagePerSecond * Time.deltaTime));
>>>>>>> ee62877142338c7d80e7badf05f02c61c1e2a88c

            // Increment the timer
            timer += Time.deltaTime;

            yield return null;
        }
    }
}