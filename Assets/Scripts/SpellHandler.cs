using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SpellHandler : MonoBehaviour
{
    [Header("Spell UI")]
    public Image currentSpellImage;

    public Spell[] availableSpells;
    
    // Input                                                                         
    private PlayerInput playerInput;
    private InputAction castSpellAction;
    private InputAction cycleSpellAction;

    private int currentSpellIndex;
    
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

        currentSpellIndex = 0;
    }

    private void SetSelectedSpell(int spellIndex)
    {
        currentSpellIndex = spellIndex;

        currentSpellImage.sprite = availableSpells[currentSpellIndex].spellIcon;
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
        //print("Mouse released");
    }

    private void CastSpellStarted(InputAction.CallbackContext context)
    {
        //print("Mouse Pressed");
    }
}
