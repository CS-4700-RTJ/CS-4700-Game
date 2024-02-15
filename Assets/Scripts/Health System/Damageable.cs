using System.Collections;
using UnityEngine;

public abstract class Damageable : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth;
    protected float currentHealth;
    
    [Header("Audio")]
    public AudioClip[] damageSounds;
    public AudioClip[] deathSounds;
    
    protected AudioSource audioSource;
    
    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null && (damageSounds.Length > 0 || deathSounds.Length > 0))
        {
            Debug.LogWarning("There is no audio source, adding one!", this);
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        currentHealth = maxHealth;
    }
    
    /// <summary>
    /// Subtracts the specified amount from the damageables health, checks for death, and plays any audio sfx.
    /// This is the ApplyDamage that always actually applies the damage.
    /// </summary>
    /// <param name="amount">The amount of damage to take</param>
    public virtual void ApplyDamage(float amount)
    {
        print("damaging " + this.name);
        currentHealth -= amount;

        if (currentHealth <= 0f) Death();
        else PlayDamagedSound();
    }
    
    /// <summary>
    /// Called whenever the damageable dies.
    /// Needs to be implemented by any child classes.
    /// </summary>
    protected abstract void Death();    

    /// <summary>
    /// Plays a random damage sound.
    /// </summary>
    protected void PlayDamagedSound()
    {
        if (damageSounds.Length == 0)
        {
            return;
        }
        
        AudioClip clipToPlay = damageSounds[Random.Range(0, damageSounds.Length)];

        if (clipToPlay) audioSource.PlayOneShot(clipToPlay);
    }

    protected void PlayDeathSound()
    {
        if (deathSounds.Length == 0)
        {
            return;
        }
        
        AudioClip deathSound = deathSounds[Random.Range(0, deathSounds.Length)];
        audioSource.PlayOneShot(deathSound);
    }
    
    public static IEnumerator FlashColor(Renderer renderer, Color color, int numFlashes = 1, float flashFrequency = 0.2f)
    {
        float flashTimer = 0;

        bool goingToColor = true;

        var material = renderer.material;
        var originalColor = material.color;
        
        int remainingFlashes = numFlashes;
        while (remainingFlashes > 0)
        {
            // Lerp the player's mesh color between its normal color and red
            material.color = Color.Lerp(originalColor, color, flashTimer);

            if (goingToColor)
            {
                flashTimer += Time.deltaTime / flashFrequency;
                if (flashTimer >= 1) goingToColor = false;
            }
            else
            {
                flashTimer -= Time.deltaTime / flashFrequency;
                if (flashTimer <= 0)
                {
                    goingToColor = true;
                    remainingFlashes--;
                }
            }

            yield return null;
        }
        
        material.color = originalColor;
    }
    
    /// <summary>
    /// Destroys this GameObject after any playing sound effects finish.
    /// In the meantime, it will be "soft" destroyed, with rendering and collisions disabled.
    /// </summary>
    protected virtual IEnumerator DestroyAfterSfx()
    {
        if (!audioSource.isPlaying)
        {
            Destroy(gameObject);
            yield break;
        }
        
        GetComponent<Renderer>().enabled = false;
        enabled = false;
        
        while (audioSource.isPlaying) yield return null;
        
        Destroy(gameObject);
    }
}
