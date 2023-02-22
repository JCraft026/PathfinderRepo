using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using Mirror;

static class MoveCharacterConstants{
    public const float FORWARD  = 1f; // Character facing forward
    public const float LEFT     = 2f; // Character facing left
    public const float BACKWARD = 3f; // Character facing backward
    public const float RIGHT    = 4f; // Character facing right
}

public class MoveCharacter : NetworkBehaviour
{
    public GameObject flashlight; // Theis character's flashlight object (if they have one)
    public float moveSpeed = 5f,  // Speed at which the character needs to move
                 facingDirection; // Direction the character should face after movement
    Vector2 movementInput;        // Character's current input direction             
    public Rigidbody2D rigidBody; // Character's RigidBody
    public Animator animator;     // Character's animator manager
    public static bool canMove = true;   
                                  // Character movement lock status
    public GameObject PauseCanvas; 
                                  // Exit game menu

    private Vector2 lastMovementInput;  //Unused as of now remove later (-Caleb)
    private float? lastFacingDirection; //Unused as of now remove later (-Caleb)

    private Player_UI playerUi;     // Imports the Player's UI to access what is the player
    public static MoveCharacter Instance; // Makes an instance of this class to access 
    Regex runnerExpression = new Regex("Runner"); // Match "Runner"
    
    public override void OnStartAuthority(){
        base.OnStartAuthority();
        if(runnerExpression.IsMatch(gameObject.name))
        {
            Debug.Log("runnerExpression matched, initializing inventory UI");
            Instance = this;
            playerUi = GameObject.Find("Player_UI").GetComponent<Player_UI>();
            playerUi.SetPlayer(this);
        }
    // Initialize the exit game menu variable
    private void Awake()
    {
        PauseCanvas = GameObject.Find("PauseCanvas");
    }

    // Update is called once per frame
    void Update(){
        if(isLocalPlayer && canMove)
        {
            // Get current input data
            movementInput.x = Input.GetAxisRaw("Horizontal"); // Returns 0 if idle, 1 if right, -1 if left
            movementInput.y = Input.GetAxisRaw("Vertical");   // Returns 0 if idle, 1 if up, -1 if down
            
            // Flashlight rotation
            if ((flashlight != null) && !((movementInput.x == 0) && (movementInput.y == 0)))
            {
                if (movementInput.x == 0)
                    flashlight.transform.eulerAngles = new Vector3(0f, 0f, 180f + 90f * (movementInput.y+1));
                else
                    flashlight.transform.eulerAngles = new Vector3(0f, 0f, 180f + movementInput.x * 45f * (movementInput.y+2));
            }

            if (PauseCanvas.gameObject.activeSelf == false)
            {
                // Set character idle facing direction
                if (movementInput.x == 0 && movementInput.y == -1)
                {
                    facingDirection = MoveCharacterConstants.FORWARD;
                }
                else if (movementInput.x == -1 && movementInput.y == 0)
                {
                    facingDirection = MoveCharacterConstants.LEFT;
                }
                else if (movementInput.x == 0 && movementInput.y == 1)
                {
                    facingDirection = MoveCharacterConstants.BACKWARD;
                }
                else if (movementInput.x == 1 && movementInput.y == 0)
                {
                    facingDirection = MoveCharacterConstants.RIGHT;
                }
            }

            // Communicate movement values with the animator controller
            animator.SetFloat("Horizontal Movement", movementInput.x);
            animator.SetFloat("Vertical Movement", movementInput.y);
            animator.SetFloat("Movement Speed", movementInput.sqrMagnitude); // Set the speed to the squared length of the movementInput vector
            animator.SetFloat("Facing Direction", facingDirection);
        }
    }

    // FixedUpdate calling frequency is based on a set timer
    void FixedUpdate(){
        // Make movement speed equal in all directions
        movementInput.Normalize();

        // Make sure all of the conditions for movement are correct
        if(isLocalPlayer && canMove && PauseCanvas.gameObject.activeSelf == false)
        {
            // Move the character based on the current character position, the input data, the move speed, and the elapesed time since the last function call
            rigidBody.MovePosition(rigidBody.position + movementInput * moveSpeed * Time.fixedDeltaTime);
        }
    }

    public Vector2 getPosition(){
        return rigidBody.position;
    }

    public void greenScreen(){
        Debug.Log("GREEN");
        animator.SetBool("isGreen", true);
        if(!runnerExpression.IsMatch(gameObject.name)){
            if(runnerExpression.IsMatch(gameObject.name)){
                SpriteRenderer runnerRenderer = gameObject.GetComponent<SpriteRenderer>();
                runnerRenderer.color = new Color32(255,255,225,20);
            }
        }
    }

    public void notGreenScreen(){
        animator.SetBool("isGreen", false);
        if(!runnerExpression.IsMatch(gameObject.name)){
            if(runnerExpression.IsMatch(gameObject.name)){
                SpriteRenderer runnerRenderer = gameObject.GetComponent<SpriteRenderer>();
                runnerRenderer.color = new Color32(255,255,225,255);
            }
        }
        Debug.Log("NO GREEN");
    }
}