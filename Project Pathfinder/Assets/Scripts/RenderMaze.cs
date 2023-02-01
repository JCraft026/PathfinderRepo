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
    private Transform wallPrefab = null;

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

    private string mazeDataJson;                              // Json string version of the maze (used to send the maze to the client)
    private List<Transform> oldWalls = new List<Transform>(); // List of wall locations last rendered
    private Transform oldMazeFloor;                           // Last maze floor location rendered

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
        oldWalls.ForEach(wall => GameObject.Destroy(wall.gameObject));
        oldWalls = new();
        
        if(oldMazeFloor != null)
        {
            GameObject.Destroy(oldMazeFloor.gameObject);
            oldMazeFloor = null;
        }
    }

    // Render the complete maze within the scene
    public void Render(WallStatus[,] mazeData)
    {
        WallStatus currentCell = new WallStatus(); // Current maze cell being rendered
        Vector2 scenePosition  = new Vector2();    // x,y position in the scene
        Transform exitPrefab   = null;             // Exit prefab being rendered
        var mazeFloor          = Instantiate(floorPrefab, transform);
                                                   // Maze floor prefab
        int currentExit        = 1;                // Next exit prefab to render

        // Render the maze floor
        mazeFloor.localScale = new Vector2(cellSize * (mazeWidth), cellSize * (mazeHeight));
        oldMazeFloor = mazeFloor;

        // Adujust the maze floor position to accomodate even numbered dimensions
        if(mazeHeight % 2 == 0)
        {
            mazeFloor.position += new Vector3(0, -cellSize / 2, 0);
        }
        if(mazeWidth % 2 == 0)
        {
            mazeFloor.position += new Vector3(-cellSize / 2, 0, 0);
        }

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

                // Render the top wall of a maze cell
                if(currentCell.HasFlag(WallStatus.TOP)){
                    if(currentCell.HasFlag(WallStatus.EXIT) && j == mazeHeight-1){
                        var topExit        = Instantiate(exitPrefab, transform) as Transform;
                        topExit.position   = scenePosition + new Vector2(0, cellSize / 2);
                        topExit.localScale = new Vector2(cellSize, topExit.localScale.y);
                        currentExit++;
                        oldWalls.Add(topExit);
                    }
                    else{
                        var topWall        = Instantiate(wallPrefab, transform) as Transform;
                        topWall.position   = scenePosition + new Vector2(0, cellSize / 2);
                        topWall.localScale = new Vector2(cellSize, topWall.localScale.y);
                        oldWalls.Add(topWall);
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
                        oldWalls.Add(leftExit);
                    }
                    else{
                        var leftWall         = Instantiate(wallPrefab, transform) as Transform;
                        leftWall.position    = scenePosition + new Vector2(-cellSize / 2, 0);
                        leftWall.localScale  = new Vector2(cellSize, leftWall.localScale.y);
                        leftWall.eulerAngles = new Vector3(0, 180, 90);
                        oldWalls.Add(leftWall);
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
                            oldWalls.Add(bottomExit);
                        }
                        else{
                            var bottomWall        = Instantiate(wallPrefab, transform) as Transform;
                            bottomWall.position   = scenePosition + new Vector2(0, -cellSize / 2);
                            bottomWall.localScale = new Vector2(cellSize, bottomWall.localScale.y);
                            oldWalls.Add(bottomWall);
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
                            oldWalls.Add(rightExit);
                        }
                        else{
                            var rightWall         = Instantiate(wallPrefab, transform) as Transform;
                            rightWall.position    = scenePosition + new Vector2(+cellSize / 2, 0);
                            rightWall.localScale  = new Vector2(cellSize, rightWall.localScale.y);
                            rightWall.eulerAngles = new Vector3(0, 180, 90);
                            oldWalls.Add(rightWall);
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
