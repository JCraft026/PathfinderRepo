using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

static class MoveCharacterConstants{
    public const float FORWARD  = 1f; // Character facing forward
    public const float LEFT     = 2f; // Character facing left
    public const float BACKWARD = 3f; // Character facing backward
    public const float RIGHT    = 4f; // Character facing right
}

public class MoveCharacter : NetworkBehaviour
{
    public float moveSpeed = 5f,  // Speed at which the character needs to move
                 facingDirection; // Direction the character should face after movement
    Vector2 movementInput;        // Character's current input direction             
    public Rigidbody2D rigidBody; // Character's RigidBody
    public Animator animator;     // Character's animator manager

    private Vector2 lastMovementInput;
    private float? lastFacingDirection;

    // Update is called once per frame
    void Update(){
        if(isLocalPlayer)
        {
            // Get current input data
            movementInput.x = Input.GetAxisRaw("Horizontal"); // Returns 0 if idle, 1 if right, -1 if left
            movementInput.y = Input.GetAxisRaw("Vertical");   // Returns 0 if idle, 1 if up, -1 if down
        

            // Set character idle facing direction
            if(movementInput.x == 0 && movementInput.y == -1){
                facingDirection = MoveCharacterConstants.FORWARD;
            }
            else if(movementInput.x == -1 && movementInput.y == 0){
                facingDirection = MoveCharacterConstants.LEFT;
            }
            else if(movementInput.x == 0 && movementInput.y == 1){
                facingDirection = MoveCharacterConstants.BACKWARD;
            }
            else if(movementInput.x == 1 && movementInput.y == 0){
                facingDirection = MoveCharacterConstants.RIGHT;
            }

            // Communicate movement values with the animator controller
            animator.SetFloat("Horizontal Movement", movementInput.x);
            animator.SetFloat("Vertical Movement", movementInput.y);
            animator.SetFloat("Movement Speed", movementInput.sqrMagnitude); // Set the speed to the squared length of the movementInput vector
            animator.SetFloat("Facing Direction", facingDirection);

            // Determine whether or not to send an animation update message to the other player
            // Note: Animation state messages are only sent to the other player if the local player's animation parameters change. This is done to minimize network traffic.
            if(lastMovementInput != null && lastFacingDirection != null)
            {
                if(lastMovementInput != movementInput || lastFacingDirection != facingDirection)
                {
                    int characterType;
                    if(gameObject.name.Contains("Runner"))
                    {
                        characterType = CustomNetworkManager.PLAYER_TYPE_RUNNER;
                    }
                    else if(gameObject.name.Contains("Mechanic"))
                    {
                        characterType = CustomNetworkManager.PLAYER_TYPE_MECHANIC;
                    }
                    else if(gameObject.name.Contains("Chaser"))
                    {
                        characterType = CustomNetworkManager.PLAYER_TYPE_CHASER;
                    }
                    else if(gameObject.name.Contains("Trapper"))
                    {
                        characterType = CustomNetworkManager.PLAYER_TYPE_TRAPPER;
                    }
                    else
                    {
                        characterType = CustomNetworkManager.PLAYER_TYPE_UNKNOWN;
                    }
                    CustomNetworkManager.AnimationMessage animationMessage = new();
                    animationMessage.characterFacingDirection = facingDirection;
                    animationMessage.characterType = characterType;
                    animationMessage.connId = this.connectionToServer.connectionId;
                    NetworkServer.SendToReady<CustomNetworkManager.AnimationMessage>(animationMessage);
                }
            }

            else
            {
                lastFacingDirection = facingDirection;
                lastMovementInput = movementInput;
            }
        }
    }

    // FixedUpdate calling frequency is based on a set timer
    void FixedUpdate(){
        // Make movement speed equal in all directions
        movementInput.Normalize();

        // Move the character based on the current character position, the input data, the move speed, and the elapesed time since the last function call
        rigidBody.MovePosition(rigidBody.position + movementInput * moveSpeed * Time.fixedDeltaTime);
    }
}
