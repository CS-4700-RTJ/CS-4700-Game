using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SpellHandler : MonoBehaviour
{
    
    [Header("Spell UI")]
    public Image currentSpellImage;
    public Slider manaSlider;
    public Color disabledColor;

    public Spell[] availableSpells;

    [Header("Mana")] 
    public int maxMana = 30;
    public float manaRegenRate = 2f;
    private float currentMana;

    [Header("Effects")]
    public Transform wandTransform;
    public GameObject spellChargeVfx;
    public AudioSource wandAudioSource;

    // Input                                                                         
    private PlayerInput playerInput;
    private InputAction castSpellAction;
    private InputAction cycleSpellAction;

    private int currentSpellIndex;

    private bool castingDisabled;
    private bool canCastSpell;
    private bool readyToCast;
    private Spell spellBeingCast;
    private Coroutine currentCast;

    private AudioSource audioSource;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.Enable(); // Make sure player can move at the start      

        castSpellAction = playerInput.actions["Cast Spell"];
        cycleSpellAction = playerInput.actions["Cycle Spell"];
    }

    private void OnDisable()
    {
        castSpellAction.started -= CastSpellStarted;
        castSpellAction.canceled -= CastSpellCanceled;
    }

    private void Start()
    {
        castSpellAction.started += CastSpellStarted;
        castSpellAction.canceled += CastSpellCanceled;
        cycleSpellAction.performed += OnCycleSpell;

        audioSource = GetComponent<AudioSource>();

        currentMana = maxMana;
        manaSlider.value = 1;

        currentSpellIndex = 0;
        currentCast = null;
        canCastSpell = true;
        castingDisabled = false;
        
        Debug.LogWarning("TODO - Wand attached to player arm, IK arm towards center of screen OR wand fixed pointing towards center, not attached to player");
        
        spellChargeVfx.SetActive(false);
    }

    private void Update()
    {
        currentMana = Math.Min(currentMana + manaRegenRate * Time.deltaTime, maxMana);
        manaSlider.value = currentMana / maxMana;
        
        canCastSpell = availableSpells[currentSpellIndex].manaCost <= currentMana;
        currentSpellImage.color = canCastSpell ? Color.white : disabledColor;
    }

    private void SetSelectedSpell(int spellIndex)
    {
        currentSpellIndex = spellIndex;

        currentSpellImage.sprite = availableSpells[currentSpellIndex].spellIcon;
    }

    private void OnCycleSpell(InputAction.CallbackContext context)
    {
        if (currentCast != null || castingDisabled) return;
        
        bool scrollForward = context.ReadValueAsButton();

        int newSpellIndex;
        if (scrollForward)
        {
            newSpellIndex = (currentSpellIndex + 1) % availableSpells.Length;
        }
        else
        {
            newSpellIndex = currentSpellIndex - 1;
            if (newSpellIndex < 0) newSpellIndex = availableSpells.Length - 1;
        }

        SetSelectedSpell(newSpellIndex);
    }

    private void CastSpellCanceled(InputAction.CallbackContext context)
    {
        if (!canCastSpell || currentCast == null || castingDisabled) return;
        
        if (readyToCast)
        {
            // Pay mana
            currentMana -= spellBeingCast.manaCost;
            manaSlider.value = currentMana / maxMana;

            if (currentMana < spellBeingCast.manaCost)
            {
                currentSpellImage.color = disabledColor;
                canCastSpell = false;
            }

            // Play SFX
            if (spellBeingCast.castSfx)
            {
                audioSource.PlayOneShot(spellBeingCast.castSfx);
            }
                
            // Create spell projectile
            GameObject spellObject = Instantiate(availableSpells[currentSpellIndex].spellPrefab);
            ICastable spellCastable = spellObject.GetComponent<ICastable>();

            spellObject.transform.SetPositionAndRotation(wandTransform.position + Vector3.up,
                wandTransform.rotation);

            // Change with direction player is looking/aiming
            var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.48f, 0f));
            spellCastable.Cast(ray.direction, wandTransform);
        }
        else
        {
            spellChargeVfx.SetActive(false);
        }

        StopCoroutine(currentCast);
        currentCast = null;
        wandAudioSource.Stop();
    }

    private void CastSpellStarted(InputAction.CallbackContext context)
    {
        if (canCastSpell && !castingDisabled) currentCast = StartCoroutine(CastSpell(availableSpells[currentSpellIndex]));
    }

    private IEnumerator CastSpell(Spell spell)
    {
        spellBeingCast = spell;
        readyToCast = false;

        spellChargeVfx.SetActive(true);
        wandAudioSource.Play();
        
        // use pitch to change playback speed
        // This makes the spell charge SFX last as long as the spell cast time
        float clipLength = wandAudioSource.clip.length;
        float pitch = clipLength / spell.castTime;

        wandAudioSource.pitch = pitch;

        yield return new WaitForSeconds(spell.castTime);
        wandAudioSource.Stop();
        
        // Destroy the charging Vfx to signify that the spell can be cast
        spellChargeVfx.SetActive(false);

        readyToCast = true;
    }

    public IEnumerator DisableCastingForTime(float time)
    {
        castingDisabled = true;

        yield return new WaitForSeconds(time);

        castingDisabled = false;
    }
}