using System.Collections;
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

    // Runs on the client once connected to the server - registers the message handler for the maze data
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        NetworkClient.RegisterHandler<MazeMessage>(ReceiveMazeData);
    }

    // Runs on the server when a client connects - sends the maze to the client from the server
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

    //Message structure used to send the maze to the client
    public struct MazeMessage : NetworkMessage
    {
        public string jsonMaze;
    }
}
