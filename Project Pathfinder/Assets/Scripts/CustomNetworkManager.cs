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
        string currentActiveGuard = conn.identity.gameObject.name;
        GameObject newGuardObject; //Result of the guard query

        //Locate the correct guard
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

        //Set the guard
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
}
