using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public class Utilities : MonoBehaviour
{
    // Regular Expressions
    public static Regex chaserRegex   = new Regex("Chaser");   // Match "Chaser"
    public static Regex engineerRegex = new Regex("Engineer"); // Match "Engineer"
    public static Regex trapperRegex  = new Regex("Trapper");  // Match "Trapper"
    public static Regex runnerRegex   = new Regex("Runner");   // Match "Runner"
    
    public static GameObject chaser;
    public static GameObject engineer;
    public static GameObject trapper;
    public static GameObject runner;
    public static GameObject mazeRenderer;
    public static GameObject miniMapHandler;
    
    
    public static Dictionary<string, GameObject> ObjectCache = new Dictionary<string, GameObject>();
    
    public static void ClearObjectLibrary() {
        ObjectCache = new Dictionary<string, GameObject>();
    }
    
    public static GameObject GetObject(string objectName) {
        if (!ObjectCache.ContainsKey(objectName))
            ObjectCache.Add(objectName, Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(objectName)));
        else if ((ObjectCache[objectName] == null))
            ObjectCache[objectName] = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(objectName));
        return ObjectCache[objectName];
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
                characterObjectPosition = GetRunner().transform.position;
                characterObjectPosition.y -= .5f;
                break;
            case ManageActiveCharactersConstants.CHASER:
                characterObjectPosition = GetChaser().transform.position;
                characterObjectPosition.y -= .84f;
                break;
            case ManageActiveCharactersConstants.ENGINEER:
                characterObjectPosition = GetEngineer().transform.position;
                characterObjectPosition.y -= .91f;
                break;
            case ManageActiveCharactersConstants.TRAPPER:
                characterObjectPosition = GetTrapper().transform.position;
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
    
    //=========================================================================
    // Get Derived Value Functions
    //=========================================================================
    
    // Get the Cell Size from the Maze Renderer
    public static float GetCellSize(){
        return GetMazeRenderer().GetComponent<RenderMaze>().cellSize;
    }

    // Get the Maze Width from the Maze Renderer
    public static float GetMazeWidth(){
        return (float) GetMazeRenderer().GetComponent<RenderMaze>().mazeWidth;
    }
    
    // Get the Maze Height from the Maze Renderer
    public static float GetMazeHeight(){
        return (float) GetMazeRenderer().GetComponent<RenderMaze>().mazeHeight;
    }
    
    // Get the Mini Map Cell Size
    public static float GetMapCellSize(){
        return GetMiniMapHandler().GetComponent<RenderMiniMap>().cellSize;
    }
    
    //=========================================================================
    // Get Object Functions
    //=========================================================================
    
    public static GameObject GetMazeRenderer() {
        if (mazeRenderer == null)
            mazeRenderer = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("MazeRenderer"));
        return mazeRenderer;
    }
    
    public static GameObject GetMiniMapHandler() {
        if (miniMapHandler == null)
            miniMapHandler = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("MiniMapHandler"));
        return miniMapHandler;
    }
    
    public static GameObject GetRunner() {
        if (runner == null)
            runner = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner")); // Changed from Runner(Clone) to Runner
        return runner;
    }
    
    public static GameObject GetChaser() {
        if (chaser == null)
            chaser = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser(Clone)"));
        return chaser;
    }
    
    public static GameObject GetEngineer() {
        if (engineer == null)
            engineer = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Engineer(Clone)"));
        return engineer;
    }
    
    public static GameObject GetTrapper() {
        if (trapper == null)
            trapper = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper(Clone)"));
        return trapper;
    }
    
    
}


