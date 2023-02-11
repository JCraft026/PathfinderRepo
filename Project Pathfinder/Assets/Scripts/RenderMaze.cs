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
    public float cellSize = 1f;

    [SerializeField]
    private Transform brickWallPrefab = null;

    [SerializeField]
    private Transform sideWallPrefab = null;

    [SerializeField]
    private Transform floorPrefab = null;

    [SerializeField]
    private Transform firstExitPrefab = null;

    [SerializeField]
    private Transform secondExitPrefab = null;

    [SerializeField]
    private Transform thirdExitPrefab = null;

    [SerializeField]
    private Transform fourthExitPrefab = null;

    private string mazeDataJson;                                   // Json string version of the maze (used to send the maze to the client)
    private List<Transform> oldComponents = new List<Transform>(); // List of wall locations last rendered

    // Called when the host starts a game
    public override void OnStartServer()
    {
        base.OnStartServer();

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
        WallStatus currentCell = new WallStatus(); // Current maze cell being rendered
        Vector2 scenePosition  = new Vector2();    // x,y position in the scene
        Transform exitPrefab   = null;             // Exit prefab being rendered
        int currentExit        = 1;                // Next exit prefab to render

        // Render the cell walls of every maze cell
        for (int j = 0; j < mazeHeight; j++){
            for (int i = 0; i < mazeWidth; i++){
                currentCell = mazeData[i,j];
                scenePosition = new Vector2(cellSize * (-mazeWidth / 2 + i), cellSize * (-mazeHeight / 2 + j));

                // Select the next exit prefab to render
                switch (currentExit)
                {
                    case 1:
                        exitPrefab = firstExitPrefab;
                        break;
                    case 2:
                        exitPrefab = secondExitPrefab;
                        break;
                    case 3:
                        exitPrefab = thirdExitPrefab;
                        break;
                    case 4:
                        exitPrefab = fourthExitPrefab;
                        break;
                }

                // Render the cell floor
                var cellFloor        = Instantiate(floorPrefab, transform);
                cellFloor.localScale = new Vector2(cellSize, cellSize);
                cellFloor.position   = scenePosition;
                cellFloor.name       = "mcf(" + (i-(int)(mazeWidth/2)) + "," + (j-(int)(mazeHeight/2)) + ")";
                oldComponents.Add(cellFloor);

                // Render the top wall of a maze cell
                if(currentCell.HasFlag(WallStatus.TOP)){
                    if(currentCell.HasFlag(WallStatus.EXIT) && j == mazeHeight-1){
                        var topExit        = Instantiate(exitPrefab, transform) as Transform;
                        topExit.position   = scenePosition + new Vector2(0, cellSize / 2);
                        topExit.localScale = new Vector2(cellSize, topExit.localScale.y);
                        currentExit++;
                        oldComponents.Add(topExit);
                    }
                    else{
                        var topBrickWall        = Instantiate(brickWallPrefab, transform) as Transform;
                        topBrickWall.position   = scenePosition + new Vector2(0, cellSize / 1.6f);
                        topBrickWall.localScale = new Vector2(topBrickWall.localScale.x * cellSize, topBrickWall.localScale.y * cellSize);
                        oldComponents.Add(topBrickWall);
                    }
                }

                // Render the left wall of a maze cell
                if(currentCell.HasFlag(WallStatus.LEFT)){
                    if(currentCell.HasFlag(WallStatus.EXIT) && i == 0){
                        var leftExit         = Instantiate(exitPrefab, transform) as Transform;
                        leftExit.position    = scenePosition + new Vector2(-cellSize / 2, 0);
                        leftExit.localScale  = new Vector2(cellSize, leftExit.localScale.y);
                        leftExit.eulerAngles = new Vector3(0, 180, 90);
                        currentExit++;
                        oldComponents.Add(leftExit);
                    }
                    else{
                        // Spawn the left wall
                        var leftWall         = Instantiate(sideWallPrefab, transform) as Transform;
                        leftWall.position    = scenePosition + new Vector2(-cellSize / 2, 0);
                        leftWall.localScale  = new Vector2(leftWall.localScale.y * cellSize, leftWall.localScale.y * cellSize);
                        leftWall.eulerAngles = new Vector3(0, 180, 90);
                        oldComponents.Add(leftWall);
                    }
                }

                // Render the bottom wall of a maze cell if the current cell is in the bottom row
                if(j == 0){
                    if (currentCell.HasFlag(WallStatus.BOTTOM))
                    {
                        if(currentCell.HasFlag(WallStatus.EXIT)){
                            var bottomExit        = Instantiate(exitPrefab, transform) as Transform;
                            bottomExit.position   = scenePosition + new Vector2(0, -cellSize / 2);
                            bottomExit.localScale = new Vector2(cellSize, bottomExit.localScale.y);
                            currentExit++;
                            oldComponents.Add(bottomExit);
                        }
                        else{
                            var bottomBrickWall        = Instantiate(brickWallPrefab, transform) as Transform;
                            bottomBrickWall.position   = scenePosition + new Vector2(0, -cellSize / 2.7f);
                            bottomBrickWall.localScale = new Vector2(bottomBrickWall.localScale.x * cellSize, bottomBrickWall.localScale.y * cellSize);
                            oldComponents.Add(bottomBrickWall);
                        }
                    }
                }

                // Render the right wall of a maze cell if the current cell is in the right most column
                if(i == mazeWidth - 1){
                    if (currentCell.HasFlag(WallStatus.RIGHT))
                    {
                        if(currentCell.HasFlag(WallStatus.EXIT)){
                            var rightExit         = Instantiate(exitPrefab, transform) as Transform;
                            rightExit.position    = scenePosition + new Vector2(+cellSize / 2, 0);
                            rightExit.localScale  = new Vector2(cellSize, rightExit.localScale.y);
                            rightExit.eulerAngles = new Vector3(0, 180, 90);
                            currentExit++;
                            oldComponents.Add(rightExit);
                        }
                        else{
                            // Spawn the right wall
                            var rightWall         = Instantiate(sideWallPrefab, transform) as Transform;
                            rightWall.position    = scenePosition + new Vector2(+cellSize / 2, 0);
                            rightWall.localScale  = new Vector2(rightWall.localScale.x * cellSize, rightWall.localScale.y * cellSize);
                            rightWall.eulerAngles = new Vector3(0, 180, 90);
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
}
