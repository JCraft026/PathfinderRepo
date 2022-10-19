using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Constants{
    public const float FORWARD  = 1f; // Character facing forward
    public const float LEFT     = 2f; // Character facing left
    public const float BACKWARD = 3f; // Character facing backward
    public const float RIGHT    = 4f; // Character facing right
}

public class MoveCharacter : MonoBehaviour
{
    public float moveSpeed = 5f,  // Speed at which the character needs to move
                 facingDirection; // Direction the character should face after movement
    Vector2 movementInput;        // Character's current input direction             
    public Rigidbody2D rigidBody; // Character's RigidBody
    public Animator animator;     // Character's animator manager

    // Update is called once per frame
    void Update(){
        // Get current input data
        movementInput.x = Input.GetAxisRaw("Horizontal"); // Returns 0 if idle, 1 if right, -1 if left
        movementInput.y = Input.GetAxisRaw("Vertical");   // Returns 0 if idle, 1 if up, -1 if down

        // Set character idle facing direction
        if(movementInput.x == 0 && movementInput.y == -1){
            facingDirection = Constants.FORWARD;
        }
        else if(movementInput.x == -1 && movementInput.y == 0){
            facingDirection = Constants.LEFT;
        }
        else if(movementInput.x == 0 && movementInput.y == 1){
            facingDirection = Constants.BACKWARD;
        }
        else if(movementInput.x == 1 && movementInput.y == 0){
            facingDirection = Constants.RIGHT;
        }

        // Communicate movement values with the animator controller
        animator.SetFloat("Horizontal Movement", movementInput.x);
        animator.SetFloat("Vertical Movement", movementInput.y);
        animator.SetFloat("Movement Speed", movementInput.sqrMagnitude); // Set the speed to the squared length of the movementInput vector
        animator.SetFloat("Facing Direction", facingDirection);
    }

    // FixedUpdate calling frequency is based on a set timer
    void FixedUpdate(){
        // Make movement speed equal in all directions
        movementInput.Normalize();

        // Move the character based on the current character position, the input data, the move speed, and the elapesed time since the last function call
        rigidBody.MovePosition(rigidBody.position + movementInput * moveSpeed * Time.fixedDeltaTime);
    }
}
