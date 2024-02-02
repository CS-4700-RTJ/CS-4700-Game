using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpellHandler : MonoBehaviour
{
    // Input                                                                         
    PlayerInput playerInput;
    private InputAction castSpellAction;
    private InputAction cycleSpellAction;

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
    }

    private void OnCycleSpell(InputAction.CallbackContext context)
    {
        bool scrollForward = context.ReadValueAsButton();
        
        if (scrollForward) print("Next Spell");
        else print("Previous Spell");
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
