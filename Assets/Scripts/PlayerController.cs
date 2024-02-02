using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Input
    PlayerInput playerInput;
    private InputAction castSpellAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.Enable(); // Make sure player can move at the start
        
        castSpellAction = playerInput.actions["Cast Spell"];
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
    }

    private void CastSpellCanceled(InputAction.CallbackContext context)
    {
        print("Mouse released");
    }

    private void CastSpellStarted(InputAction.CallbackContext context)
    {
        print("Mouse Pressed");
    }
}
