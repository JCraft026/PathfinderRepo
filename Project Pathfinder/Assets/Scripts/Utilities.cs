using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Utilities : MonoBehaviour
{
    // Get the scene coordinates corresponding to the given maze cell grid positon
    public static Vector2 GetMazeCellCoordinate(int cellColumn, int cellRow){
        Vector2 mazeCoordinate = new Vector2(RenderMaze.CellSize * (-RenderMaze.MazeWidth / 2 + cellColumn + .5f), RenderMaze.CellSize * (-RenderMaze.MazeHeight / 2 + cellRow + .5f));

        return mazeCoordinate;
    }

    // Get the distance between two objects
    public static double GetDistanceBetweenObjects(Vector3 firstPosition, Vector3 secondPosition){
        return Math.Sqrt(Math.Abs(Math.Pow((firstPosition.x-secondPosition.x), 2)) + Math.Abs(Math.Pow((firstPosition.y-secondPosition.y), 2)));
    }
}
