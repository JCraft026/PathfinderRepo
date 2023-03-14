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
    private Transform leftCrackedWallPrefab = null;

    [SerializeField]
    private Transform floorPrefab = null;

    [SerializeField]
    private Transform exit1LeftRightClosed = null;

    [SerializeField]
    private Transform exit1LeftRightOpen = null;

    [SerializeField]
    private Transform exit1TopBottomClosed = null;

    [SerializeField]
    private Transform exit1TopBottomOpen = null;

    [SerializeField]
    private Transform exit2LeftRightClosed = null;

    [SerializeField]
    private Transform exit2LeftRightOpen = null;

    [SerializeField]
    private Transform exit2TopBottomClosed = null;

    [SerializeField]
    private Transform exit2TopBottomOpen = null;

    [SerializeField]
    private Transform exit3LeftRightClosed = null;

    [SerializeField]
    private Transform exit3LeftRightOpen = null;

    [SerializeField]
    private Transform exit3TopBottomClosed = null;

    [SerializeField]
    private Transform exit3TopBottomOpen = null;

    [SerializeField]
    private Transform exit4LeftRightClosed = null;

    [SerializeField]
    private Transform exit4LeftRightOpen = null;

    [SerializeField]
    private Transform exit4TopBottomClosed = null;

    [SerializeField]
    private Transform exit4TopBottomOpen = null;

    [SerializeField]
    private Transform tunnelEntrance = null;

    [SerializeField]
    private Transform chaserFastTravelIdle = null;

    [SerializeField]
    private Transform engineerFastTravelIdle = null;

    [SerializeField]
    private Transform trapperFastTravelIdle = null;

    [SerializeField]
    private Transform controlRoomPrefab = null;

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
        WallStatus currentCell    = new WallStatus();    // Current maze cell being rendered
        Vector2 scenePosition     = new Vector2();       // x,y position in the scene
        Transform exitPrefab      = null;                // Exit prefab being rendered
        Transform openExitPrefab  = null;                // Opened exit prefab being rendered
        System.Random randomNum   = new System.Random(); // Random number generator
        Transform topWallPrefab;                         // Object prefab for selected top wall
        int currentExit           = 1,                   // Next exit prefab to render
        tunnelEntranceHeightIndex = (mazeHeight/2),      // j index for the cell where the tunnel entrance will spawn
        tunnelEntranceWidthIndex  = (mazeWidth/2);       // i index for the cell where the tunnel entrance will spawn
        string exitName           = null;                // Name of the exit object being rendered

        // Render the maze control room
        var controlRoom = Instantiate(controlRoomPrefab, transform);
        controlRoom.position = new Vector2(cellSize * mazeWidth, cellSize * mazeHeight);
        controlRoom.name = "Control Room";

        // Render the Fast Travel Idle zones
        var chaserTravelIdle = Instantiate(chaserFastTravelIdle, transform);
        chaserTravelIdle.position = new Vector2(cellSize * -mazeWidth, cellSize * mazeHeight);
        chaserTravelIdle.name = "TravelIdleC";
        var engineerTravelIdle = Instantiate(engineerFastTravelIdle, transform);
        engineerTravelIdle.position = new Vector2(cellSize * -mazeWidth, 0);
        engineerTravelIdle.name = "TravelIdleE";
        var trapperTravelIdle = Instantiate(trapperFastTravelIdle, transform);
        trapperTravelIdle.position = new Vector2(cellSize * -mazeWidth, cellSize * -mazeHeight);
        trapperTravelIdle.name = "TravelIdleT";


        // Render the components of every maze cell
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

                // Render the Control Room Entrance
                if(j == tunnelEntranceHeightIndex && i == tunnelEntranceWidthIndex){
                    if(currentCell.HasFlag(WallStatus.TOP)){
                        var controlRoomEntrance        = Instantiate(tunnelEntrance, transform) as Transform;
                        controlRoomEntrance.position   = scenePosition + new Vector2(0, cellSize / 1.68f);
                        controlRoomEntrance.localScale = new Vector2(cellSize * 2, cellSize * 2);
                        controlRoomEntrance.name       = "Tunnel_Entrance"; 
                        oldComponents.Add(controlRoomEntrance);
                    }
                    else{
                        tunnelEntranceHeightIndex++;
                    }
                }

                // Render the top wall of a maze cell
                if(currentCell.HasFlag(WallStatus.TOP)){
                    if(currentCell.HasFlag(WallStatus.EXIT) && j == mazeHeight-1){
                        switch (currentExit)
                        {
                            case 1:
                                exitPrefab     = exit1TopBottomClosed;
                                openExitPrefab = exit1TopBottomOpen;
                                exitName       = "Exit1";
                                break;
                            case 2:
                                exitPrefab     = exit2TopBottomClosed;
                                openExitPrefab = exit2TopBottomOpen;
                                exitName       = "Exit2";
                                break;
                            case 3:
                                exitPrefab     = exit3TopBottomClosed;
                                openExitPrefab = exit3TopBottomOpen;
                                exitName       = "Exit3";
                                break;
                            case 4:
                                exitPrefab     = exit4TopBottomClosed;
                                openExitPrefab = exit4TopBottomOpen;
                                exitName       = "Exit4";
                                break;
                        }
                        var topExit        = Instantiate(exitPrefab, transform) as Transform;
                        topExit.position   = scenePosition + new Vector2(0, cellSize / 1.55f);
                        topExit.localScale = new Vector2(topExit.localScale.x * cellSize, topExit.localScale.y * cellSize);
                        topExit.name       = exitName + "(Close)";
                        currentExit++;
                        oldComponents.Add(topExit);
                        var topExitOpen        = Instantiate(openExitPrefab, transform) as Transform;
                        topExitOpen.position   = scenePosition + new Vector2(0, cellSize / 1.55f);
                        topExitOpen.localScale = new Vector2(topExitOpen.localScale.x * cellSize, topExitOpen.localScale.y * cellSize);
                        topExitOpen.name       = exitName + "(Open)";
                        oldComponents.Add(topExitOpen);
                    }
                    else if(currentCell.HasFlag(WallStatus.TOP_CRACKED) && j != mazeHeight-1 && j != tunnelEntranceHeightIndex && i != tunnelEntranceWidthIndex){
                        var topWall        = Instantiate(crackedWallPrefab, transform) as Transform;
                        topWall.position   = scenePosition + new Vector2(0, cellSize / 1.55f);
                        topWall.localScale = new Vector2(topWall.localScale.x * cellSize, topWall.localScale.y * cellSize);
                        topWall.name       = "Wall_TB cracked"; 
                        oldComponents.Add(topWall); //DEBUG: MAKE SURE THIS MATCHES PROD
                    }
                    else{
                        if(i % 3 == 0 && (j != tunnelEntranceHeightIndex ||  i != tunnelEntranceWidthIndex)){
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
                                exitPrefab     = exit1LeftRightClosed;
                                openExitPrefab = exit1LeftRightOpen;
                                exitName       = "Exit1";
                                break;
                            case 2:
                                exitPrefab     = exit2LeftRightClosed;
                                openExitPrefab = exit2LeftRightOpen;
                                exitName       = "Exit2";
                                break;
                            case 3:
                                exitPrefab     = exit3LeftRightClosed;
                                openExitPrefab = exit3LeftRightOpen;
                                exitName       = "Exit3";
                                break;
                            case 4:
                                exitPrefab     = exit4LeftRightClosed;
                                openExitPrefab = exit4LeftRightOpen;
                                exitName       = "Exit4";
                                break;
                        }
                        var leftExit         = Instantiate(exitPrefab, transform) as Transform;
                        leftExit.position    = scenePosition + new Vector2(-cellSize / 2, 0);
                        leftExit.localScale  = new Vector2(leftExit.localScale.x * cellSize, leftExit.localScale.y * cellSize);
                        leftExit.eulerAngles = new Vector3(0, 180, 90);
                        leftExit.name        = exitName + "(Close)";
                        currentExit++;
                        oldComponents.Add(leftExit);
                        var leftExitOpen         = Instantiate(openExitPrefab, transform) as Transform;
                        leftExitOpen.position    = scenePosition + new Vector2(-cellSize / 2, 0);
                        leftExitOpen.localScale  = new Vector2(leftExitOpen.localScale.x * cellSize, leftExitOpen.localScale.y * cellSize);
                        leftExitOpen.eulerAngles = new Vector3(0, 180, 90);
                        leftExitOpen.name        = exitName + "(Open)";
                        oldComponents.Add(leftExitOpen);
                    }
                    else if(currentCell.HasFlag(WallStatus.LEFT_CRACKED) && i != 0){
                        var leftWall         = Instantiate(leftCrackedWallPrefab, transform) as Transform;
                        leftWall.position    = scenePosition + new Vector2(-cellSize / 2, 0);
                        leftWall.localScale  = new Vector2(2, 2);
                        leftWall.eulerAngles = new Vector3(0, 180, 90);
                        leftWall.name        = "Wall_LR cracked"; 
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
                                    exitPrefab     = exit1TopBottomClosed;
                                    openExitPrefab = exit1TopBottomOpen;
                                    exitName       = "Exit1";
                                    break;
                                case 2:
                                    exitPrefab     = exit2TopBottomClosed;
                                    openExitPrefab = exit2TopBottomOpen;
                                    exitName       = "Exit2";
                                    break;
                                case 3:
                                    exitPrefab     = exit3TopBottomClosed;
                                    openExitPrefab = exit3TopBottomOpen;
                                    exitName       = "Exit3";
                                    break;
                                case 4:
                                    exitPrefab     = exit4TopBottomClosed;
                                    openExitPrefab = exit4TopBottomOpen;
                                    exitName       = "Exit4";
                                    break;
                            }
                            var bottomExit        = Instantiate(exitPrefab, transform) as Transform;
                            bottomExit.position   = scenePosition + new Vector2(0, -cellSize / 2.9f);
                            bottomExit.localScale = new Vector2(bottomExit.localScale.x * cellSize, bottomExit.localScale.y * cellSize);
                            bottomExit.name       = exitName + "(Close)";
                            currentExit++;
                            oldComponents.Add(bottomExit);
                            var bottomExitOpen        = Instantiate(openExitPrefab, transform) as Transform;
                            bottomExitOpen.position   = scenePosition + new Vector2(0, -cellSize / 2.9f);
                            bottomExitOpen.localScale = new Vector2(bottomExitOpen.localScale.x * cellSize, bottomExitOpen.localScale.y * cellSize);
                            bottomExitOpen.name       = exitName + "(Open)";
                            oldComponents.Add(bottomExitOpen);
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
                                    exitPrefab     = exit1LeftRightClosed;
                                    openExitPrefab = exit1LeftRightOpen;
                                    exitName       = "Exit1";
                                    break;
                                case 2:
                                    exitPrefab     = exit2LeftRightClosed;
                                    openExitPrefab = exit2LeftRightOpen;
                                    exitName       = "Exit2";
                                    break;
                                case 3:
                                    exitPrefab     = exit3LeftRightClosed;
                                    openExitPrefab = exit3LeftRightOpen;
                                    exitName       = "Exit3";
                                    break;
                                case 4:
                                    exitPrefab     = exit4LeftRightClosed;
                                    openExitPrefab = exit4LeftRightOpen;
                                    exitName       = "Exit4";
                                    break;
                            }
                            var rightExit         = Instantiate(exitPrefab, transform) as Transform;
                            rightExit.position    = scenePosition + new Vector2(+cellSize / 2, 0);
                            rightExit.localScale  = new Vector2(rightExit.localScale.x * cellSize, rightExit.localScale.y * cellSize);
                            rightExit.eulerAngles = new Vector3(0, 180, 90);
                            rightExit.name        = exitName + "(Close)";
                            currentExit++;
                            oldComponents.Add(rightExit);
                            var rightExitOpen         = Instantiate(openExitPrefab, transform) as Transform;
                            rightExitOpen.position    = scenePosition + new Vector2(+cellSize / 2, 0);
                            rightExitOpen.localScale  = new Vector2(rightExitOpen.localScale.x * cellSize, rightExitOpen.localScale.y * cellSize);
                            rightExitOpen.eulerAngles = new Vector3(0, 180, 90);
                            rightExitOpen.name        = exitName + "(Open)";
                            oldComponents.Add(rightExitOpen);
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
    
        // Enable shadows
        //GameObject.Find("MazeRenderer").GetComponent<CompositeShadowCaster2D>().enabled = true;
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

    // Set the MazeDataJson for the client
    public void SetMazeDataJson(string jsonText){
        mazeDataJson = jsonText;
    }
}