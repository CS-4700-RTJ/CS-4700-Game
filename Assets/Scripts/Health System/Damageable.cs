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

        if (audioSource == null)
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
        AudioClip clipToPlay = damageSounds[Random.Range(0, damageSounds.Length)];

        if (clipToPlay) audioSource.PlayOneShot(clipToPlay);
    }
}
