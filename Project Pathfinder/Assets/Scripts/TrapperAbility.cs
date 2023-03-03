using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Mirror;
using System.Text.RegularExpressions;
using System.Linq;

public class TrapperAbility : NetworkBehaviour
{
    public GameObject barricade; //
    private int[] trapperLocation; //
    private WallStatus[,] mazeData; //
    private WallStatus currentCell;

    private MoveCharacter trapperMoveCharacter; //
    private Vector3 placementDirection;
    private Vector3 barricadeLocation;
    private Vector3 placementOrientation;

    void Start(){
        trapperMoveCharacter = gameObject.GetComponent<MoveCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        // When trapper presses "[q]"
        if((Input.GetKeyDown("q") && CustomNetworkManager.isRunner == false && gameObject.GetComponent<ManageActiveCharacters>().guardId == gameObject.GetComponent<ManageActiveCharacters>().activeGuardId)){
            // Get trapper cell location
            trapperLocation = Utilities.GetCharacterCellLocation(ManageActiveCharactersConstants.TRAPPER);
            
            // Get the mazeData as Json text
            string mazeDataJson = CustomNetworkManagerDAO.GetNetworkManagerGameObject().GetComponent<CustomNetworkManager>().mazeRenderer.GiveMazeDataToNetworkManager();
            
            // Convert Json text to maze coordinates
            mazeData = JsonConvert.DeserializeObject<WallStatus[,]>(mazeDataJson);

            // Find the trapper's current cell (add 6 to each coordinate to match the orignal 2D array)
            currentCell = mazeData[trapperLocation[0] + 6, trapperLocation[1] + 6];
            Debug.Log(currentCell);

            barricadeLocation = trapperMoveCharacter.transform.position;
            
            // See if there is already a wall where the Trapper is facing
            switch(trapperMoveCharacter.facingDirection){
                default:
                    Debug.Log("No correct facing direction when trying to place wall in TrapperAbility.cs");
                    break;
                case 1f:
                    if(currentCell.HasFlag(WallStatus.BOTTOM)){
                        Debug.Log("Cell already has a bottom wall");
                    }
                    else{
                        placementDirection = new Vector2(0, -2.5f);
                        placementOrientation = new Vector3(0,0,0);
                        PlaceBarricade();
                    }
                    break;
                case 2f:
                    if(currentCell.HasFlag(WallStatus.LEFT)){
                        Debug.Log("Cell already has a left wall");
                    }
                    else{
                        placementDirection = new Vector2(-2.5f, 0f);
                        placementOrientation = new Vector3(0,0,90);
                        PlaceBarricade();
                    }
                    break;
                case 3f:
                    if(currentCell.HasFlag(WallStatus.TOP)){
                        Debug.Log("Cell already has a top wall");
                    } 
                    else{
                        placementDirection = new Vector2(0f, 2.5f);
                        placementOrientation = new Vector3(0,0,0);
                        PlaceBarricade();
                    }  
                    break;
                case 4f:
                    if(currentCell.HasFlag(WallStatus.RIGHT)){
                        Debug.Log("Cell already has a right wall");
                    }
                    else{
                        placementDirection = new Vector2(2.5f, 0f);
                        placementOrientation = new Vector3(0,0,90);
                        PlaceBarricade();
                    }
                    break;
            }
        }
    }

    [Command]
    public void PlaceBarricade(){
        Debug.Log(trapperMoveCharacter.transform.position);
        Debug.Log("Tried to place a wall");
        GameObject tempBarricade;
        tempBarricade = Instantiate(barricade, trapperMoveCharacter.transform.position + placementDirection, Quaternion.Euler(placementOrientation));
        NetworkServer.Spawn(tempBarricade);
        return;
    }
}
