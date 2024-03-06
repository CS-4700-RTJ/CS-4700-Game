using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Damageable
{
    [Header("UI")] public Slider healthBar;

    [Header("Damage Invincibility")]
    [SerializeField]
    private float invulnerableTime = 1f;
    private bool isInvulnerable = false;

    [SerializeField, Min(0)]
    private float flashFrequency = 0.2f;
    [SerializeField, Min(0)]
    private int numFlashes = 1;
    
    private MeshRenderer renderer;
    private PlayerController _playerController;
    
    protected override void Start()
    {
        base.Start();
        renderer = GetComponentInChildren<MeshRenderer>();
        _playerController = GetComponent<PlayerController>();
        
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

        // Disable inputs
        // TODO - after player input is implemented

        // Disable PlayerController
        _playerController.enabled = false;

        healthBar.value = 0;
    }

    private IEnumerator ApplyDamageInvulnerability(float time)
    {
        isInvulnerable = true;

        yield return new WaitForSeconds(time);

        isInvulnerable = false;
    }
}