using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Utilities : MonoBehaviour
{
    // Get the scene coordinates corresponding to the given maze cell grid positon
    public static Vector2 GetMazeCellCoordinate(int cellColumn, int cellRow){
        Vector2 mazeCoordinate = new Vector2(RenderMaze.CellSize * (-RenderMaze.MazeWidth / 2 + cellColumn + .5f), RenderMaze.CellSize * (-RenderMaze.MazeHeight / 2 + cellRow + .5f));

        return mazeCoordinate;
    }

    // Get the maze index of the cell a given character is standing in
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
        cellLocation[0] = (int)(characterObjectPosition.x/RenderMaze.CellSize);
        cellLocation[1] = (int)(characterObjectPosition.y/RenderMaze.CellSize);
        Debug.Log("Cell Size: " + RenderMaze.CellSize);

        return cellLocation;
    }
}
