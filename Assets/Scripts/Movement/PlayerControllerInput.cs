using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerControllerInput : MonoBehaviour
{
    public Vector2 move { get; private set; }
    public Vector2 look { get; private set; }
    public bool jump { get; set; }
    public bool sprint { get; private set; }

    public bool analogMovement;
    
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Movement"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;
        lookAction.performed -= OnLook;
        lookAction.canceled -= OnLook;
        jumpAction.performed -= OnJump;
        jumpAction.canceled -= OnJump;
        sprintAction.performed -= OnSprint;
        sprintAction.canceled -= OnSprint;
    }

    private void Start()
    {
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
        lookAction.performed += OnLook;
        lookAction.canceled += OnLook;
        jumpAction.performed += OnJump;
        jumpAction.canceled += OnJump;
        sprintAction.performed += OnSprint;
        sprintAction.canceled += OnSprint;
    }

    public bool IsCurrentDeviceMouse()
    {
        return playerInput.currentControlScheme == "Keyboard Mouse";
    }

    /// <summary>
    /// Manually stops sprint input.
    /// Useful if a status effect prevents sprint, or if the player has no stamina
    /// </summary>
    public void StopSprint()
    {
        sprint = false;
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        // If the application has focus, then confine the mouse if time is paused for an upgrade, or lock it otherwise
        Cursor.lockState = hasFocus ? (Time.timeScale < 0.1 ? CursorLockMode.Confined : CursorLockMode.Locked) : CursorLockMode.None;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }
    
    private void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        jump = context.ReadValueAsButton();
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        sprint = context.ReadValueAsButton();
    }
}
