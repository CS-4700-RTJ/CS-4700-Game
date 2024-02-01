using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : Damageable
{
    [Header("Damage Invincibility")]
    [SerializeField]
    private float invulnerableTime = 1f;
    private bool isInvulnerable = false;

    [SerializeField, Min(0)]
    private float flashFrequency = 1f;
    [SerializeField, Min(0)]
    private float flashDuration = 2f;
    
    private MeshRenderer renderer;

    protected override void Start()
    {
        base.Start();
        renderer = GetComponent<MeshRenderer>();
    }

    public override void ApplyDamage(float amount)
    {
        if (!isInvulnerable)
        {
            base.ApplyDamage(amount);
            StartCoroutine(ApplyDamageInvulnerability(invulnerableTime));
            StartCoroutine(DamageFlash());
        }
    }

    protected override void Death()
    {
        // Play the death sound
        AudioClip deathSound = deathSounds[Random.Range(0, deathSounds.Length)];
        audioSource.PlayOneShot(deathSound);

        // Disable inputs
        // TODO - after player input is implemented

        // Disable PlayerController
        var controller = GetComponent<PlayerController>();
        controller.enabled = false;
    }

    private IEnumerator DamageFlash()
    {
        float startTime = Time.time;
        float flashTimer = flashFrequency;

        var playerMat = renderer.material;
        var originalColor = playerMat.color;

        while (Time.time - startTime <= flashDuration)
        {
            // Lerp the player's mesh color between its normal color and red
            playerMat.color = Color.Lerp(originalColor, Color.red, flashTimer);

            flashTimer += Time.deltaTime / flashDuration;
            
            yield return null;
        }
        
        playerMat.color = originalColor;
    }
    
    private IEnumerator ApplyDamageInvulnerability(float invulnerableTime)
    {
        isInvulnerable = true;

        yield return new WaitForSeconds(invulnerableTime);

        isInvulnerable = false;
    }
}