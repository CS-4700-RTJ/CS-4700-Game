using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Damageable
{
    protected override void Death()
    {
        PlayDeathSound();

        // Replace with death animation
        
        StartCoroutine(DestroyAfterSfx());
    }
}