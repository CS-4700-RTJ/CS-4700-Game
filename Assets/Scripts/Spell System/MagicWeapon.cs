using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicWeapon : MonoBehaviour, ICastable
{
    // TODO - remove this once animated so that it only does 1 weapon swing
    public float weaponActiveTime = 3f;

    public AudioClip weaponAttackSfx;
    
    private SpellHandler playerSpellHandler;
    private AudioSource audioSource;
    
    public void Cast(Vector3 castDirection, Transform casterTransform)
    {
        playerSpellHandler = casterTransform.GetComponentInParent<SpellHandler>();
        transform.SetParent(casterTransform);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        audioSource = GetComponent<AudioSource>();
        
        StartCoroutine(DoMeleeAttack());
    }

    // TODO - animate this and modify as needed
    private IEnumerator DoMeleeAttack()
    {
        playerSpellHandler.StartCoroutine(playerSpellHandler.DisableCastingForTime(weaponActiveTime));

        audioSource.PlayOneShot(weaponAttackSfx);
        
        yield return new WaitForSeconds(weaponActiveTime);
        
        Destroy(gameObject);
    }
}
