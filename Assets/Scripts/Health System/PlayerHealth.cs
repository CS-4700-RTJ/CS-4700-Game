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
    private int numFlashes = 2;
    
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
        PlayDeathSound();

        // Disable inputs
        // TODO - after player input is implemented

        // Disable PlayerController
        var controller = GetComponent<PlayerController>();
        controller.enabled = false;
    }

    private IEnumerator DamageFlash()
    {
        float startTime = Time.time;
        float flashTimer = 0;

        bool goingToRed = true;

        var playerMat = renderer.material;
        var originalColor = playerMat.color;


        int remainingFlashes = numFlashes;
        while (remainingFlashes > 0)
        {
            // Lerp the player's mesh color between its normal color and red
            playerMat.color = Color.Lerp(originalColor, Color.red, flashTimer);

            if (goingToRed)
            {
                flashTimer += Time.deltaTime / flashFrequency;
                if (flashTimer >= 1) goingToRed = false;
            }
            else
            {
                flashTimer -= Time.deltaTime / flashFrequency;
                if (flashTimer <= 0)
                {
                    goingToRed = true;
                    remainingFlashes--;
                }
            }

            yield return null;
        }
        
        playerMat.color = originalColor;
    }
    
    private IEnumerator ApplyDamageInvulnerability(float time)
    {
        isInvulnerable = true;

        yield return new WaitForSeconds(time);

        isInvulnerable = false;
    }
}