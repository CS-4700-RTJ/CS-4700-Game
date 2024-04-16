using System.Collections;
using System.Collections.Generic;
<<<<<<< HEAD
=======
using Cinemachine;
>>>>>>> ee62877142338c7d80e7badf05f02c61c1e2a88c
using Unity.VisualScripting;
using UnityEngine;

public class DestructibleObject : Damageable
{
    public GameObject createOnDestroy;
    
    // DestructibleObject's don't have health but instead die on any hit
    //public override void ApplyDamage(float amount)
    //{
    //    Death();
    //}

    protected override void Death()
    {
<<<<<<< HEAD
        if (deathSounds.Length > 0)
        {
            PlayDeathSound();
            StartCoroutine(DestroyAfterSfx());
        }
        else Destroy();
=======
        // if (deathSounds.Length > 0)
        // {
        //     PlayDeathSound();
        //     StartCoroutine(DestroyAfterSfx());
        // }
        // else Destroy();
        Destroy();
>>>>>>> ee62877142338c7d80e7badf05f02c61c1e2a88c
    }

    // Creates the object (if applicable) and then destroys this
    protected void Destroy()
    {
        if (createOnDestroy)
        {
            var objectTransform = transform;
            var newObj = Instantiate(createOnDestroy).transform; 
            newObj.SetPositionAndRotation(objectTransform.position, objectTransform.rotation);
            var objectScale = objectTransform.localScale;
            newObj.localScale = objectScale;
        }

        Destroy(gameObject);
    }

    protected override IEnumerator DestroyAfterSfx()
    {
        AudioClip deathClip = deathSounds[Random.Range(0, deathSounds.Length)];
        
        audioSource.PlayOneShot(deathClip);

        yield return StartCoroutine(base.DestroyAfterSfx());

        Destroy();
    }
}
