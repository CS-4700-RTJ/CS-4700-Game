using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Damageable
{
<<<<<<< HEAD
    [Header("UI")] public Slider healthBar;
=======
    // Have a static reference to the player's transform so that enemies can quickly access it without having to 
    // use GameObject.Find.
    // There should only ever by one PlayerHealth at a time, which is why it is put in this script.
    /// <summary>
    /// Static reference to the Player's transform.
    /// </summary>
    public static Transform PlayerTransform { get; private set; }
    
    [Header("UI")] 
    public Slider healthBar;
>>>>>>> ee62877142338c7d80e7badf05f02c61c1e2a88c

    [Header("Damage Invincibility")]
    [SerializeField]
    private float invulnerableTime = 1f;
    private bool isInvulnerable = false;

    [SerializeField, Min(0)]
    private float flashFrequency = 0.2f;
    [SerializeField, Min(0)]
    private int numFlashes = 1;
    
    private MeshRenderer renderer;
    
    private void Awake()
    {
        PlayerTransform = transform;
    }

    protected override void Start()
    {
        base.Start();
        renderer = GetComponentInChildren<MeshRenderer>();
        
        healthBar.value = 1;
    }

    public override void ApplyDamage(float amount)
    {
        if (!isInvulnerable)
        {
            base.ApplyDamage(amount);
            StartCoroutine(ApplyDamageInvulnerability(invulnerableTime));
            //StartCoroutine(DamageFlash());
            StartCoroutine(FlashColor(renderer, Color.red, numFlashes, flashFrequency));

            healthBar.value = currentHealth / maxHealth;
        }
    }

    protected override void Death()
    {
        // Play the death sound
        PlayDeathSound();
<<<<<<< HEAD

        // Disable inputs
        // TODO - after player input is implemented

        // Disable PlayerController
        var controller = GetComponent<PlayerController>();
        controller.enabled = false;

        healthBar.value = 0;
=======
        
        healthBar.value = 0;

        EventManager.TriggerOnPlayerDeath();
>>>>>>> ee62877142338c7d80e7badf05f02c61c1e2a88c
    }

    private IEnumerator ApplyDamageInvulnerability(float time)
    {
        isInvulnerable = true;

        yield return new WaitForSeconds(time);

        isInvulnerable = false;
    }
}