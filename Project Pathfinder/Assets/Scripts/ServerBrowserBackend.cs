using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Mirror.Discovery;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerBrowserBackend : MonoBehaviour
{
    #region Global Variables
    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>(); // List of servers discovered to return to the frontend
    Vector2 scrollViewPos = Vector2.zero; // I don't know what this is doing here, this is something that frontend will handle
    public CustomNetworkDiscovery networkDiscovery; // Allows the server browser to detect open games and connect to them
    private const int LOAD_MAZE_SCENE_INDEX = 5;    // Build index for the LoadMaze scene, this is subject to change in the future
    #endregion

    #region Hosting
    // Launch the maze renderer scene and then host the game
    public void StartHosting()
    {
        discoveredServers.Clear(); // We might as well wipe out the old servers that we won't need anymore once we start hosting

        // This needs to run within a coroutine as it is a thread safe version of "async" for unity
        StartCoroutine(LoadMazeSceneAsync());
    }
    #endregion Hosting

    #region Scene Management
    // Load the "LoadMaze" scene asyncrhonously and set it up with the network manager
    IEnumerator LoadMazeSceneAsync()
    {
        // Start the loading process
        Debug.Log("Loading Scene #... " + LOAD_MAZE_SCENE_INDEX);
        AsyncOperation loading = SceneManager.LoadSceneAsync(LOAD_MAZE_SCENE_INDEX); 
        loading.allowSceneActivation = true;

        // Wait for the scene to be loaded
        while(!loading.isDone)
        {
            Debug.Log("Scene Loading Progress: " + loading.progress);
            yield return null;
        }

        // Isolate the network manager
        /*var networkManagerObject = (networkDiscovery.gameObject
                                                    .GetComponent(Type.GetType("CustomNetworkManager"))
                                                    as CustomNetworkManager);*/
        CustomNetworkManagerDAO dao = new();
        var networkManagerObject = dao.GetCustomNetworkManager();
        if(networkManagerObject == null)
        {
            Debug.LogError("NETWORK MANAGER IS NULL");
        }
        Debug.Log("Active Scene: " + SceneManager.GetActiveScene().name);

        // Isolate the maze renderer object in the maze scene
        var mazeRendererObject = SceneManager.GetActiveScene()
                                .GetRootGameObjects()
                                .Select(x => 
                                    {
                                        Debug.Log(x.name);
                                        if(x.name.Contains("MazeRenderer")) 
                                            return x;
                                        else 
                                            return null;
                                    })
                                .FirstOrDefault(x => x != null);
        if(mazeRendererObject == null)
        {
            Debug.LogError("MAZE RENDERER GAMEOBJECT IS NULL");
        }
        
        // Isolate the RenderMaze script inside of the maze renderer to use in the network manager (the network manager and maze renderer both have references to the same RenderMaze script/object)
        var mazeRendererScript = mazeRendererObject.GetComponent(Type.GetType("RenderMaze")) as RenderMaze;
        if(mazeRendererScript == null)
        {
            Debug.LogError("MAZE RENDERER SCRIPT COULD NOT BE FOUND");
        }

        // Set the networkManager's maze renderer
        networkManagerObject.mazeRenderer = mazeRendererScript;

        // Start the server
        networkManagerObject.StartHost();
        networkDiscovery.AdvertiseServer();
    }
    #endregion Scene Management

    #region Client Side Functionality
    public Dictionary<long, ServerResponse> LookForOtherServers()
    {
        discoveredServers.Clear();
        networkDiscovery.StartDiscovery();
        return discoveredServers;
    }

    public void JoinServer(ServerResponse serverInfo, CustomNetworkManager networkManager)
    {
        networkDiscovery.StopDiscovery();
        networkManager.StartClient(serverInfo.uri);
    }
    #endregion Client Side Functionality
}
