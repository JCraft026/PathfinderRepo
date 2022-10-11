using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Flags]
public enum WallState
{
    // 0000 -> NO WALLS
    // 1111 -> LEFT,RIGHT,UP,DOWN
    LEFT = 1, // 0001
    RIGHT = 2, // 0010
    UP = 4, // 0100
    DOWN = 8, // 1000

    VISITED = 128, // 1000 0000
}

public struct Position
{
    public int X;
    public int Y;
}

public struct Neighbour
{
    public Position Position;
    public WallState SharedWall;
}

public static class MazeGenerator
{

    private static WallState GetOppositeWall(WallState wall)
    {
        switch (wall)
        {
            case WallState.RIGHT: return WallState.LEFT;
            case WallState.LEFT: return WallState.RIGHT;
            case WallState.UP: return WallState.DOWN;
            case WallState.DOWN: return WallState.UP;
            default: return WallState.LEFT;
        }
    }

    private static WallState[,] ApplyRecursiveBacktracker(WallState[,] maze, int width, int height)
    {
        // here we make changes
        var rng = new System.Random(/*seed*/);                                                       // Create a random number generator
        var positionStack = new Stack<Position>();                                                   // Creates a stack that holds only items of a Position type
        var position = new Position { X = rng.Next(0, width), Y = rng.Next(0, height) };             // Finds a random position on the map (x is between 0 and width, y is between 0 and height)

        maze[position.X, position.Y] |= WallState.VISITED;  // 1000 1111                             // Mark the current cell positon of the maze as visited by perfoming a bitwise OR operation
        positionStack.Push(position);                                                                // Push the current cell postion onto the position stack

        while (positionStack.Count > 0)
        {
            var current = positionStack.Pop();                                                       // Set the current cell position to the position stored on the stack
            var neighbours = GetUnvisitedNeighbours(current, maze, width, height);                   // Get a list of the current cell's unvisited neighbors

            // If the current cell has an unvisited neighbor
            if (neighbours.Count > 0)
            {
                positionStack.Push(current);                                                         // Push the current cell position onto the positon stack to mark it as a 
                                                                                                     // previously visited cell so that when the curren path reaches a dead end, 
                                                                                                     // it automatically back tracks backwards through the completed path until a 
                                                                                                     // new path is found by selection of an unvisited neighbor

                var randIndex = rng.Next(0, neighbours.Count);                                       // Select one of the list indexes of one of the cells neighbors at random
                var randomNeighbour = neighbours[randIndex];                                         // Assign the data belonging to the neighbor at the random index

                var nPosition = randomNeighbour.Position;                                            // assign the cell position of the unvisited neighbor

                maze[current.X, current.Y] &= ~randomNeighbour.SharedWall;                           // Destroy the wall that is shared by the current cell and the neighbor cell
                                                                                                     // (the ~ (bitwise complement) will invert the bit of the shared wall (making it 
                                                                                                     // zero), a bitwise AND operation is then performed making the cooresponding 
                                                                                                     // wallstate bit of the cell zero)
                maze[nPosition.X, nPosition.Y] &= ~GetOppositeWall(randomNeighbour.SharedWall);      // Mark that the shared wall was destroyed in the neighbors cell as well
                maze[nPosition.X, nPosition.Y] |= WallState.VISITED;                                 // Mark the neighbor cell position of the maze as visited

                positionStack.Push(nPosition);                                                       // Push the neighbor cell to the position stack so that it can be the new current cell
            }
        }

        return maze;
    }

    private static List<Neighbour> GetUnvisitedNeighbours(Position p, WallState[,] maze, int width, int height)
    {
        var list = new List<Neighbour>();

        if (p.X > 0) // left
        {
            if (!maze[p.X - 1, p.Y].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X - 1,
                        Y = p.Y
                    },
                    SharedWall = WallState.LEFT
                });
            }
        }

        if (p.Y > 0) // DOWN
        {
            if (!maze[p.X, p.Y - 1].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X,
                        Y = p.Y - 1
                    },
                    SharedWall = WallState.DOWN
                });
            }
        }

        if (p.Y < height - 1) // UP
        {
            if (!maze[p.X, p.Y + 1].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X,
                        Y = p.Y + 1
                    },
                    SharedWall = WallState.UP
                });
            }
        }

        if (p.X < width - 1) // RIGHT
        {
            if (!maze[p.X + 1, p.Y].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X + 1,
                        Y = p.Y
                    },
                    SharedWall = WallState.RIGHT
                });
            }
        }

        return list;
    }

// Returns a two dimensional wallstate array with each index representing a cell and its assigned value representing the wallstates of each of its walls
    public static WallState[,] Generate(int width, int height)
    {
        WallState[,] maze = new WallState[width, height];
        WallState initial = WallState.RIGHT | WallState.LEFT | WallState.UP | WallState.DOWN;
        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                maze[i, j] = initial;  // 1111
            }
        }
        
        return ApplyRecursiveBacktracker(maze, width, height);
    }
}
