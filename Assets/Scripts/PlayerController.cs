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

    public bool isSprinting;
    
    // Input variables
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintStartAction;
    private InputAction sprintEndAction;

    private Vector3 moveDirection;
    private Vector3 airbornMoveDirection;
    private Vector2 lookVector;

    // Movement variables
    private CharacterController controller;
    [SerializeField]
    private float playerJumpVelocity;

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
        sprintStartAction = playerInput.actions["Sprint Pressed"];
        sprintEndAction = playerInput.actions["Sprint Released"];
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;
        lookAction.performed -= OnLook;
        lookAction.canceled -= OnLook;
        jumpAction.performed -= Jump;
        sprintStartAction.performed -= SprintPressed;
        sprintEndAction.performed -= SprintReleased;
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main;

        
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
        lookAction.performed += OnLook;
        lookAction.canceled += OnLook;
        jumpAction.performed += Jump;
        sprintStartAction.performed += SprintPressed;
        sprintEndAction.performed += SprintReleased;
    }

    private void FixedUpdate()
    {
        if (controller.isGrounded && playerJumpVelocity < 0)
        {
            playerJumpVelocity = -2f;
        }
        else
        {
            playerJumpVelocity += Physics.gravity.y * Time.fixedDeltaTime;
        }
        
    }

    private void Update(){
        // Process movement and looking
        if(isSprinting)
            ProcessMove(sprintMoveSpeed);
        else{
            ProcessMove(defaultMoveSpeed);
        }
        ProcessLook();
    }

    // Moves the player according to gravity and player input
    private void ProcessMove(float ms)
    {
        controller.Move(transform.TransformDirection(moveDirection) * (ms * Time.fixedDeltaTime));
        
        // Make sure you normalize the move vector so that you don't move faster in a diagonal direction (Pythagorean theorem)
        Vector3 move = transform.TransformDirection(moveDirection).normalized * (ms * Time.fixedDeltaTime);
        move += Vector3.up * (playerJumpVelocity * Time.fixedDeltaTime);
        
        controller.Move(move);
    }
    private void OnMove(InputAction.CallbackContext context)
    {
        if(controller.isGrounded){
            Vector2 input = context.ReadValue<Vector2>();
            moveDirection = new Vector3(input.x, 0, input.y);
        }
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
            playerJumpVelocity = jumpStrength;
        }
    }

    private void SprintPressed(InputAction.CallbackContext context){
        if(context.performed && controller.isGrounded){
            Debug.Log("Sprinting!");
            isSprinting = true;
        }
    }
    private void SprintReleased(InputAction.CallbackContext context){
        if(context.performed){
            Debug.Log("Not Sprinting!");
            isSprinting = false;
        }
    }
}
