using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Mirror;
using System;

public class CustomNetworkManager : NetworkManager
{
    // Global Variables
    public static System.Random randomNum  = new System.Random(); // Random number generator
    public static int initialActiveGuardId = randomNum.Next(1,3); // Guard ID of the initial active guard
    public static bool isRunner            = false;               // User playing as Runner status

    public ItemWorld itemWorld;                                  //

    [SerializeField]
    RenderMaze mazeRenderer;

    [SerializeField]
    bool hostIsRunner;

    // Runs on the client once connected to the server - registers the message handler for the maze data
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        NetworkClient.RegisterHandler<MazeMessage>(ReceiveMazeData);
        NetworkClient.RegisterHandler<AnimationMessage>(NetworkAnimationHandler);
    }

    // Runs on the server when a client connects
    // Sends the maze to the client from the server
    // Also registers the animation handlers for each player
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        try
        {
            MazeMessage mazeMessage;
            mazeMessage.jsonMaze = mazeRenderer.GiveMazeDataToNetworkManager();

            if(mazeMessage.jsonMaze != null)
                conn.Send(mazeMessage);
            else
            {
                Debug.Log("mazeMessage.jsonMaze == null, mazeMessage not being sent to client");
            }
        }
        catch
        {
            Debug.Log("Exception caught in OnServerConnect!");
        }
        try{
            //itemWorld.spawnItemWorld(new Vector2(0,-5), Item.getRandomItem());
            Item generatedItem = Item.getRandomItem();
            generatedItem.amount = 1;
            Debug.Log("Generated an item");
            ItemWorldSpawner.SpawnItemWorld(new Vector2(0, -2), generatedItem);
        }
        catch(Exception e){
            Debug.LogError("failed to spawn item world in the network manager");
            Debug.LogError(e.Message);
            Debug.Log(e.Source);
        }
    }

    //Called when the client receives the json text of the maze
    public void ReceiveMazeData(MazeMessage mazeText)
    {
        // Don't run this code if the server is also a client as it will cause the maze to double render
        if(!NetworkClient.isHostClient)
        {
            try
            {
                if(mazeText.jsonMaze == null)
                    throw(new Exception("mazeText.jsonMaze == null, no data sent!"));
                else
                {
                    WallStatus[,] newMaze = JsonConvert.DeserializeObject<WallStatus[,]>(mazeText.jsonMaze); //If mazeText.jsonMaze == null major issues occur
                    mazeRenderer.CleanMap();
                    mazeRenderer.Render(newMaze);
                }
            }
            catch(Exception e)
            {
                Debug.Log("There was a problem decoding and/or rendering mazeText.jsonMaze resulting in the exception: " + e.Message);
            }
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log("OnServerConnect");

        // If the host is the runner set the client to the guards, if the client is the runner set the host to the guards
        if((hostIsRunner && NetworkServer.connections.Count > 1) ||
            (!hostIsRunner && NetworkServer.connections.Count == 1))
        {
            GameObject oldPlayer = conn.identity.gameObject;

            GameObject chaser = Instantiate(spawnPrefabs.FirstOrDefault(prefab => prefab.name.Contains("Chaser")));
            GameObject engineer = Instantiate(spawnPrefabs.FirstOrDefault(prefab => prefab.name.Contains("Engineer")));
            GameObject trapper = Instantiate(spawnPrefabs.FirstOrDefault(prefab => prefab.name.Contains("Trapper")));
            
            NetworkServer.Spawn(chaser);
            NetworkServer.Spawn(trapper);
            NetworkServer.Spawn(engineer);

            // Set guard spawn locations
            SetGuardSpawnLocations();

            // Select a random guard to initialize control
            switch (initialActiveGuardId)
            {
                case ManageActiveCharactersConstants.CHASER:
                    NetworkServer.ReplacePlayerForConnection(conn, chaser);
                    initialActiveGuardId = ManageActiveCharactersConstants.CHASER;
                    break;
                case ManageActiveCharactersConstants.ENGINEER:
                    NetworkServer.ReplacePlayerForConnection(conn, engineer);
                    initialActiveGuardId = ManageActiveCharactersConstants.ENGINEER;
                    break;
                case ManageActiveCharactersConstants.TRAPPER:
                    NetworkServer.ReplacePlayerForConnection(conn, trapper);
                    initialActiveGuardId = ManageActiveCharactersConstants.TRAPPER;
                    break;
            }

            Destroy(oldPlayer);

            Debug.Log("Replaced conID: " + conn.connectionId);
        }
    }

    public static void ChangeActiveGuard(NetworkConnectionToClient conn, int nextActiveGuardId)
    {
        string currentActiveGuard = conn.identity.gameObject.name; // Name of the current active guard object
        GameObject newGuardObject;                                 // Result of the guard query

        // Get the next guard's game object and update the active guard identification number
        switch (nextActiveGuardId)
        {
            case ManageActiveCharactersConstants.CHASER:
                newGuardObject = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser"));
                break;
            case ManageActiveCharactersConstants.ENGINEER:
                newGuardObject = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Engineer"));
                break;
            case ManageActiveCharactersConstants.TRAPPER:
                newGuardObject = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper"));
                break;
            default:
                newGuardObject = null;
                break;
        }

        // Switch guard control from the old guards object to the next guard's object
        if(newGuardObject != null)
        {
            NetworkServer.ReplacePlayerForConnection(conn, newGuardObject);
        }
        else
        {
            Debug.LogWarning("Could not find a new guard to switch to!");
        }
    }

    public void NetworkAnimationHandler(AnimationMessage animationState)
    {
        //This empty function is required for the networked animations to run... I don't know why and I'm scared to ask!
        
    }

    //Message structure used to send the maze to the client
    public struct MazeMessage : NetworkMessage
    {
        public string jsonMaze;
    }

    //Message structure used to send animation states between clients
    public struct AnimationMessage : NetworkMessage
    {
        public int characterType;
        public Vector2 movementInput;
        public float characterFacingDirection;
        public int connId;
    }

    // Position each guard object at a determined spawn location
    public void SetGuardSpawnLocations(){
        bool chaserSet          = false; // Chaser spawn set status
        bool engineerSet        = false; // Engineer spawn set status
        bool trapperSet         = false; // Trapper spawn set status
        bool firstPositionUsed  = false; // First exit position used status
        bool secondPositionUsed = false; // Second exit position used status
        bool thirdPositionUsed  = false; // Third exit position used status
        bool fourthPositionUsed = false; // Fourth exit position used status

        // Maze exit cell locations
        Vector2 firstExitPosition  = Utilities.GetMazeCellCoordinate(GenerateMaze.ExitLocations[0]._x, GenerateMaze.ExitLocations[0]._y);
        Vector2 secondExitPosition = Utilities.GetMazeCellCoordinate(GenerateMaze.ExitLocations[1]._x, GenerateMaze.ExitLocations[1]._y);
        Vector2 thirdExitPosition  = Utilities.GetMazeCellCoordinate(GenerateMaze.ExitLocations[2]._x, GenerateMaze.ExitLocations[2]._y);
        Vector2 fourthExitPosition = Utilities.GetMazeCellCoordinate(GenerateMaze.ExitLocations[3]._x, GenerateMaze.ExitLocations[3]._y);

        // Guard objects
        GameObject chaser   = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser"));
        GameObject engineer = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Engineer"));
        GameObject trapper  = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper"));

        // Set Chaser spawn position
        while(chaserSet == false){
            switch (randomNum.Next(1,4))
            {
                case 1:
                    if(firstPositionUsed == false){
                        chaser.transform.position = firstExitPosition;
                        firstPositionUsed         = true;
                        chaserSet                 = true;
                    }
                    break;
                case 2:
                    if(secondPositionUsed == false){
                        chaser.transform.position  = secondExitPosition;
                        secondPositionUsed         = true;
                        chaserSet                  = true;
                    }
                    break;
                case 3:
                    if(thirdPositionUsed == false){
                        chaser.transform.position = thirdExitPosition;
                        thirdPositionUsed         = true;
                        chaserSet                 = true;
                    }
                    break;
                case 4:
                    if(fourthPositionUsed == false){
                        chaser.transform.position  = fourthExitPosition;
                        fourthPositionUsed         = true;
                        chaserSet                  = true;
                    }
                    break;
            }
        }

        // Set Engineer spawn position
        while(engineerSet == false){
            switch (randomNum.Next(1,4))
            {
                case 1:
                    if(firstPositionUsed == false){
                        engineer.transform.position = firstExitPosition;
                        firstPositionUsed           = true;
                        engineerSet                 = true;
                    }
                    break;
                case 2:
                    if(secondPositionUsed == false){
                        engineer.transform.position = secondExitPosition;
                        secondPositionUsed          = true;
                        engineerSet                 = true;
                    }
                    break;
                case 3:
                    if(thirdPositionUsed == false){
                        engineer.transform.position = thirdExitPosition;
                        thirdPositionUsed           = true;
                        engineerSet                 = true;
                    }
                    break;
                case 4:
                    if(fourthPositionUsed == false){
                        engineer.transform.position = fourthExitPosition;
                        fourthPositionUsed          = true;
                        engineerSet                 = true;
                    }
                    break;
            }
        }

        // Set Trapper spawn position
        while(trapperSet == false){
            switch (randomNum.Next(1,4))
            {
                case 1:
                    if(firstPositionUsed == false){
                        trapper.transform.position = firstExitPosition;
                        firstPositionUsed          = true;
                        trapperSet                 = true;
                    }
                    break;
                case 2:
                    if(secondPositionUsed == false){
                        trapper.transform.position = secondExitPosition;
                        secondPositionUsed         = true;
                        trapperSet                 = true;
                    }
                    break;
                case 3:
                    if(thirdPositionUsed == false){
                        trapper.transform.position = thirdExitPosition;
                        thirdPositionUsed          = true;
                        trapperSet                 = true;
                    }
                    break;
                case 4:
                    if(fourthPositionUsed == false){
                        trapper.transform.position = fourthExitPosition;
                        fourthPositionUsed         = true;
                        trapperSet                 = true;
                    }
                    break;
            }
        }
    }
}
