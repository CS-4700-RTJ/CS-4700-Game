using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sensitivity = 2f;

    private Transform playerTransform;
    private Transform headTransform;
    private Rigidbody rb;
    

    private void Start()
    {
        // Assuming the player object and head object are both children of an empty parent object
        playerTransform = transform;
        headTransform = transform.Find("Head");
        rb = GetComponent<Rigidbody>();

        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    private void Update()
    {
        // Handle player movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed * Time.deltaTime;
        playerTransform.Translate(movement);

        // Handle player rotation (looking around)
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float rotationX = mouseY * sensitivity;
        float rotationY = mouseX * sensitivity;

        // Rotate the head (camera) around the X-axis for vertical look
        headTransform.Rotate(Vector3.left * rotationX);

        // Rotate the entire player around the Y-axis for horizontal look
        playerTransform.Rotate(Vector3.up * rotationY);
        
   
}
}