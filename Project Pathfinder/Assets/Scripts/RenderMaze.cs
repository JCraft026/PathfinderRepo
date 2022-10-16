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
    [Range(1, 100)]
    private int mazeDensity = 26;

    [SerializeField]
    private float cellSize = 1f;

    [SerializeField]
    private Transform wallPrefab = null;

    [SerializeField]
    private Transform floorPrefab = null;

    int randomNumber = 0; // Random number to determine if a wall generates or not

    // Start is called before the first frame update
    void Start()
    {
        // Get the generated maze data
        WallStatus[,] mazeData = GenerateMaze.Generate(mazeWidth, mazeHeight);

        // Render the maze in the scene
        Render(mazeData);
    }

    // Render the complete maze within the scene
    private void Render(WallStatus[,] mazeData)
    {
        WallStatus currentCell = new WallStatus(); // Current maze cell being rendered
        Vector2 scenePosition  = new Vector2();    // x,y position in the scene
        var mazeFloor          = Instantiate(floorPrefab, transform);
                                                   // Maze floor prefab

        // Render the maze floor
        mazeFloor.localScale = new Vector2(cellSize * (mazeWidth), cellSize * (mazeHeight));

        // Adujust the maze floor in relation to the maze cells
        if(mazeHeight % 2 == 0){
            mazeFloor.position += new Vector3(0, -cellSize / 2, 0);
        }
        if(mazeWidth % 2 == 0){
            mazeFloor.position += new Vector3(-cellSize / 2, 0, 0);
        }

        // Render the cell walls of every maze cell
        for (int j = 0; j < mazeHeight; j++){
            for (int i = 0; i < mazeWidth; i++){
                currentCell = mazeData[i,j];
                scenePosition = new Vector2(cellSize * (-mazeWidth / 2 + i), cellSize * (-mazeHeight / 2 + j));
                randomNumber  = Random.Range(1, mazeDensity + 1); // Provides RNG for maze generation

                // Render the top wall of a maze cell
                if(j != mazeHeight - 1){
                    // Potentially render the top walls in between the top and bottom of the maze
                    if(currentCell.HasFlag(WallStatus.TOP) && randomNumber != 1){
                        var topWall        = Instantiate(wallPrefab, transform) as Transform;
                        topWall.position   = scenePosition + new Vector2(0, cellSize / 2);
                        topWall.localScale = new Vector2(cellSize, topWall.localScale.y);
                    }
                }
                else{
                    // Render the top walls at the top of the maze
                    if(currentCell.HasFlag(WallStatus.TOP)){
                        var topWall        = Instantiate(wallPrefab, transform) as Transform;
                        topWall.position   = scenePosition + new Vector2(0, cellSize / 2);
                        topWall.localScale = new Vector2(cellSize, topWall.localScale.y);
                    }
                }

                // Render the left wall of a maze cell
                if(i != 0){
                    // Potentially render left walls inbetween the left and right of maze
                    if(currentCell.HasFlag(WallStatus.LEFT) && randomNumber != 1){
                        var leftWall         = Instantiate(wallPrefab, transform) as Transform;
                        leftWall.position    = scenePosition + new Vector2(-cellSize / 2, 0);
                        leftWall.localScale  = new Vector2(cellSize, leftWall.localScale.y);
                        leftWall.eulerAngles = new Vector3(0, 180, 90);
                    }
                }
                else{
                    var leftWall         = Instantiate(wallPrefab, transform) as Transform;
                    leftWall.position    = scenePosition + new Vector2(-cellSize / 2, 0);
                    leftWall.localScale  = new Vector2(cellSize, leftWall.localScale.y);
                    leftWall.eulerAngles = new Vector3(0, 180, 90);
                }

                // Render the bottom wall of a maze cell if the current cell is in the bottom row
                if(j == 0){
                    // Render the bottom wall of the maze
                    if(currentCell.HasFlag(WallStatus.BOTTOM)){
                        var bottomWall        = Instantiate(wallPrefab, transform) as Transform;
                        bottomWall.position   = scenePosition + new Vector2(0, -cellSize / 2);
                        bottomWall.localScale = new Vector2(cellSize, bottomWall.localScale.y);
                    }
                }

                // Render the right wall of a maze cell if the current cell is in the right most column
                if(i == mazeWidth - 1){
                    if (currentCell.HasFlag(WallStatus.RIGHT)){
                        var rightWall         = Instantiate(wallPrefab, transform) as Transform;
                        rightWall.position    = scenePosition + new Vector2(+cellSize / 2, 0);
                        rightWall.localScale  = new Vector2(cellSize, rightWall.localScale.y);
                        rightWall.eulerAngles = new Vector3(0, 180, 90);
                    }
                }
            }
        }
    }
}