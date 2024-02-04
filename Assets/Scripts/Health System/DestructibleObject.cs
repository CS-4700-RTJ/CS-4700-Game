using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : Damageable
{
    public GameObject createOnDestroy;

    // DestructibleObject's don't have health but instead die on any hit
    public override void ApplyDamage(float amount)
    {
        Death();
    }

    protected override void Death()
    {
        if (deathSounds.Length > 0) StartCoroutine(DestroyAfterSFX());
        else Destroy();
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

    private IEnumerator DestroyAfterSFX()
    {
        AudioClip deathClip = deathSounds[Random.Range(0, deathSounds.Length)];
        
        audioSource.PlayOneShot(deathClip);

        yield return new WaitForSeconds(deathClip.length);

        Destroy();
    }
}
