using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class Utilities : MonoBehaviour
{
    public static Dictionary<string, GameObject> ObjectLibrary = new Dictionary<string, GameObject>();
    
    public static void ClearObjectLibrary() {
        ObjectLibrary = new Dictionary<string, GameObject>();
    }
    
    public static GameObject GetObject(string objectName) {
        if (!ObjectLibrary.ContainsKey(objectName))
            ObjectLibrary.Add(objectName, Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(objectName)));
        else if ((ObjectLibrary[objectName] == null))
            ObjectLibrary[objectName] = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(objectName));
        return ObjectLibrary[objectName];
    }
    
    // Get the scene coordinates corresponding to the given maze cell grid positon
    public static Vector2 GetMazeCellCoordinate(int cellColumn, int cellRow){
        Vector2 mazeCoordinate = new Vector2(GetCellSize() * (-GetMazeWidth() / 2 + cellColumn + .5f), GetCellSize() * (-GetMazeHeight() / 2 + cellRow + .5f));

        return mazeCoordinate;
    }
    
    // Get the distance between two objects
    public static double GetDistanceBetweenObjects(Vector3 firstPosition, Vector3 secondPosition){
        return Math.Sqrt(Math.Abs(Math.Pow((firstPosition.x-secondPosition.x), 2)) + Math.Abs(Math.Pow((firstPosition.y-secondPosition.y), 2)));
    }

    // Get the maze index of the cell a given character is standing in, returns a 2 index integer array with index 0 being the x value and index 1 being the y
    public static int[] GetCharacterCellLocation(int characterCode){
        int[]   cellLocation = new int[2];
        Vector2 characterObjectPosition;

        switch (characterCode)
        {
            case ManageActiveCharactersConstants.RUNNER:
                characterObjectPosition = GetObject("Runner").transform.position; // Changed from Runner(Clone) to Runner
                characterObjectPosition.y -= .5f;
                break;
            case ManageActiveCharactersConstants.CHASER:
                characterObjectPosition = GetObject("Chaser(Clone)").transform.position;
                characterObjectPosition.y -= .84f;
                break;
            case ManageActiveCharactersConstants.ENGINEER:
                characterObjectPosition = GetObject("Engineer(Clone)").transform.position;
                characterObjectPosition.y -= .91f;
                break;
            case ManageActiveCharactersConstants.TRAPPER:
                characterObjectPosition = GetObject("Trapper(Clone)").transform.position;
                characterObjectPosition.y -= .76f;
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
        return GetObject("MazeRenderer").GetComponent<RenderMaze>().cellSize;
    }

    // Get the Maze Width from the Maze Renderer
    public static float GetMazeWidth(){
        return (float) GetObject("MazeRenderer").GetComponent<RenderMaze>().mazeWidth;
    }

    // Get the Maze Height from the Maze Renderer
    public static float GetMazeHeight(){
        return (float) GetObject("MazeRenderer").GetComponent<RenderMaze>().mazeHeight;
    }

    // Get the Mini Map Cell Size
    public static float GetMapCellSize(){
        return GetObject("MiniMapHandler").GetComponent<RenderMiniMap>().cellSize;
    }
}


