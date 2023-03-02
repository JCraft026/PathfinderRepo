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
                        PlaceBarricade();
                    }
                    break;
                case 2f:
                    if(currentCell.HasFlag(WallStatus.LEFT)){
                        Debug.Log("Cell already has a left wall");
                    }
                    else{
                        PlaceBarricade();
                    }
                    break;
                case 3f:
                    if(currentCell.HasFlag(WallStatus.TOP)){
                        Debug.Log("Cell already has a top wall");
                    } 
                    else{
                        PlaceBarricade();
                    }  
                    break;
                case 4f:
                    if(currentCell.HasFlag(WallStatus.RIGHT)){
                        Debug.Log("Cell already has a right wall");
                    }
                    else{
                        PlaceBarricade();
                    }
                    break;
            }
        }
    }

    public void PlaceBarricade(){
        Debug.Log("Tried to place a wall");
        Vector2 scenePosition             = new Vector2(8.0f * (-13.0f / 2 + trapperLocation[0]), 8.0f * (-13.0f / 2 + trapperLocation[1]));
        var topBrickWall                  = Instantiate(barricade, gameObject.transform) as GameObject;
        topBrickWall.transform.position   = scenePosition + new Vector2(0, 8.0f / 1.55f);
        topBrickWall.transform.localScale = new Vector2(topBrickWall.transform.localScale.x * 8.0f, topBrickWall.transform.localScale.y * 8.0f);
        return;
    }
}
