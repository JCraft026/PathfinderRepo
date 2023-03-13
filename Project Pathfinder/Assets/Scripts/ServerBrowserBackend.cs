using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Mirror.Discovery;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
    *This class provides us with the ability to host games, join games, and create games all from the server browser menu
*/
public class ServerBrowserBackend : MonoBehaviour
{
    #region Global Variables
    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
                                                    // List of servers discovered to return to the frontend
    Vector2 scrollViewPos = Vector2.zero;           // I don't know what this is doing here, this is something that frontend will handle
    public CustomNetworkDiscovery networkDiscovery; // Allows the server browser to detect open games and connect to them
    private const int LOAD_MAZE_SCENE_INDEX = 5;    // Build index for the LoadMaze scene, this is subject to change in the future
    public string serverName;                       // The name the server will use when advertising its self to potential clients
    
    #endregion

    #region Hosting
    // Launch the maze renderer scene and then host the game
    public void StartHosting()
    {
        discoveredServers.Clear(); // We might as well wipe out the old servers that we won't need anymore once we start hosting
        CustomNetworkManager.isHost = true; // Set the network manager to run in host mode

        // This needs to run within a coroutine as it is a thread safe version of "async" for unity
        StartCoroutine(LoadMazeSceneAsync(true));
    }
    #endregion Hosting

    #region Scene Management
    
    // Load the "LoadMaze" scene asyncrhonously and set it up with the network manager
    IEnumerator LoadMazeSceneAsync(bool isHost)
    {
        // Start the loading process
        Debug.Log("Loading Scene #... " + LOAD_MAZE_SCENE_INDEX);
        AsyncOperation loading = SceneManager.LoadSceneAsync(LOAD_MAZE_SCENE_INDEX); 
        loading.allowSceneActivation = false;

        // Wait for the scene to be loaded
        while(!loading.isDone)
        {
            Debug.Log("Scene Loading Progress: " + loading.progress);

            // Check if the scene is done loading
            if(loading.progress >= .9f)
            {
                loading.allowSceneActivation = true;
            }
            yield return null;
        }
        Debug.Log("Scene done loading (progress): " + loading.progress);

        // Isolate the network manager
        var networkManagerObject = (networkDiscovery.gameObject
                                                    .GetComponent(Type.GetType("CustomNetworkManager"))
                                                    as CustomNetworkManager);
        if(networkManagerObject == null)
        {
            Debug.LogError("ServerBrowserBackend: NETWORK MANAGER IS NULL");
        }
        Debug.Log("Active Scene: " + SceneManager.GetActiveScene().name);

        // Set the networkManager's maze renderer
        networkManagerObject.mazeRenderer = null; //Used to ensure that the mazeRenderer from an old session is not still being referenced
        while(networkManagerObject.mazeRenderer == null)
        {
            yield return null;
            try{
                networkManagerObject.mazeRenderer = GetMazeRenderer();
            }
            catch{
                Debug.LogWarning("ServerBrowserBackend: Failed to get MazeRenderer - attempting again next frame");
            }
        }
        
        // Start the server if we are a host
        if(isHost)
        {
            Debug.Log("I am hosting");
            networkManagerObject.StartHost();
            networkDiscovery.AdvertiseServer();
        }
    }

    //Grab the mazeRenderer script from the gameplay scene (LoadMaze)
    public RenderMaze GetMazeRenderer()
    {
        Debug.Log("ServerBrowserBackend: Searching for renderer in scene: " + SceneManager.GetActiveScene().name);
        try
        {
            // Isolate the maze renderer object in the maze scene
            GameObject mazeRendererObject = null;
            mazeRendererObject = SceneManager.GetActiveScene()
                                     .GetRootGameObjects()
                                     .Select(x => 
                                         {
                                             Debug.Log("GetMazeRenderer Searching in: " + x.name);
                                             if(x.name.Contains("MazeRenderer")) 
                                                 return x;
                                             else 
                                                 return null;
                                         })
                                     .FirstOrDefault(x => x != null);
             if(mazeRendererObject == null)
             {
                 Debug.LogError("ServerBrowserBackend: MAZE RENDERER GAMEOBJECT IS NULL");
             }
        
            // Isolate the RenderMaze script inside of the maze renderer to use in the network manager (the network manager and maze renderer both have references to the same RenderMaze script/object)
            var mazeRendererScript = mazeRendererObject.GetComponent<RenderMaze>();
            if(mazeRendererScript == null)
            {
                Debug.LogError("ServerBrowserBackend: MAZE RENDERER SCRIPT COULD NOT BE FOUND");
            }

            // Set the networkManager's maze renderer
            return mazeRendererScript;
        }
        catch(Exception e)
        {
            Debug.Log("ServerBrowserBackend: Failed to find maze renderer");
            Debug.LogWarning("ServerBrowserBackend: Failed to find maze renderer with exception: " + e);
            return null;
        }
    }

    public IEnumerator GetMazeRendererAsync()
    {
        var CusNetMan = CustomNetworkManagerDAO.GetNetworkManagerGameObject().GetComponent<CustomNetworkManager>();

        while(!SceneManager.GetActiveScene().name.Contains("Maze"))
        {
            yield return null;
            Debug.Log("Still in lobby scene...");
        }
        CusNetMan.mazeRenderer = SceneManager.GetActiveScene()
                                     .GetRootGameObjects()
                                     .Select(x => 
                                         {
                                             Debug.Log("GetMazeRenderer Searching in: " + x.name);
                                             if(x.name.Contains("MazeRenderer")) 
                                                 return x;
                                             else 
                                                 return null;
                                         })
                                     .FirstOrDefault(x => x != null).GetComponent<RenderMaze>();
        CusNetMan.OnMazeRendererAsyncComplete();
    }
    #endregion Scene Management

    #region Client Side Functionality

    // Search for other servers and return them as a dictionary
    public Dictionary<long, ServerResponse> LookForOtherServers()
    {

        // Wipe out the servers we found last time, search for new ones
        discoveredServers.Clear();
        networkDiscovery.StartDiscovery();
        Debug.Log("LookForOtherServers");

        Debug.Log("Found " + discoveredServers.Count + " Servers");
        foreach(var x in discoveredServers)
        {
            Debug.Log("Server ID: " + x.Value.serverId);
        }
        Debug.Log("End server logs");
        return discoveredServers;
    }

    // Join the specified server
    public void JoinServer(ServerResponse serverInfo, CustomNetworkManager networkManager, bool isHostRunnerFromHost)
    {
        // Tell the network manager to run in client mode and figure out what team we are on
        CustomNetworkManager.isHost = false;
        networkManager.hostIsRunner = isHostRunnerFromHost;
        CustomNetworkManager.isRunner = !isHostRunnerFromHost;

        // Stop searching for new servers
        networkDiscovery.StopDiscovery();

        // Join the selected server
        networkManager.StartClient(serverInfo.uri);

        // Load the maze from the host
        StartCoroutine(LoadMazeSceneAsync(false));
    }
    #endregion Client Side Functionality
}
