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

    public bool isRunner = false; // Reflect whether the current player is playing as the Runner

    public const int PLAYER_TYPE_RUNNER   = 0;
    public const int PLAYER_TYPE_CHASER   = 1;
    public const int PLAYER_TYPE_TRAPPER  = 2;
    public const int PLAYER_TYPE_MECHANIC = 3;
    public const int PLAYER_TYPE_UNKNOWN  = 404;

    // Runs on the client once connected to the server - registers the message handler for the maze data
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        NetworkClient.RegisterHandler<MazeMessage>(ReceiveMazeData);
        NetworkClient.RegisterHandler<AnimationMessage>(NetworkAnimationHandler);
        //NetworkServer.RegisterHandler<AnimationMessage>(PassAnimationMessage, true);
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
        
        // If a client that is not also the host is connected, force the new player to be on the opposing team
        if(NetworkServer.connections.Count > 1)
        {
            GameObject oldPlayer = conn.identity.gameObject;

            NetworkServer.ReplacePlayerForConnection(conn, Instantiate(clientPlayerCharacter));

            Destroy(oldPlayer);

            Debug.Log("Replaced conID: " + conn.connectionId);
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
