using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Mirror;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine.SceneManagement;

public class EngineerAbility : NetworkBehaviour
{
    public GameObject barricadeHorizontal; //
    public GameObject barricadeVertical; //
    private int[] engineerLocation; //
    // private WallStatus[,] mazeData; //
    private WallStatus currentCell;

    private MoveCharacter engineerMoveCharacter; //
    private Vector3 placementDirection;
    private Vector3 barricadeLocation;
    private Vector3 placementOrientation;
    private float   scaler = 6.9f;
    public int barricadeCount = 0; //
    CustomNetworkManager customNetworkManager;

    void Start(){
        engineerMoveCharacter = gameObject.GetComponent<MoveCharacter>();

        // Get the mazeData as Json text
        string mazeDataJson = CustomNetworkManagerDAO.GetNetworkManagerGameObject().GetComponent<CustomNetworkManager>().mazeRenderer.GiveMazeDataToNetworkManager();

        // Convert Json text to maze coordinates
        // List<GameObject> walls = Resources.FindObjectsOfTypeAll<GameObject>()
        //     .Where<GameObject>(x => x.name.Contains("Wall") || x.name.Contains("Exit ")).ToList();
        customNetworkManager = CustomNetworkManagerDAO.GetNetworkManagerGameObject().GetComponent<CustomNetworkManager>();
        if(customNetworkManager.parsedMazeJson == null){
            Debug.LogError("Parsed Maze Data is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // When engineer presses "[q]"
        if((Input.GetKeyDown("q") && CustomNetworkManager.isRunner == false && gameObject.GetComponent<ManageActiveCharacters>().guardId == gameObject.GetComponent<ManageActiveCharacters>().activeGuardId) && barricadeCount < 3){
            // Get engineer cell location
            engineerLocation = Utilities.GetCharacterCellLocation(ManageActiveCharactersConstants.ENGINEER);
            
            //tEST FOR NULLNESS
            if(customNetworkManager.parsedMazeJson == null){
                Debug.LogError("Parsed Maze Data is null");
            }

            // Find the engineer's current cell (add 6 to each coordinate to match the orignal 2D array)
            currentCell = customNetworkManager.parsedMazeJson[engineerLocation[0] + 6, engineerLocation[1] + 6];
            Debug.Log(currentCell);

            // Assign the barricade location to the engineer as default
            barricadeLocation = engineerMoveCharacter.transform.position;
            
            // See if there is already a wall where the Engineer is facing
            switch(engineerMoveCharacter.facingDirection){
                default:
                    Debug.Log("No correct facing direction when trying to place wall in EngineerAbility.cs");
                    break;
                case 1f:
                    if(currentCell.HasFlag(WallStatus.BOTTOM)){
                        Debug.Log("Cell already has a bottom wall");
                    }
                    else{
                        placementDirection   = new Vector2(0, -2.5f);
                        placementOrientation = new Vector3(0,0,0);
                        barricadeLocation    = new Vector2(engineerLocation[0] * 8.0f, engineerMoveCharacter.transform.position.y);
                        scaler               = 6.9f;
                        PlaceBarricade(1, placementDirection.x, placementDirection.y,
                               placementOrientation.x, placementOrientation.y, placementOrientation.z,
                               barricadeLocation.x, barricadeLocation.y, scaler); // Flag of 1 to spawn Horizontal version
                    }
                    break;
                case 2f:
                    if(currentCell.HasFlag(WallStatus.LEFT)){
                        Debug.Log("Cell already has a left wall");
                    }
                    else{
                        placementDirection   = new Vector2(-2.5f, 0f);
                        placementOrientation = new Vector3(0,0,90);
                        barricadeLocation    = new Vector2(engineerMoveCharacter.transform.position.x, engineerLocation[1] * 8.0f);
                        scaler               = 7.65f;
                        PlaceBarricade(0, placementDirection.x, placementDirection.y,
                               placementOrientation.x, placementOrientation.y, placementOrientation.z,
                               barricadeLocation.x, barricadeLocation.y, scaler); // Flag of 0 to spawn Vertical version
                    }
                    break;
                case 3f:
                    if(currentCell.HasFlag(WallStatus.TOP)){
                        Debug.Log("Cell already has a top wall");
                    } 
                    else{
                        placementDirection   = new Vector2(0f, 2.5f);
                        placementOrientation = new Vector3(0,0,0);
                        barricadeLocation    = new Vector2(engineerLocation[0] * 8.0f, engineerMoveCharacter.transform.position.y);
                        scaler               = 6.9f;
                       PlaceBarricade(1, placementDirection.x, placementDirection.y,
                               placementOrientation.x, placementOrientation.y, placementOrientation.z,
                               barricadeLocation.x, barricadeLocation.y, scaler); // Flag of 1 to spawn Horizontal version
                    }  
                    break;
                case 4f:
                    if(currentCell.HasFlag(WallStatus.RIGHT)){
                        Debug.Log("Cell already has a right wall");
                    }
                    else{
                        placementDirection   = new Vector2(2.5f, 0f);
                        placementOrientation = new Vector3(0,0,90);
                        barricadeLocation    = new Vector2(engineerMoveCharacter.transform.position.x, engineerLocation[1] * 8.0f);
                        scaler               = 7.65f;
                        PlaceBarricade(0, placementDirection.x, placementDirection.y,
                               placementOrientation.x, placementOrientation.y, placementOrientation.z,
                               barricadeLocation.x, barricadeLocation.y, scaler); // Flag of 0 to spawn Vertical version
                    }
                    break;
            }
        }
        else if((Input.GetKeyDown("q") && CustomNetworkManager.isRunner == false && gameObject.GetComponent<ManageActiveCharacters>().guardId == gameObject.GetComponent<ManageActiveCharacters>().activeGuardId) && barricadeCount >= 3){
            Debug.Log("You're at the max number of walls (3)");
        }
    }

    [Command]
    public void PlaceBarricade(int axis, float placementDirectionX, float placementDirectionY,
                               float placementOrientationX, float placementOrientationY, float placementOrientationZ,
                               float barricadeLocationX, float barricadeLocationY, float scaler){
        Debug.Log("Tried to place a wall");
        Debug.Log("PlaceBarricade: (Placement direction, barricade location) (" + placementDirectionX + ", " + placementDirectionY + ") (" + barricadeLocationX + ", " + barricadeLocationY + ")");
        GameObject tempBarricade;
        if(axis == 1){
            tempBarricade = Instantiate(barricadeHorizontal, new Vector3(barricadeLocationX, barricadeLocationY, 0) + 
               new Vector3(placementDirectionX, placementDirectionY, 0), 
               Quaternion.Euler(new Vector3(placementOrientationX, placementOrientationY, placementOrientationZ)));
            tempBarricade.transform.localScale = new Vector2(tempBarricade.transform.localScale.x * scaler, tempBarricade.transform.localScale.y * scaler);
        }
        else{
            tempBarricade = Instantiate(barricadeVertical, new Vector3(barricadeLocationX, barricadeLocationY, 0) + 
               new Vector3(placementDirectionX, placementDirectionY, 0), 
               Quaternion.Euler(new Vector3(placementOrientationX, placementOrientationY, placementOrientationZ)));
            tempBarricade.transform.localScale = new Vector2(tempBarricade.transform.localScale.x * scaler, tempBarricade.transform.localScale.y * scaler);
        }
        NetworkServer.Spawn(tempBarricade);
        barricadeCount += 1;
        return;
    }

    public void decreseBarricadeCount(){
        barricadeCount -= 1;
    }
}
