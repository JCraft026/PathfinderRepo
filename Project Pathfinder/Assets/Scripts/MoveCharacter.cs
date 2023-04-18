using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using Mirror;
using System.Linq;
using Newtonsoft.Json;
using System;
using TMPro;

static class MoveCharacterConstants{
    public const float FORWARD  = 1f; // Character facing forward
    public const float LEFT     = 2f; // Character facing left
    public const float BACKWARD = 3f; // Character facing backward
    public const float RIGHT    = 4f; // Character facing right
}

public class MoveCharacter : NetworkBehaviour
{
    [SerializeField]
    private Transform guardRebootCountdownPrefab = null;     // Countdown Prefab until guard restarts from EMP disabling
    public GameObject flashlight;                            // Character's flashlight object (if they have one)
    public GameObject selflight;                             // Character's selflight object (if they have one)
    public float moveSpeed = 5f;                             // Speed at which the character needs to move
    public float facingDirection;                            // Direction the character should face after movement
    public Vector2 movementInput;                            // Character's current input direction             
    public Rigidbody2D rigidBody;                            // Character's RigidBody
    public Animator animator;                                // Character's animator manager
    public bool canMove = true;                              // Character movement lock status
    public bool isDisabled = false;                          // Character disabled by an EMP
    public GameObject PauseCanvas;                           // Exit game menu
    public bool isRestricted = true;                         // Status of parent guard objects movement restricted
    private GameObject characterArrow;                       // Arrow of the current active character
    private float mazeWidth = 13;                            // Width of the maze
    private float mazeHeight = 13;                           // Height of the maze
    public WallStatus[,] mazeData = new WallStatus[13, 13];  // Maze data
    private WallStatus currentCell;                          // Wall status of the cell the parent character object is in
    private float currentCellY;                              // Y position of the current cell
    private float currentCellX;                              // X position of the current cell
    private int[] characterCellLocation = new int[2];        // Cell location of the current character
    private int activeCharacterCode;                         // Code identifying the current active character
    public AudioSource audioSource;                          // Makes the player make sounds
    [SyncVar]
    public bool isMoving; // Keeps track of if the player is moving, this way the client can play the correct audio
    private Player_UI playerUi;                              // Imports the Player's UI to access what is the player
    public static MoveCharacter Instance;                    // Makes an instance of this class to access 
    Regex runnerExpression = new Regex("Runner");            // Match "Runner" 
    private string disabledPopupText;                        // Text displayed on guard disabled
    private float disabledTimeLeft = 30.5f;                  // Time left for the guard to be disabled
    Transform guardRebootCountdown;                          // Countdown until guard restarts from EMP disabling
    private bool rebootCountdownActive = false;       // Status of reboot countdown being spawned
    
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
        if(!runnerExpression.IsMatch(gameObject.name)){
            if(gameObject.GetComponent<ManageActiveCharacters>().guardId != gameObject.GetComponent<ManageActiveCharacters>().activeGuardId){
                canMove = false;
                isRestricted = true;
                animator.SetFloat("Movement Speed", 0.0f);
                rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
            }
            else{
                if(isRestricted == true && isDisabled == false){
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
        
        if(Math.Abs(gameObject.transform.position.x) < (mazeWidth/2) * Utilities.GetCellSize() && Math.Abs(gameObject.transform.position.y) < (mazeHeight/2) * Utilities.GetCellSize()){
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

        // Show and update guard disabled countdown
        if(!CustomNetworkManager.isRunner){
            if(isDisabled){
                if(!rebootCountdownActive){
                    guardRebootCountdown = Instantiate(guardRebootCountdownPrefab, transform);
                    guardRebootCountdown.transform.SetParent(GameObject.Find("Minimap").transform, false);
                    rebootCountdownActive = true;
                }
                if(rebootCountdownActive){
                    if(gameObject.GetComponent<ManageActiveCharacters>().guardId == gameObject.GetComponent<ManageActiveCharacters>().activeGuardId){
                        guardRebootCountdown.gameObject.SetActive(true);
                    }
                    else{
                        guardRebootCountdown.gameObject.SetActive(false);
                    }
                    guardRebootCountdown.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = ((int)disabledTimeLeft).ToString();
                    if(disabledTimeLeft > 0){
                        disabledTimeLeft -= Time.deltaTime;
                    }
                    else{
                        disabledTimeLeft    = 30.5f;
                    }
                }
            }
            else{
                if(rebootCountdownActive){
                    Destroy(guardRebootCountdown.gameObject);
                    rebootCountdownActive = false;
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

    bool IsNearBottomWall(){
        bool nearBottomWall = false;
        Regex tbWallExpression = new Regex("TB"); // Match top and bottom walls
        Collider2D[] nearByObjects = Physics2D.OverlapCircleAll(gameObject.transform.position, 5f);

        foreach(var nearByObject in nearByObjects){
            if(tbWallExpression.IsMatch(nearByObject.gameObject.name) && nearByObject.transform.position.y < (gameObject.transform.position.y + 1) && nearByObject.transform.position.x == currentCellX){
                nearBottomWall = true;
            }
        }

        return nearBottomWall;
    }

    public void startDisableGuard(){
        StartCoroutine(disableGuard());
    }

    IEnumerator disableGuard(){
        Vector3 currentFlashlightPos = new Vector3(); // Retain the Location of the Light before moving it
        Vector3 currentSelflightPos  = new Vector3(); // Retain the Location of the Light before moving it
        
        // Disable movement, sight, and show electricity particles
        canMove = false;
        isDisabled = true;
        gameObject.GetComponent<Attack>().enabled = false;

        // Disable Guard abilities based on which guard you are
        switch(gameObject.name){
            case "Chaser(Clone)":
                gameObject.GetComponent<ChaserAbility>().enabled = false;
                break;
            case "Trapper(Clone)":
                gameObject.GetComponent<TrapperAbility>().enabled = false;
                break;
            case "Engineer(Clone)":
                gameObject.GetComponent<EngineerAbility>().enabled = false;
                break;
            default:
                Debug.LogError("Guard Type " + gameObject.name + " does not exist");
                break;
        }

        if(flashlight != null){
            // Store and move all light sources away from guard
            currentFlashlightPos = flashlight.transform.position;
            flashlight.transform.position = new Vector3(-100, 0,0);
            currentSelflightPos = selflight.transform.position;
            selflight.transform.position  = new Vector3(-100, 0,0);
        }
        else{
            // Store and move all light sources away from guard
            currentSelflightPos = selflight.transform.position;
            selflight.transform.position = new Vector3(-100, 0,0);
        }

        // Send Popup message that the guard is disabled
        if(CustomNetworkManager.isRunner == false){
            switch (gameObject.GetComponent<ManageActiveCharacters>().guardId)
            {
                case ManageActiveCharactersConstants.CHASER:
                    disabledPopupText = "<color=#03fc52>Chaser</color> <color=red>disabled by EMP</color>";
                    GameObject.Find("ChaIcon").GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
                    break;
                case ManageActiveCharactersConstants.ENGINEER:
                    disabledPopupText = "<color=#fcba03>Engineer</color> <color=red>disabled by EMP</color>";
                    GameObject.Find("EngIcon").GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
                    break;
                case ManageActiveCharactersConstants.TRAPPER:
                    disabledPopupText = "<color=#0373fc>Trapper</color> <color=red>disabled by EMP</color>";
                    GameObject.Find("TraIcon").GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
                    break;
            }
            GameObject.Find("PopupMessageManager").GetComponent<ManagePopups>().ProcessPopup(disabledPopupText, 5f);
        }
        
        // Wait 5 seconds
        yield return new WaitForSeconds(30);

        // Take off the diabled effect, move the light sources back to the guard, and enable movement
        gameObject.GetComponentsInChildren<SpriteRenderer>()
            .FirstOrDefault<SpriteRenderer>(x => x.gameObject.name == "DisableEffect").enabled = false;
        if(flashlight != null){
            flashlight.transform.position = currentFlashlightPos;
            selflight.transform.position  = currentSelflightPos;
        }
        else{
            selflight.transform.position  = currentSelflightPos;
        }
        canMove = true;
        isDisabled = false;
        gameObject.GetComponent<Attack>().enabled = true;
        
        // Enable Guard abilities based on which guard you are
        switch(gameObject.name){
            case "Chaser(Clone)":
                gameObject.GetComponent<ChaserAbility>().enabled = true;
                break;
            case "Trapper(Clone)":
                gameObject.GetComponent<TrapperAbility>().enabled = true;
                break;
            case "Engineer(Clone)":
                gameObject.GetComponent<EngineerAbility>().enabled = true;
                break;
            default:
                Debug.LogError("Guard Type " + gameObject.name + " does not exist");
                break;
        }

        // Change the color of the disabled guard minimap icon back to normal
        // Send Popup message that the guard is disabled
        if(CustomNetworkManager.isRunner == false){
            switch (gameObject.GetComponent<ManageActiveCharacters>().guardId)
            {
                case ManageActiveCharactersConstants.CHASER:
                    GameObject.Find("ChaIcon").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                    break;
                case ManageActiveCharactersConstants.ENGINEER:
                    GameObject.Find("EngIcon").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                    break;
                case ManageActiveCharactersConstants.TRAPPER:
                    GameObject.Find("TraIcon").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                    break;
            }
        }
    }

    // Required to set isMoving on the server (SyncVars do not sync unless they are set on the server)
    [Command(requiresAuthority = false)]
    public void cmd_SetIsMoving(bool set)
    {
        isMoving = set;
    }
}