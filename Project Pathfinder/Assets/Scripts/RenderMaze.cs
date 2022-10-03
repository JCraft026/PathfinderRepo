using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderMaze : MonoBehaviour
{
    // Initialize fields on the inspector
    [SerializeField]
    [Range(1, 50)]
    private int mazeWidth = 10;

    [SerializeField]
    [Range(1, 50)]
    private int mazeHeight = 10;

    [SerializeField]
    private float cellSize = 1f;

    [SerializeField]
    private Transform wallPrefab = null;

    [SerializeField]
    private Transform floorPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        // Get the generated maze data
        WallStatus[,] mazeData = GenerateMaze.Generate(mazeWidth, mazeHeight);

        // Render the maze in the scene
        Render(mazeData);
    }

    // Render the complete maze within the scene
    private void Render(WallStatus[,] mazeData){
        WallStatus currentCell = new WallStatus(); // Current maze cell being rendered
        Vector2 scenePosition  = new Vector2();    // x,y position in the scene
        var mazeFloor          = Instantiate(floorPrefab, transform);
                                                   // Maze floor prefab

        // Render the maze floor
        mazeFloor.localScale = new Vector2(mazeWidth, mazeHeight);

        // Render the cell walls of every maze cell
        for (int j = 0; j < mazeHeight; j++){
            for (int i = 0; i < mazeWidth; i++){
                currentCell = mazeData[i,j];
                scenePosition = new Vector2(-mazeWidth / 2 + i, -mazeHeight / 2 + j);

                // Render the top wall of a maze cell
                if(currentCell.HasFlag(WallStatus.TOP)){
                    var topWall        = Instantiate(wallPrefab, transform) as Transform;
                    topWall.position   = scenePosition + new Vector2(0, cellSize / 2);
                    topWall.localScale = new Vector2(cellSize, topWall.localScale.y);
                }

                // Render the left wall of a maze cell
                if(currentCell.HasFlag(WallStatus.LEFT)){
                    var leftWall         = Instantiate(wallPrefab, transform) as Transform;
                    leftWall.position    = scenePosition + new Vector2(-cellSize / 2, 0);
                    leftWall.localScale  = new Vector2(cellSize, leftWall.localScale.y);
                    leftWall.eulerAngles = new Vector3(0, 90, 0);
                }

                // Render the bottom wall of a maze cell if the current cell is in the bottom row
                if(j == 0){
                    if (currentCell.HasFlag(WallStatus.BOTTOM))
                    {
                        var bottomWall        = Instantiate(wallPrefab, transform) as Transform;
                        bottomWall.position   = scenePosition + new Vector2(0, -cellSize / 2);
                        bottomWall.localScale = new Vector2(cellSize, bottomWall.localScale.y);
                    }
                }

                // Render the right wall of a maze cell if the current cell is in the right most column
                if(i == mazeWidth - 1){
                    if (currentCell.HasFlag(WallStatus.RIGHT))
                    {
                        var rightWall         = Instantiate(wallPrefab, transform) as Transform;
                        rightWall.position    = scenePosition + new Vector2(+cellSize / 2, 0);
                        rightWall.localScale  = new Vector2(cellSize, rightWall.localScale.y);
                        rightWall.eulerAngles = new Vector3(0, 90, 0);
                    }
                }
            }
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
