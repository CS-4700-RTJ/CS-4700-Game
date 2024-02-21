using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float defaultMoveSpeed = 3f;
    public float sprintMoveSpeed = 5f;
    public float jumpStrength;
    public float maxSprintTime = 5f;
       
    // Input variables
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    private Vector3 moveDirection;
    private Vector3 airbornMoveDirection;
    private Vector2 lookVector;

    // Movement variables
    private CharacterController controller;
    private float playerYVelocity;
    private float airbornSpeed;
    private float availableSprint;
    private bool isSprinting;

    // Look variables
    private Camera cam;
    private float xRotation = 0f;
    [SerializeField]
    private float xSensitivity = 1f;
    [SerializeField]
    private float ySensitivity = 1f;
    
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
        jumpAction.performed -= Jump;
        sprintAction.performed -= SprintPressed;
        sprintAction.canceled -= SprintReleased;
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        availableSprint = maxSprintTime;
        
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
        lookAction.performed += OnLook;
        lookAction.canceled += OnLook;
        jumpAction.performed += Jump;
        sprintAction.performed += SprintPressed;
        sprintAction.canceled += SprintReleased;
    }

    private void Update()
    {
        if (controller.isGrounded && playerYVelocity < 0)
        {
            playerYVelocity = -2f;
        }
        else
        {
            playerYVelocity += Physics.gravity.y * Time.deltaTime;
        }

        if (isSprinting)
        {
            availableSprint -= Time.deltaTime;

            if (availableSprint <= 0)
            {
                isSprinting = false;
            }
        } else if (controller.isGrounded)
        {
            availableSprint += Time.deltaTime;
            if (availableSprint > maxSprintTime) availableSprint = maxSprintTime;
        }
        
        // Process movement and looking
        ProcessMove();
        ProcessLook();
    }

    // Moves the player according to gravity and player input
    private void ProcessMove()
    {
        float moveSpeed = controller.isGrounded ? (isSprinting ? sprintMoveSpeed : defaultMoveSpeed) : airbornSpeed;
        Vector3 moveVector = controller.isGrounded ? moveDirection : airbornMoveDirection;
        
        // Make sure you normalize the move vector so that you don't move faster in a diagonal direction (Pythagorean theorem)
        Vector3 move = transform.TransformDirection(moveVector.normalized) * (moveSpeed * Time.deltaTime);
        controller.Move(move);
        
        controller.Move(Vector3.up * (playerYVelocity * Time.deltaTime));
    }
    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        moveDirection = new Vector3(input.x, 0, input.y);
    }
    
    // Rotates the camera according to the mouse movement
    private void ProcessLook()
    {
        // calculate camera rotation for looking up and down
        xRotation -= (lookVector.y * Time.fixedDeltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        // apply this to our camera transform
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // rotate player to look left and right
        transform.Rotate(Vector3.up * (lookVector.x * Time.fixedDeltaTime * xSensitivity));
    }
    
    private void OnLook(InputAction.CallbackContext context)
    {
        lookVector = context.ReadValue<Vector2>();
    }

    private void Jump(InputAction.CallbackContext context)
    {
        airbornMoveDirection = moveDirection;
        if (controller.isGrounded)
        {
            playerYVelocity = jumpStrength;

            airbornSpeed = isSprinting ? sprintMoveSpeed : defaultMoveSpeed;
        }
    }

    private void SprintPressed(InputAction.CallbackContext context){
        isSprinting = true;
    }
    
    private void SprintReleased(InputAction.CallbackContext context){
        isSprinting = false;
    }
}