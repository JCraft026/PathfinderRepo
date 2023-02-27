using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json; //Needed for serializing/deserializing the maze sent to the client
using UnityEngine;
using Mirror;
using System.Linq;

public class RenderMaze : NetworkBehaviour
{
    // Initialize fields on the inspector
    [SerializeField]
    [Range(1, 50)]
    public int mazeWidth = 10;

    [SerializeField]
    [Range(1, 50)]
    public int mazeHeight = 10;

    [SerializeField]
    [Range(1, 20)]
    public int mossyWallSpawnChance = 10;

    [SerializeField]
    private Transform brickWallPrefab = null;

    [SerializeField]
    private Transform mossyWallPrefab = null;

    [SerializeField]
    private Transform torchWallPrefab = null;

    [SerializeField]
    private Transform sideWallPrefab = null;

    [SerializeField]
    private Transform crackedWallPrefab = null;

    [SerializeField]
    private Transform floorPrefab = null;

    [SerializeField]
    private Transform exit1LeftRightClosed = null;

    //[SerializeField]
    //private Transform exit1LeftRightOpen = null;

    [SerializeField]
    private Transform exit1TopBottomClosed = null;

    //[SerializeField]
    //private Transform exit1TopBottomOpen = null;

    [SerializeField]
    private Transform exit2LeftRightClosed = null;

    //[SerializeField]
    //private Transform exit2LeftRightOpen = null;

    [SerializeField]
    private Transform exit2TopBottomClosed = null;

    //[SerializeField]
    //private Transform exit2TopBottomOpen = null;

    [SerializeField]
    private Transform exit3LeftRightClosed = null;

    //[SerializeField]
    //private Transform exit3LeftRightOpen = null;

    [SerializeField]
    private Transform exit3TopBottomClosed = null;

    //[SerializeField]
    //private Transform exit3TopBottomOpen = null;

    [SerializeField]
    private Transform exit4LeftRightClosed = null;

    //[SerializeField]
    //private Transform exit4LeftRightOpen = null;

    [SerializeField]
    private Transform exit4TopBottomClosed = null;

    //[SerializeField]
    //private Transform exit4TopBottomOpen = null;

    private string mazeDataJson;                                   // Json string version of the maze (used to send the maze to the client)
    private List<Transform> oldComponents = new List<Transform>(); // List of wall locations last rendered
    public float cellSize = 8f;                                    // Size of the maze cell

    // Called when the host starts a game
    public void CreateMaze()
    {
        // Get the generated maze data
        WallStatus[,] mazeData = GenerateMaze.Generate(mazeWidth, mazeHeight);
        mazeDataJson = JsonConvert.SerializeObject(mazeData);

        // Clean up left over walls and such from the last game
        CleanMap();

        // Render the maze in the scene
        Render(mazeData);
    }

    // Cleans the left over objects from the last game
    public void CleanMap()
    {
        oldComponents.ForEach(wall => GameObject.Destroy(wall.gameObject));
        oldComponents = new();
    }

