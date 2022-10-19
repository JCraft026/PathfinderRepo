using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCharacter : MonoBehaviour
{
    public float moveSpeed = 5f;   // Speed at which the character is moving
    public Rigidbody2D rigidBody;  // Character's RigidBody
    Vector2 movementInput;         // Character's current input direction
    public Animator animator;      // Character's animator manager

    // Update is called once per frame
    void Update(){
        // Get current input data
        movementInput.x = Input.GetAxisRaw("Horizontal"); // Returns 0 if idle, 1 if right, -1 if left
        movementInput.y = Input.GetAxisRaw("Vertical");   // Returns 0 if idle, 1 if up, -1 if down

        // Communicate movement values with the animator controller
        animator.SetFloat("Horizontal Movement", movementInput.x);
        animator.SetFloat("Vertical Movement", movementInput.y);
        animator.SetFloat("Movement Speed", movementInput.sqrMagnitude); // Set the speed to the squared length of the movementInput vector
    }

    // FixedUpdate calling frequency is based on a set timer
    void FixedUpdate(){
        // Move the character based on the current character position, the input data, the move speed, and the elapesed time since the last function call
        rigidBody.MovePosition(rigidBody.position + movementInput * moveSpeed * Time.fixedDeltaTime);
    }
}
