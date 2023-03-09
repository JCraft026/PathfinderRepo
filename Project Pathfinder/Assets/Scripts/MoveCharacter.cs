using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using Newtonsoft.Json;

static class MoveCharacterConstants{
    public const float FORWARD  = 1f; // Character facing forward
    public const float LEFT     = 2f; // Character facing left
    public const float BACKWARD = 3f; // Character facing backward
    public const float RIGHT    = 4f; // Character facing right
}

public class MoveCharacter : NetworkBehaviour
{
    public GameObject flashlight;                            // Character's flashlight object (if they have one)
    public float moveSpeed = 5f;                             // Speed at which the character needs to move
    public float facingDirection;                            // Direction the character should face after movement
    Vector2 movementInput;                                   // Character's current input direction             
    public Rigidbody2D rigidBody;                            // Character's RigidBody
    public Animator animator;                                // Character's animator manager
    public static bool canMove = true;                       // Character movement lock status
    public GameObject PauseCanvas;                           // Exit game menu
    private Vector2 lastMovementInput;                       // Unused as of now remove later (-Caleb)
    private float? lastFacingDirection;                      // Unused as of now remove later (-Caleb)
    private GameObject characterArrow;                       // Arrow of the current active character
    private float mazeWidth = 13;                            // Width of the maze
    private float mazeHeight = 13;                           // Height of the maze
    private WallStatus[,] mazeData = new WallStatus[13, 13]; // Maze data
    private WallStatus currentCell;                          // Wall status of the cell the parent character object is in
    private float currentCellY;                              // Y position of the current cell
    private int[] characterCellLocation = new int[2];        // Cell location of the current character
    private int activeCharacterCode;                         // Code identifying the current active character

    // Initialize the exit game menu variable
    private void Awake()
    {
        PauseCanvas = GameObject.Find("PauseCanvas");
    }

    void Start(){

        // Process maze data
        string mazeDataJson = CustomNetworkManagerDAO.GetNetworkManagerGameObject().GetComponent<CustomNetworkManager>().mazeRenderer.GiveMazeDataToNetworkManager();
        mazeData = JsonConvert.DeserializeObject<WallStatus[,]>(mazeDataJson);

        // Assign active character code and character arrow
        if(Utilities.runnerRegex.IsMatch(gameObject.name)){
            activeCharacterCode = ManageActiveCharactersConstants.RUNNER;
            characterArrow      = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Red Arrow"));
        }
        else if(Utilities.chaserRegex.IsMatch(gameObject.name)){
            activeCharacterCode = ManageActiveCharactersConstants.CHASER;
            characterArrow      = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Green Arrow"));
        }
        else if(Utilities.engineerRegex.IsMatch(gameObject.name)){
            activeCharacterCode = ManageActiveCharactersConstants.ENGINEER;
            characterArrow      = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Gold Arrow"));
        }
        else if(Utilities.trapperRegex.IsMatch(gameObject.name)){
            activeCharacterCode = ManageActiveCharactersConstants.TRAPPER;
            characterArrow      = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Blue Arrow"));
        }
    }

    // Update is called once per frame
    void Update(){
        if(isLocalPlayer && canMove)
        {
            // Get current input data
            movementInput.x = Input.GetAxisRaw("Horizontal"); // Returns 0 if idle, 1 if right, -1 if left
            movementInput.y = Input.GetAxisRaw("Vertical");   // Returns 0 if idle, 1 if up, -1 if down
            
            // Manage flashlight rotation
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
        if(isLocalPlayer){
            // Get cell location of parent character object
            switch (activeCharacterCode)
            {
                case ManageActiveCharactersConstants.RUNNER:
                    characterCellLocation = Utilities.GetCharacterCellLocation(ManageActiveCharactersConstants.RUNNER);
                    break;
                case ManageActiveCharactersConstants.CHASER:
                    characterCellLocation = Utilities.GetCharacterCellLocation(ManageActiveCharactersConstants.CHASER);
                    break;
                case ManageActiveCharactersConstants.ENGINEER:
                    characterCellLocation = Utilities.GetCharacterCellLocation(ManageActiveCharactersConstants.ENGINEER);
                    break;
                case ManageActiveCharactersConstants.TRAPPER:
                    characterCellLocation = Utilities.GetCharacterCellLocation(ManageActiveCharactersConstants.TRAPPER);
                    break;
            }
            Debug.Log((characterCellLocation[0] + (int)(mazeWidth/2)) + ", " + (characterCellLocation[1] + (int)(mazeHeight/2)));
            currentCell  = mazeData[characterCellLocation[0] + (int)(mazeWidth/2), characterCellLocation[1] + (int)(mazeHeight/2)];
            currentCellY = Utilities.GetMazeCellCoordinate(characterCellLocation[0], characterCellLocation[1]).y;

            // Manage character arrow display
            if(currentCell.HasFlag(WallStatus.BOTTOM) && (currentCellY - gameObject.transform.position.y) > 2.0f){
                characterArrow.SetActive(true);
            }
            else{
                characterArrow.SetActive(false);
            }
            Debug.Log("Current Cell: " + characterCellLocation[0] + characterCellLocation[1]);
            Debug.Log(gameObject.name + "CurrentCellY: " + currentCellY);
            Debug.Log(gameObject.name + ": " + gameObject.transform.position.y);
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
}
