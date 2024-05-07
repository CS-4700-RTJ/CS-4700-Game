
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SpellHandler : MonoBehaviour
{
    
    [Header("Spell UI")]
    public Image currentSpellImage;
    public TMP_Text spellNameText;
    public Slider manaSlider;
    public Color disabledColor;
    public Image progressReticle;

    public Spell[] availableSpells;

    [Header("Mana")] 
    public int maxMana = 30;
    public float manaRegenRate = 2f;
    private float currentMana;

    [Header("Effects")]
    public Transform castTransform;
    public Transform wandTransform;
    public GameObject spellChargeVfx;
    public AudioSource wandAudioSource;
    
    public float WandCameraOffset = 28f;

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

    private Camera _mainCamera;

    private Animator animator;
    private static readonly int AnimatorIsCharging = Animator.StringToHash("IsCharging");
    private static readonly int AnimatorChargeFinished = Animator.StringToHash("ChargeFinished");

    private float spellTextVisibleDuration = 2f;
    private float spellTextFadeTime = 1f;
    private Coroutine spellNameFadeRoutine;
    
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
        animator = GetComponent<Animator>();

        currentMana = maxMana;
        manaSlider.value = 1;

        currentSpellIndex = 0;
        currentCast = null;
        canCastSpell = true;
        castingDisabled = false;

        _mainCamera = Camera.main;

        spellNameFadeRoutine = StartCoroutine(FadeSpellText());
        
        spellChargeVfx.SetActive(false);
        progressReticle.fillAmount = 0f;
    }
    
    private void Update()
    {
        currentMana = Math.Min(currentMana + manaRegenRate * Time.deltaTime, maxMana);
        manaSlider.value = currentMana / maxMana;
        
        canCastSpell = availableSpells[currentSpellIndex].manaCost <= currentMana;
        currentSpellImage.color = canCastSpell ? Color.white : disabledColor;
    }

    public void IncreaseMaxMana(int increase)
    {
        maxMana += increase;
        currentMana += increase;
        manaSlider.value = currentMana / maxMana;
    }
    
    /// <summary>
    /// Selects the very first spell in the list of available spells, so that the UI matches
    /// </summary>
    public void SelectStartingSpell() 
    {
        SetSelectedSpell(0);
    }

    private void SetSelectedSpell(int spellIndex)
    {
        currentSpellIndex = spellIndex;

        currentSpellImage.sprite = availableSpells[currentSpellIndex].spellIcon;

        if (spellNameFadeRoutine != null) StopCoroutine(spellNameFadeRoutine);
        spellNameText.text = availableSpells[currentSpellIndex].spellName;
        spellNameFadeRoutine = StartCoroutine(FadeSpellText());
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
            if (spellBeingCast.castSfx[0])
            {
                var clipToPlay = spellBeingCast.castSfx[UnityEngine.Random.Range(0, spellBeingCast.castSfx.Length)];
                audioSource.PlayOneShot(clipToPlay);
            }
                
            // Create spell projectile
            GameObject spellObject = Instantiate(availableSpells[currentSpellIndex].spellPrefab);
            ICastable spellCastable = spellObject.GetComponent<ICastable>();

            // Change with direction player is looking/aiming
            var ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.48f, 10f));

            spellObject.transform.SetPositionAndRotation(castTransform.position,
                Quaternion.LookRotation(ray.direction.normalized));
            
            spellCastable.Cast(ray.direction, castTransform);
        }
        else
        {
            spellChargeVfx.SetActive(false);
        }

        StopCoroutine(currentCast);
        currentCast = null;
        wandAudioSource.Stop();
        animator.SetBool(AnimatorIsCharging, false);
        animator.speed = 1;
        progressReticle.fillAmount = 0f;
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
        
        animator.SetBool(AnimatorIsCharging, true);
        animator.SetBool(AnimatorChargeFinished, false);
        animator.speed = 1 / spell.castTime;

        // use pitch to change playback speed
        // This makes the spell charge SFX last as long as the spell cast time
        float clipLength = wandAudioSource.clip.length;
        float pitch = clipLength / spell.castTime;

        wandAudioSource.pitch = pitch;

        float timer = 0f;
        while (timer < spell.castTime)
        {
            yield return null;
            
            timer += Time.deltaTime;
            progressReticle.fillAmount = timer / spell.castTime;
        }

        progressReticle.fillAmount = 1f;
        // yield return new WaitForSeconds(spell.castTime);
        wandAudioSource.Stop();
        
        // Destroy the charging Vfx to signify that the spell can be cast
        spellChargeVfx.SetActive(false);

        readyToCast = true;
        animator.SetBool(AnimatorChargeFinished, true);
    }

    private void DisableCasting()
    {
        castingDisabled = true;
    }

    private void EnableCasting()
    {
        castingDisabled = false;
    } 

    public IEnumerator DisableCastingForTime(float time)
    {
        castingDisabled = true;

        yield return new WaitForSeconds(time);

        castingDisabled = false;
    }
    
    private IEnumerator FadeSpellText() 
    {        
        // Make spell text fully visible
        Color textColor = Color.white;

        spellNameText.color = textColor;

        yield return new WaitForSeconds(spellTextVisibleDuration);
        
        float timer = spellTextFadeTime;
        
        while (timer > 0f)
        {
            textColor.a = Mathf.Lerp(0, 1, timer / spellTextFadeTime);
            spellNameText.color = textColor;
            
            timer -= Time.deltaTime;
            yield return null;
        }

        textColor.a = 0;
        spellNameText.color = textColor;
    }
}