using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Mirror;
using System;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField]
    RenderMaze mazeRenderer;

    [SerializeField]
    GameObject hostPlayerCharacter;
    
    [SerializeField]
    GameObject clientPlayerCharacter;

    [SerializeField]
    bool hostIsRunner;

    public static System.Random randomNum = new System.Random(); // Random number generator
    public static int activeGuardId = randomNum.Next(1,3);
    public static bool isRunner = false;

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
            switch (activeGuardId)
            {
                case ManageActiveCharactersConstants.CHASER:
                    NetworkServer.ReplacePlayerForConnection(conn, chaser);
                    activeGuardId = ManageActiveCharactersConstants.CHASER;
                    break;
                case ManageActiveCharactersConstants.ENGINEER:
                    NetworkServer.ReplacePlayerForConnection(conn, engineer);
                    activeGuardId = ManageActiveCharactersConstants.ENGINEER;
                    break;
                case ManageActiveCharactersConstants.TRAPPER:
                    NetworkServer.ReplacePlayerForConnection(conn, trapper);
                    activeGuardId = ManageActiveCharactersConstants.TRAPPER;
                    break;
            }

            Destroy(oldPlayer);

            Debug.Log("Replaced conID: " + conn.connectionId);
        }
    }

    public static void ChangeActiveGuard(NetworkConnectionToClient conn, int nextActiveGuardId)
    {
        // Run this function for only host side calls
        if(isRunner == false){
            string currentActiveGuard = conn.identity.gameObject.name; // Name of the current active guard
            GameObject newGuardObject;                                 // Result of the guard query

            // Get the next guard's game object and update the active guard identification number
            switch (nextActiveGuardId)
            {
                case ManageActiveCharactersConstants.CHASER:
                    newGuardObject = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser"));
                    activeGuardId = ManageActiveCharactersConstants.CHASER;
                    break;
                case ManageActiveCharactersConstants.ENGINEER:
                    newGuardObject = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Engineer"));
                    activeGuardId = ManageActiveCharactersConstants.ENGINEER;
                    break;
                case ManageActiveCharactersConstants.TRAPPER:
                    newGuardObject = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper"));
                    activeGuardId = ManageActiveCharactersConstants.TRAPPER;
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
        bool chaserSet          = false;
        bool engineerSet        = false;
        bool trapperSet         = false;
        bool firstPositionUsed  = false;
        bool secondPositionUsed = false;
        bool thirdPositionUsed  = false;
        bool fourthPositionUsed = false;

        // Maze exit cell locations
        Vector2 firstExitPosition  = new Vector2(RenderMaze.CellSize * (-RenderMaze.MazeWidth / 2 + GenerateMaze.exitLocations[0]._x + .5f), RenderMaze.CellSize * (-RenderMaze.MazeHeight / 2 + GenerateMaze.exitLocations[0]._y + .5f));
        Vector2 secondExitPosition = new Vector2(RenderMaze.CellSize * (-RenderMaze.MazeWidth / 2 + GenerateMaze.exitLocations[1]._x + .5f), RenderMaze.CellSize * (-RenderMaze.MazeHeight / 2 + GenerateMaze.exitLocations[1]._y + .5f));
        Vector2 thirdExitPosition  = new Vector2(RenderMaze.CellSize * (-RenderMaze.MazeWidth / 2 + GenerateMaze.exitLocations[2]._x + .5f), RenderMaze.CellSize * (-RenderMaze.MazeHeight / 2 + GenerateMaze.exitLocations[2]._y + .5f));
        Vector2 fourthExitPosition = new Vector2(RenderMaze.CellSize * (-RenderMaze.MazeWidth / 2 + GenerateMaze.exitLocations[3]._x + .5f), RenderMaze.CellSize * (-RenderMaze.MazeHeight / 2 + GenerateMaze.exitLocations[3]._y + .5f));

        // Guard objects
        GameObject chaser   = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser"));
        GameObject engineer = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Engineer"));
        GameObject trapper  = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper"));

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
