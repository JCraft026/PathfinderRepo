using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using Mirror;
using System.Linq;
using Newtonsoft.Json;
using System;

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
    public Vector2 movementInput;                                   // Character's current input direction             
    public Rigidbody2D rigidBody;                            // Character's RigidBody
    public Animator animator;                                // Character's animator manager
    public bool canMove = true;                       // Character movement lock status
    public GameObject PauseCanvas;                           // Exit game menu
    public bool isRestricted = true;                         // Status of parent guard objects movement restricted
    private GameObject characterArrow;                       // Arrow of the current active character
    private float mazeWidth = 13;                            // Width of the maze
    private float mazeHeight = 13;                           // Height of the maze
    private WallStatus[,] mazeData = new WallStatus[13, 13]; // Maze data
    private WallStatus currentCell;                          // Wall status of the cell the parent character object is in
    private float currentCellY;                              // Y position of the current cell
    private int[] characterCellLocation = new int[2];        // Cell location of the current character
    private int activeCharacterCode;                         // Code identifying the current active character
    public AudioSource audioSource;                          // Makes the player make sounds

    private Player_UI playerUi;     // Imports the Player's UI to access what is the player
    public static MoveCharacter Instance; // Makes an instance of this class to access 
    Regex runnerExpression = new Regex("Runner"); // Match "Runner"
    [SyncVar]
    public bool isMoving; // Keeps track of if the player is moving, this way the client can play the correct audio
    
    public override void OnStartAuthority(){
        base.OnStartAuthority();
        if(CustomNetworkManager.isRunner)
        {
            Debug.Log("Initializing runner inventory UI");
            Instance = this;
            playerUi = GameObject.Find("Player_UI").GetComponent<Player_UI>();
            playerUi.SetPlayer(this);
        }
    }
    // Initialize the exit game menu variable
    void Awake()
    {
        PauseCanvas = GameObject.Find("PauseCanvas");
        isMoving = false;
    }

    void Start(){

        // Process maze data
        //string mazeDataJson = CustomNetworkManagerDAO.GetNetworkManagerGameObject().GetComponent<CustomNetworkManager>().mazeRenderer.GiveMazeDataToNetworkManager();
        mazeData = CustomNetworkManagerDAO.GetNetworkManagerGameObject().GetComponent<CustomNetworkManager>().parsedMazeJson;
        //mazeData = JsonConvert.DeserializeObject<WallStatus[,]>(mazeDataJson);

        // Assign active character code and character arrow
        if(Utilities.runnerRegex.IsMatch(gameObject.name)){
            activeCharacterCode = ManageActiveCharactersConstants.RUNNER;
        }
        else if(Utilities.chaserRegex.IsMatch(gameObject.name)){
            activeCharacterCode = ManageActiveCharactersConstants.CHASER;
        }
        else if(Utilities.engineerRegex.IsMatch(gameObject.name)){
            activeCharacterCode = ManageActiveCharactersConstants.ENGINEER;
        }
        else if(Utilities.trapperRegex.IsMatch(gameObject.name)){
            activeCharacterCode = ManageActiveCharactersConstants.TRAPPER;
        }
    }

    // Update is called once per frame
    void Update(){
        // Assign active character code and character arrow
        if(Utilities.runnerRegex.IsMatch(gameObject.name)){
            characterArrow = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Red Arrow"));
        }
        else if(Utilities.chaserRegex.IsMatch(gameObject.name)){
            characterArrow = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Green Arrow"));
        }
        else if(Utilities.engineerRegex.IsMatch(gameObject.name)){
            characterArrow = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Gold Arrow"));
        }
        else if(Utilities.trapperRegex.IsMatch(gameObject.name)){
            characterArrow = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Blue Arrow"));
        }

        // Ensure the mazeData is not null. If it is we should not be using mazeData at all
        if(mazeData == null)
        {
            mazeData = CustomNetworkManagerDAO.GetNetworkManagerGameObject().GetComponent<CustomNetworkManager>().parsedMazeJson;
            
        }

        // Disable movement on inactive guards
        if(!CustomNetworkManager.isRunner){
            if(gameObject.GetComponent<ManageActiveCharacters>().guardId != gameObject.GetComponent<ManageActiveCharacters>().activeGuardId){
                canMove = false;
                isRestricted = true;
                animator.SetFloat("Movement Speed", 0.0f);
                rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
            }
            else{
                if(isRestricted == true){
                    canMove = true;
                    isRestricted = false;
                }
                rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }

        // Process character movement
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

            // Sync whether or not a player is moving to the other clients
            if(movementInput.x != 0 || movementInput.y != 0)
            {
                cmd_SetIsMoving(true);
            }
            else
            {
                cmd_SetIsMoving(false);
            }

            // Communicate movement values with the animator controller
            animator.SetFloat("Horizontal Movement", movementInput.x);
            animator.SetFloat("Vertical Movement", movementInput.y);
            animator.SetFloat("Movement Speed", movementInput.sqrMagnitude); // Set the speed to the squared length of the movementInput vector
            animator.SetFloat("Facing Direction", facingDirection);
        }
        
        if(Math.Abs(gameObject.transform.position.x) < (int)(mazeWidth/2) * Utilities.GetCellSize() && Math.Abs(gameObject.transform.position.y) < (int)(mazeHeight/2) * Utilities.GetCellSize()){
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

            // Only manage minimap pieces if mazeData is not null
            if(mazeData != null)
            {
                currentCell  = mazeData[characterCellLocation[0] + (int)(mazeWidth/2), characterCellLocation[1] + (int)(mazeHeight/2)];
                currentCellY = characterCellLocation[1] * Utilities.GetCellSize();
            

                // Manage character arrow display
                if((currentCell.HasFlag(WallStatus.BOTTOM)) && (currentCellY - gameObject.transform.position.y) > 2.3f){
                    characterArrow.GetComponent<SpriteRenderer>().enabled = true;
                }
                else{
                    characterArrow.GetComponent<SpriteRenderer>().enabled = false;
                }
            }
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

            // Handle footstep audio for the local player
            if(movementInput != Vector2.zero && audioSource.isPlaying == false)
            {
                audioSource.Play();
            }
            else if(movementInput == Vector2.zero && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        // Handle footstep audio for the other players (non-local players)
        if(!isLocalPlayer && canMove && isMoving && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else if(!isLocalPlayer && canMove && !isMoving && audioSource.isPlaying)
        {
            audioSource.Stop();
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

    // Required to set isMoving on the server (SyncVars do not sync unless they are set on the server)
    [Command(requiresAuthority = false)]
    public void cmd_SetIsMoving(bool set)
    {
        isMoving = set;
    }
}