using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// X and Y grid positions of each maze cell
public struct CellPosition{
    public int _x;
    public int _y;
}

// Data belonging to the neighbor of the current maze cell
public struct CellNeighbor{
    public CellPosition _position;   // Grid position of the neighbor cell
    public WallStatus   _sharedWall; // Wall shared by the current and neighbor cells
}

// Status bits reflecting the presence of each wall within a maze cell
[Flags]
public enum WallStatus{
    TOP     = 1,   // (0001) Top wall bit
    LEFT    = 2,   // (0010) Left wall bit
    BOTTOM  = 4,   // (0100) Bottom wall bit
    RIGHT   = 8,   // (1000) Right wall bit
    VISITED = 128, // (1000 0000) Cell visited status bit
}

public class GenerateMaze : MonoBehaviour
{
    // Create a completed randomly generated maze
    public static WallStatus[,] Generate(int mazeWidth, int mazeHeight){
        WallStatus[,] mazeData      = new WallStatus[mazeWidth, mazeHeight]; // Maze cell data
        WallStatus    initialStatus = WallStatus.TOP | WallStatus.LEFT | WallStatus.BOTTOM | WallStatus.RIGHT;
                                                                             // Initial wall statuses for every maze cell

        // Initialize the wall statuses of each maze cell
        for(int j = 0; j < mazeHeight; j++){
            for(int i = 0; i < mazeWidth; i++){
                mazeData[i,j] = initialStatus;
            }
        }
        
        // Generate all paths throughout the maze
        return GeneratePaths(mazeData, mazeWidth, mazeHeight);
    }

    // Generate all paths throughout the maze
    private static WallStatus[,] GeneratePaths(WallStatus[,] mazeData, int mazeWidth, int mazeHeight){
        Stack<CellPosition> positionStack      = new Stack<CellPosition>(); // Holds maze cell position data
        System.Random randomNum                = new System.Random();       // Random number generator
        CellPosition initialCell               = new CellPosition{ _x = randomNum.Next(0, mazeWidth), _y = randomNum.Next(0, mazeHeight) },
                                                                            // Starting cell for path generation
                     currentCell               = new CellPosition(),        // Position of the current cell being processed
                     neighborCell              = new CellPosition();        // Position of the selected neighbor of the current cell
        List<CellNeighbor> unvisitedNeighbors  = new List<CellNeighbor>();  // List of unvisited neighbors to the current cell  
        CellNeighbor randomNeighbour           = new CellNeighbor();        // Randomly selected neighbor from the list of current cell neighbors
        int randomIndex;                                                    // Index used to randomly select a cell neighbor
        
        // Process the initial cell as part of the maze path
        mazeData[initialCell._x, initialCell._y] |= WallStatus.VISITED;
        positionStack.Push(initialCell);

        // Generate maze paths until all maze cells have been visited
        while(positionStack.Count > 0){
            currentCell        = positionStack.Pop();
            unvisitedNeighbors = GetUnvisitedNeighbours(currentCell, mazeData, mazeWidth, mazeHeight);

            // If the current cell has an unvisited neighbor, add the neighbor cell to the current maze path
            if(unvisitedNeighbors.Count > 0){
                // Add the current cell to the path history
                positionStack.Push(currentCell);

                // Randomly select an unvisited neighbor
                randomIndex     = randomNum.Next(0, unvisitedNeighbors.Count);
                randomNeighbour = unvisitedNeighbors[randomIndex];
                neighborCell    = randomNeighbour._position;

                // Delete the maze wall separating the current cell from its neighbor
                mazeData[currentCell._x, currentCell._y] &= ~randomNeighbour._sharedWall;
                mazeData[neighborCell._x, neighborCell._y] &= ~GetOppositeWall(randomNeighbour._sharedWall);

                // Set the neighbor cell as the new current cell
                mazeData[neighborCell._x, neighborCell._y] |= WallStatus.VISITED;
                positionStack.Push(neighborCell);
            }
        }
        return mazeData;
    }

    // Get a list of the current cell's unvisited neighbors
    private static List<CellNeighbor> GetUnvisitedNeighbours(CellPosition currentCell, WallStatus[,] mazeData, int mazeWidth, int mazeHeight){
        List<CellNeighbor> unvisitedNeighbors = new List<CellNeighbor>(); // List of unvisited neighbors to the current cell

        // Check if there is an unvisited neighbor above the current cell
        if(currentCell._y < mazeHeight - 1){
            if(!mazeData[currentCell._x, currentCell._y + 1].HasFlag(WallStatus.VISITED)){
                unvisitedNeighbors.Add(new CellNeighbor{_position = new CellPosition{_x = currentCell._x, _y = currentCell._y + 1}, _sharedWall = WallStatus.TOP});
            }
        }

        // Check if there is an unvisited neighbor to the left of the current cell
        if(currentCell._x > 0){
            if(!mazeData[currentCell._x - 1, currentCell._y].HasFlag(WallStatus.VISITED)){
                unvisitedNeighbors.Add(new CellNeighbor{_position = new CellPosition{_x = currentCell._x - 1, _y = currentCell._y}, _sharedWall = WallStatus.LEFT});
            }
        }

        // Check if there is an unvisited neighbor below the current cell
        if(currentCell._y > 0){
            if(!mazeData[currentCell._x, currentCell._y - 1].HasFlag(WallStatus.VISITED)){
                unvisitedNeighbors.Add(new CellNeighbor{_position = new CellPosition{_x = currentCell._x, _y = currentCell._y - 1}, _sharedWall = WallStatus.BOTTOM});
            }
        }

        // Check if there is an unvisited neighbor to the right of the current cell
        if(currentCell._x < mazeWidth - 1){
            if(!mazeData[currentCell._x + 1, currentCell._y].HasFlag(WallStatus.VISITED)){
                unvisitedNeighbors.Add(new CellNeighbor{_position = new CellPosition{_x = currentCell._x + 1, _y = currentCell._y}, _sharedWall = WallStatus.RIGHT});
            }
        }

        return unvisitedNeighbors;
    }

    // Get the cell wall oposite to the one given
    private static WallStatus GetOppositeWall(WallStatus mazeWall){
        switch (mazeWall)
        {
            case WallStatus.TOP:
                return WallStatus.BOTTOM;
            case WallStatus.LEFT:
                return WallStatus.RIGHT;
            case WallStatus.BOTTOM:
                return WallStatus.TOP;
            case WallStatus.RIGHT:
                return WallStatus.LEFT;
            default:
                return WallStatus.TOP;
        }
    }
}