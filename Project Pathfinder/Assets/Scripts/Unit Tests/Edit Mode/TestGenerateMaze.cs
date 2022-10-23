using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestGenerateMaze
{
    // Confirm that the generated maze's size correctly cooresponds to the given parameters
    [Test]
    public void TestMazeSize(){
        System.Random randomNum = new System.Random();   // Random number generator
        int mazeWidth           = randomNum.Next(1, 50), // Random maze width
            mazeHeight          = randomNum.Next(1, 50), // Random maze height
            mazeDataSize;                                // Size of the maze data array
        
        // Get the generated maze data
        WallStatus[,] mazeData = GenerateMaze.Generate(mazeWidth, mazeHeight);

        // Get the maze data array size
        mazeDataSize = mazeData.GetLength(0) * mazeData.GetLength(1);

        // Test that the maze is the expected size
        Assert.That(mazeDataSize == mazeWidth * mazeHeight, "maze is not the expected size");
    }

    // Confirm that all maze cells have been visited
    [Test]
    public void TestAllCellsVisited(){
        System.Random randomNum = new System.Random();   // Random number generator
        int mazeWidth           = randomNum.Next(1, 50), // Random maze width
            mazeHeight          = randomNum.Next(1, 50), // Random maze height
            unvisitedCellCount  = 0;

        // Get the generated maze data
        WallStatus[,] mazeData = GenerateMaze.Generate(mazeWidth, mazeHeight);

        // Get the amount of unvisited cells
        for (int j = 0; j < mazeHeight; j++){
            for (int i = 0; i < mazeWidth; i++){
                if(mazeData[i,j] < WallStatus.VISITED){
                    unvisitedCellCount++;
                }
            }
        }

        // Test that all the maze cells have been visited
        Assert.That(unvisitedCellCount == 0, "unvisited maze cells present");
    }

    // Confirm that all four maze exits have been generated
    [Test]
    public void TestCorrectExitAmountGenerated(){
        System.Random randomNum = new System.Random();   // Random number generator
        int mazeWidth           = randomNum.Next(1, 50), // Random maze width
            mazeHeight          = randomNum.Next(1, 50), // Random maze height
            exitAmount          = 0;                     // Total exits generated

        exitAmount = GenerateMaze.GetExitLocations(mazeWidth, mazeHeight).Length;
        Assert.That(exitAmount == 4, "all four maze exits are not present");
    }
}