using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Utilities : MonoBehaviour
{
    // Get the scene coordinates corresponding to the given maze cell grid positon
    public static Vector2 GetMazeCellCoordinate(int cellColumn, int cellRow){
        Vector2 mazeCoordinate = new Vector2(GetCellSize() * (-GetMazeWidth() / 2 + cellColumn + .5f), GetCellSize() * (-GetMazeHeight() / 2 + cellRow + .5f));

        return mazeCoordinate;
    }

    // Get the maze index of the cell a given character is standing in, returns a 2 index integer array with index 0 being the x value and index 1 being the y
    public static int[] GetCharacterCellLocation(int characterCode){
        int[]   cellLocation = new int[2];
        Vector2 characterObjectPosition;

        switch (characterCode)
        {
            case ManageActiveCharactersConstants.RUNNER:
                characterObjectPosition = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner")).transform.position;
                break;
            case ManageActiveCharactersConstants.CHASER:
                characterObjectPosition = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser")).transform.position;
                break;
            case ManageActiveCharactersConstants.ENGINEER:
                characterObjectPosition = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Enginieer")).transform.position;
                break;
            case ManageActiveCharactersConstants.TRAPPER:
                characterObjectPosition = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper")).transform.position;
                break;
            default:
                characterObjectPosition.x = 0.0f;
                characterObjectPosition.y = 0.0f;
                break;
        }
        cellLocation[0] = (int)Mathf.Ceil(((characterObjectPosition.x - GetCellSize()/2)/GetCellSize()));
        cellLocation[1] = (int)Mathf.Ceil(((characterObjectPosition.y - GetCellSize()/2)/GetCellSize()));

        return cellLocation;
    }

    // Get the Cell Size from the Maze Renderer
    public static float GetCellSize(){
        GameObject mazeRenderer = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("MazeRenderer"));

        return mazeRenderer.GetComponent<RenderMaze>().cellSize;
    }

    // Get the Maze Width from the Maze Renderer
    public static float GetMazeWidth(){
        GameObject mazeRenderer = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("MazeRenderer"));

        return (float)mazeRenderer.GetComponent<RenderMaze>().mazeWidth;
    }

    // Get the Maze Height from the Maze Renderer
    public static float GetMazeHeight(){
        GameObject mazeRenderer = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("MazeRenderer"));

        return (float)mazeRenderer.GetComponent<RenderMaze>().mazeHeight;
    }
}