    // Render the complete maze within the scene
    public void Render(WallStatus[,] mazeData)
    {
        WallStatus currentCell  = new WallStatus();    // Current maze cell being rendered
        Vector2 scenePosition   = new Vector2();       // x,y position in the scene
        Transform exitPrefab    = null;                // Exit prefab being rendered
        System.Random randomNum = new System.Random(); // Random number generator
        Transform topWallPrefab;                       // Object prefab for selected top wall
        int currentExit         = 1;                   // Next exit prefab to render

        // Render the cell walls of every maze cell
        for (int j = 0; j < mazeHeight; j++){
            for (int i = 0; i < mazeWidth; i++){
                currentCell = mazeData[i,j];
                scenePosition = new Vector2(cellSize * (-mazeWidth / 2 + i), cellSize * (-mazeHeight / 2 + j));

                // Render the cell floor
                var cellFloor        = Instantiate(floorPrefab, transform);
                cellFloor.localScale = new Vector2(cellSize, cellSize);
                cellFloor.position   = scenePosition;
                cellFloor.name       = "mcf(" + (i-(int)(mazeWidth/2)) + "," + (j-(int)(mazeHeight/2)) + ")";
                oldComponents.Add(cellFloor);

                // Render the top wall of a maze cell
                if(currentCell.HasFlag(WallStatus.TOP)){
                    if(currentCell.HasFlag(WallStatus.EXIT) && j == mazeHeight-1){
                        switch (currentExit)
                        {
                            case 1:
                                exitPrefab = exit1TopBottomClosed;
                                break;
                            case 2:
                                exitPrefab = exit2TopBottomClosed;
                                break;
                            case 3:
                                exitPrefab = exit3TopBottomClosed;
                                break;
                            case 4:
                                exitPrefab = exit4TopBottomClosed;
                                break;
                        }
                        var topExit        = Instantiate(exitPrefab, transform) as Transform;
                        topExit.position   = scenePosition + new Vector2(0, cellSize / 1.55f);
                        topExit.localScale = new Vector2(topExit.localScale.x * cellSize, topExit.localScale.y * cellSize);
                        currentExit++;
                        oldComponents.Add(topExit);
                    }
                    else if(currentCell.HasFlag(WallStatus.TOP_CRACKED) && j != mazeHeight-1){
                        var topWall        = Instantiate(crackedWallPrefab, transform) as Transform;
                        topWall.position   = scenePosition + new Vector2(0, cellSize / 1.55f);
                        topWall.localScale = new Vector2(topWall.localScale.x * cellSize, topWall.localScale.y * cellSize);
                        topWall.name       = "Wall_TB"; 
                        oldComponents.Add(topWall); //DEBUG: MAKE SURE THIS MATCHES PROD
                    }
                    else{
                        if(i % 3 == 0){
                            topWallPrefab = torchWallPrefab;
                        }
                        else if(randomNum.Next(1, mossyWallSpawnChance) == 1){
                            topWallPrefab = mossyWallPrefab;
                        }
                        else{
                            topWallPrefab = brickWallPrefab;
                        }
                        var topBrickWall        = Instantiate(topWallPrefab, transform) as Transform;
                        topBrickWall.position   = scenePosition + new Vector2(0, cellSize / 1.55f);
                        topBrickWall.localScale = new Vector2(topBrickWall.localScale.x * cellSize, topBrickWall.localScale.y * cellSize);
                        topBrickWall.name       = "Wall_TB"; 
                        oldComponents.Add(topBrickWall);
                    }
                }

                // Render the left wall of a maze cell
                if(currentCell.HasFlag(WallStatus.LEFT)){
                    if(currentCell.HasFlag(WallStatus.EXIT) && i == 0){
                        switch (currentExit)
                        {
                            case 1:
                                exitPrefab = exit1LeftRightClosed;
                                break;
                            case 2:
                                exitPrefab = exit2LeftRightClosed;
                                break;
                            case 3:
                                exitPrefab = exit3LeftRightClosed;
                                break;
                            case 4:
                                exitPrefab = exit4LeftRightClosed;
                                break;
                        }
                        var leftExit         = Instantiate(exitPrefab, transform) as Transform;
                        leftExit.position    = scenePosition + new Vector2(-cellSize / 2, 0);
                        leftExit.localScale  = new Vector2(leftExit.localScale.x * cellSize, leftExit.localScale.y * cellSize);
                        leftExit.eulerAngles = new Vector3(0, 180, 90);
                        currentExit++;
                        oldComponents.Add(leftExit);
                    }
                    else if(currentCell.HasFlag(WallStatus.LEFT_CRACKED) && i != 0){
                        var leftWall         = Instantiate(crackedWallPrefab, transform) as Transform;
                        leftWall.position    = scenePosition + new Vector2(-cellSize / 2, 0);
                        leftWall.localScale  = new Vector2(cellSize, leftWall.localScale.y);
                        leftWall.eulerAngles = new Vector3(0, 180, 90);
                        oldComponents.Add(leftWall);
                    }
                    else{
                        // Spawn the left wall
                        var leftWall         = Instantiate(sideWallPrefab, transform) as Transform;
                        leftWall.position    = scenePosition + new Vector2(-cellSize / 2, 0);
                        leftWall.localScale  = new Vector2(leftWall.localScale.x * cellSize, leftWall.localScale.y * cellSize);
                        leftWall.eulerAngles = new Vector3(0, 180, 90);
                        leftWall.name        = "Wall_LR"; 
                        oldComponents.Add(leftWall);
                    }
                }

                // Render the bottom wall of a maze cell if the current cell is in the bottom row
                if(j == 0){
                    if (currentCell.HasFlag(WallStatus.BOTTOM))
                    {
                        if(currentCell.HasFlag(WallStatus.EXIT)){
                            switch (currentExit)
                            {
                                case 1:
                                    exitPrefab = exit1TopBottomClosed;
                                    break;
                                case 2:
                                    exitPrefab = exit2TopBottomClosed;
                                    break;
                                case 3:
                                    exitPrefab = exit3TopBottomClosed;
                                    break;
                                case 4:
                                    exitPrefab = exit4TopBottomClosed;
                                    break;
                            }
                            var bottomExit        = Instantiate(exitPrefab, transform) as Transform;
                            bottomExit.position   = scenePosition + new Vector2(0, -cellSize / 2.9f);
                            bottomExit.localScale = new Vector2(bottomExit.localScale.x * cellSize, bottomExit.localScale.y * cellSize);
                            currentExit++;
                            oldComponents.Add(bottomExit);
                        }
                        else{
                            var bottomBrickWall        = Instantiate(brickWallPrefab, transform) as Transform;
                            bottomBrickWall.position   = scenePosition + new Vector2(0, -cellSize / 2.9f);
                            bottomBrickWall.localScale = new Vector2(bottomBrickWall.localScale.x * cellSize, bottomBrickWall.localScale.y * cellSize);
                            bottomBrickWall.name       = "Wall_TB"; 
                            oldComponents.Add(bottomBrickWall);
                        }
                    }
                }

                // Render the right wall of a maze cell if the current cell is in the right most column
                if(i == mazeWidth - 1){
                    if (currentCell.HasFlag(WallStatus.RIGHT))
                    {
                        if(currentCell.HasFlag(WallStatus.EXIT)){
                            switch (currentExit)
                            {
                                case 1:
                                    exitPrefab = exit1LeftRightClosed;
                                    break;
                                case 2:
                                    exitPrefab = exit2LeftRightClosed;
                                    break;
                                case 3:
                                    exitPrefab = exit3LeftRightClosed;
                                    break;
                                case 4:
                                    exitPrefab = exit4LeftRightClosed;
                                    break;
                            }
                            var rightExit         = Instantiate(exitPrefab, transform) as Transform;
                            rightExit.position    = scenePosition + new Vector2(+cellSize / 2, 0);
                            rightExit.localScale  = new Vector2(rightExit.localScale.x * cellSize, rightExit.localScale.y * cellSize);
                            rightExit.eulerAngles = new Vector3(0, 180, 90);
                            currentExit++;
                            oldComponents.Add(rightExit);
                        }
                        else{
                            // Spawn the right wall
                            var rightWall         = Instantiate(sideWallPrefab, transform) as Transform;
                            rightWall.position    = scenePosition + new Vector2(+cellSize / 2, 0);
                            rightWall.localScale  = new Vector3(rightWall.localScale.x * cellSize, rightWall.localScale.y * cellSize, 1);
                            rightWall.eulerAngles = new Vector3(0, 180, 90);
                            rightWall.name        = "Wall_LR"; 
                            oldComponents.Add(rightWall);
                        }
                    }
                }
            }
        }

        // Render the minimap in the canvas
        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("MiniMapHandler")).GetComponent<RenderMiniMap>().Render(mazeData);
    }

    // Used by the network manager to get the maze json string
    public string GiveMazeDataToNetworkManager()
    {
        return mazeDataJson;
    }

    // Used by ManageCrackedWalls to adjust the detection size for wall breaking
    public float GetCellSize(){
        return cellSize;
    }
}