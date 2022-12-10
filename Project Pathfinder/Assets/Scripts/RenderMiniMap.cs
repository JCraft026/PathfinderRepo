using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderMiniMap : MonoBehaviour
{
    // Initialize fields on the inspector
    [SerializeField]
    public float cellSize = 1f;

    [SerializeField]
    private Transform wallPrefab = null;

    [SerializeField]
    private Transform floorPrefab = null;

    [SerializeField]
    private Transform roofPrefab = null;

    [SerializeField]
    private Transform firstExitPrefab = null;

    [SerializeField]
    private Transform secondExitPrefab = null;

    [SerializeField]
    private Transform thirdExitPrefab = null;

    [SerializeField]
    private Transform fourthExitPrefab = null;

    // Render the complete minimap within the scene canvas
    public void Render(WallStatus[,] mazeData)
    {
        WallStatus currentCell = new WallStatus();          // Current minimap cell being rendered
        Vector2 scenePosition  = new Vector2();             // x,y position in the scene
        Transform exitPrefab   = null;                      // Exit prefab being rendered
        int currentExit        = 1;                         // Next exit prefab to render
        float mazeWidth        = Utilities.GetMazeWidth(),  // Cell width of the maze
              mazeHeight       = Utilities.GetMazeHeight(); // Cell height of the maze

        // Render the cell walls/floors of every minimap cell
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

                // Render the minimap cell floor
                var cellFloor = Instantiate(floorPrefab, transform);
                cellFloor.transform.SetParent(GameObject.Find("Minimap").transform, false);
                cellFloor.GetComponent<RectTransform>().localScale    = new Vector2(cellSize, cellSize);
                cellFloor.GetComponent<RectTransform>().localPosition = scenePosition;
                cellFloor.name = "cf(" + (i-(int)(mazeWidth/2)) + "," + (j-(int)(mazeHeight/2)) + ")";

                // If the player is the runner, render the minimap cell roof
                if(CustomNetworkManager.isRunner == true){
                    var cellRoof = Instantiate(roofPrefab, transform);
                    cellRoof.transform.SetParent(GameObject.Find("Minimap").transform, false);
                    cellRoof.GetComponent<RectTransform>().localScale    = new Vector2(cellSize, cellSize);
                    cellRoof.GetComponent<RectTransform>().localPosition = scenePosition;
                    cellRoof.name = "cr(" + (i-(int)(mazeWidth/2)) + "," + (j-(int)(mazeHeight/2)) + ")";
                }

                // Render the top wall of a minimap cell
                if(currentCell.HasFlag(WallStatus.TOP)){
                    if(currentCell.HasFlag(WallStatus.EXIT) && j == mazeHeight-1){
                        var topExit = Instantiate(exitPrefab, transform) as Transform;
                        topExit.transform.SetParent(GameObject.Find("Minimap").transform, false);
                        topExit.GetComponent<RectTransform>().localPosition = scenePosition + new Vector2(0, cellSize / 2);
                        topExit.GetComponent<RectTransform>().localScale = new Vector2(cellSize, topExit.localScale.y);
                        currentExit++;
                    }
                    else{
                        var topWall = Instantiate(wallPrefab, transform) as Transform;
                        topWall.transform.SetParent(GameObject.Find("Minimap").transform, false);
                        topWall.GetComponent<RectTransform>().localPosition = scenePosition + new Vector2(0, cellSize / 2);
                        topWall.GetComponent<RectTransform>().localScale = new Vector2(cellSize, topWall.localScale.y);
                    }
                }

                // Render the left wall of a minimap cell
                if(currentCell.HasFlag(WallStatus.LEFT)){
                    if(currentCell.HasFlag(WallStatus.EXIT) && i == 0){
                        var leftExit = Instantiate(exitPrefab, transform) as Transform;
                        leftExit.transform.SetParent (GameObject.Find("Minimap").transform, false);
                        leftExit.GetComponent<RectTransform>().localPosition = scenePosition + new Vector2(-cellSize / 2, 0);
                        leftExit.GetComponent<RectTransform>().localScale  = new Vector2(cellSize, leftExit.localScale.y);
                        leftExit.eulerAngles = new Vector3(0, 180, 90);
                        currentExit++;
                    }
                    else{
                        var leftWall = Instantiate(wallPrefab, transform) as Transform;
                        leftWall.transform.SetParent(GameObject.Find("Minimap").transform, false);
                        leftWall.GetComponent<RectTransform>().localPosition = scenePosition + new Vector2(-cellSize / 2, 0);
                        leftWall.GetComponent<RectTransform>().localScale  = new Vector2(cellSize, leftWall.localScale.y);
                        leftWall.eulerAngles = new Vector3(0, 180, 90);
                    }
                }

                // Render the bottom wall of a minimap cell if the current cell is in the bottom row
                if(j == 0){
                    if (currentCell.HasFlag(WallStatus.BOTTOM))
                    {
                        if(currentCell.HasFlag(WallStatus.EXIT)){
                            var bottomExit = Instantiate(exitPrefab, transform) as Transform;
                            bottomExit.transform.SetParent(GameObject.Find("Minimap").transform, false);
                            bottomExit.GetComponent<RectTransform>().localPosition = scenePosition + new Vector2(0, -cellSize / 2);
                            bottomExit.GetComponent<RectTransform>().localScale = new Vector2(cellSize, bottomExit.localScale.y);
                            currentExit++;
                        }
                        else{
                            var bottomWall = Instantiate(wallPrefab, transform) as Transform;
                            bottomWall.transform.SetParent(GameObject.Find("Minimap").transform, false);
                            bottomWall.GetComponent<RectTransform>().localPosition = scenePosition + new Vector2(0, -cellSize / 2);
                            bottomWall.GetComponent<RectTransform>().localScale = new Vector2(cellSize, bottomWall.localScale.y);
                        }
                    }
                }

                // Render the right wall of a minimap cell if the current cell is in the right most column
                if(i == mazeWidth - 1){
                    if (currentCell.HasFlag(WallStatus.RIGHT))
                    {
                        if(currentCell.HasFlag(WallStatus.EXIT)){
                            var rightExit = Instantiate(exitPrefab, transform) as Transform;
                            rightExit.transform.SetParent(GameObject.Find("Minimap").transform, false);
                            rightExit.GetComponent<RectTransform>().localPosition = scenePosition + new Vector2(+cellSize / 2, 0);
                            rightExit.GetComponent<RectTransform>().localScale  = new Vector2(cellSize, rightExit.localScale.y);
                            rightExit.eulerAngles = new Vector3(0, 180, 90);
                            currentExit++;
                        }
                        else{
                            var rightWall = Instantiate(wallPrefab, transform) as Transform;
                            rightWall.transform.SetParent(GameObject.Find("Minimap").transform, false);
                            rightWall.GetComponent<RectTransform>().localPosition = scenePosition + new Vector2(+cellSize / 2, 0);
                            rightWall.GetComponent<RectTransform>().localScale  = new Vector2(cellSize, rightWall.localScale.y);
                            rightWall.eulerAngles = new Vector3(0, 180, 90);
                        }
                    }
                }
            }
        }
    }
}