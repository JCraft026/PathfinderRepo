using System.Collections;
using System.Collections.Generic;
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
        int[] cellLocation = new int[2];

        switch (characterCode)
        {
            case ManageActiveCharactersConstants.RUNNER:
                break;
            case ManageActiveCharactersConstants.CHASER:
                break;
            case ManageActiveCharactersConstants.ENGINEER:
                break;
            case ManageActiveCharactersConstants.TRAPPER:
                break;
        }
    }
}
