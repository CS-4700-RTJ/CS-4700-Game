using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private CharacterController controller;
    private Vector3 playerVelocity;
    public float speed = 5f;
    public float jumpHeight = 3f;
    
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // receive the inputs for our InputManager.cs and apply them to our character controller
    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * (speed * Time.deltaTime));

        playerVelocity += Physics.gravity * Time.deltaTime;
            if (controller.isGrounded && playerVelocity.y < 0)
            {
                playerVelocity.y = -2f;
            }
        controller.Move(playerVelocity *Time.deltaTime);
    }

    public void Jump()
    {
        if (controller.isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * Physics.gravity.y);
        }
    }
    
    
}
