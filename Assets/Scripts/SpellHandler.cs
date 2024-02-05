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

    [Header("Mana")] public int maxMana = 30;
    private float currentMana;

    public Transform wandTransform;
    public GameObject spellChargeVfx;

    // Input                                                                         
    private PlayerInput playerInput;
    private InputAction castSpellAction;
    private InputAction cycleSpellAction;

    private int currentSpellIndex;

    private bool readyToCast;
    private Spell spellBeingCast;
    private Coroutine currentCast;

    private AudioSource audioSource;

    private bool canCastSpell;

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
        
        spellChargeVfx.SetActive(false);
    }

    private void SetSelectedSpell(int spellIndex)
    {
        currentSpellIndex = spellIndex;

        currentSpellImage.sprite = availableSpells[currentSpellIndex].spellIcon;
        
        canCastSpell = availableSpells[currentSpellIndex].manaCost <= currentMana;
        currentSpellImage.color = canCastSpell ? Color.white : disabledColor;
    }

    private void OnCycleSpell(InputAction.CallbackContext context)
    {
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
        if (!canCastSpell || currentCast == null) return;
        
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
            spellCastable.Cast(wandTransform.forward);
        }
        else
        {
            spellChargeVfx.SetActive(false);
        }

        StopCoroutine(currentCast);
    }

    private void CastSpellStarted(InputAction.CallbackContext context)
    {
        if (canCastSpell) currentCast = StartCoroutine(CastSpell(availableSpells[currentSpellIndex]));
    }

    private IEnumerator CastSpell(Spell spell)
    {
        spellBeingCast = spell;
        readyToCast = false;

        spellChargeVfx.SetActive(true);

        yield return new WaitForSeconds(spell.castTime);
        print("Ready to cast!");
        
        // Destroy the charging Vfx to signify that the spell can be cast
        spellChargeVfx.SetActive(false);

        readyToCast = true;
    }
}