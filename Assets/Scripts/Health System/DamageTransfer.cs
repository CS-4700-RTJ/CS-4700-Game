using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTransfer : Damageable
{
    [Header("Transfer Info")]
    public Damageable target;
    public float damageMultiplier = 1f;

    public override void ApplyDamage(float amount)
    {
        if (target) target.ApplyDamage(amount * damageMultiplier);
    }

    protected override void Death()
    {
        
    }
}
