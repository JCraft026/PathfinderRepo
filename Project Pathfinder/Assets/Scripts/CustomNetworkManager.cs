using System.Collections;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;

/*
    *This class is responsible for all general networking that is not specifically covered by any other class
*/
public class CustomNetworkManager : NetworkManager
{
    #region Global Variables
    // Global Variables
    public static System.Random RandomNumberGenerator  = new System.Random();
                                            // Random number generator
    public static int InitialActiveGuardId = ManageActiveCharactersConstants.CHASER;
                                            // Guard ID of the initial active guard
    public static bool PlayerRoleSet       = false;
                                            // Status of player role being assigned
    public static bool IsRunner            = false;
                                            // User playing as Runner status (NOTE: not the same as hostIsRunner, this is used for the client to determine their team)
    public static bool IsHost;              // Each player will have this variable, it is set when you decide to join or jost a game
    
    public static PlayerProfile CurrentLogin = new PlayerProfile();

    [SerializeField]
    public ServerBrowserBackend Backend;    // References the ServerBrowserBackend, this is required when we join from the server browser

    [SerializeField]
    public RenderMaze MazeRenderer;         // Enables us to render the maze
    public WallStatus[,] ParsedMazeJson;
    public string MazeDataJson = null;
    public static bool SteamGeneratorsSpawned = false;
                                             // Status of steam generators being spawned in the scene
    public static bool HostIsFrozen = true; // Status of host movement being frozen
    public static bool ClientJoined = false; // Status of client joining game

    public RenderMaze GetMazeRendererSafely() 
    {
        if(MazeRenderer == null)
        {
            MazeRenderer = Backend.GetMazeRenderer();
            return MazeRenderer;
        }
        else
            return MazeRenderer;
    }

    [SerializeField]
    public bool hostIsRunner;               // Used to determine if the host is the runner or not

    public bool IsHostRunner { get => hostIsRunner; }
                                            // Required for CustomNetworkDiscovery to advertise which team the client will join as
    #endregion

    #region Client Only Code

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        // Set who the runner is
        if(hostIsRunner && IsHost)
        {
            Debug.Log("isRunner=true");
            IsRunner = true;
        }
        else if(!hostIsRunner && IsHost)
        {
            Debug.Log("isRunner=false");
            IsRunner = false;
        }
        else if(hostIsRunner && !IsHost)
        {
            Debug.Log("isRunner=false");
            IsRunner = false;
        }
        else if(!hostIsRunner && !IsHost)
        {
            Debug.Log("isRunner=true");
            IsRunner = true;
        }

        // Find the maze renderer and create the maze (if we are the host) (Might be a good idea to move this ServerBrowserBackend.LoadMazeAsync())
        if (NetworkServer.connections.Count == 1){
            Resources.FindObjectsOfTypeAll<GameObject>()
                .FirstOrDefault(gObject => gObject.name.Contains("MazeRenderer"))
                .GetComponent<RenderMaze>()
                .CreateMaze();
        }

        // Reflect that the runner/guard master status has been set
        PlayerRoleSet = true;

        // Stop the menu music
        //GetComponent<AudioSource>().Stop();
    }

    // Runs on the client once connected to the server - registers the message handler for the maze data
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        NetworkClient.RegisterHandler<MazeMessage>(ReceiveMazeData);
        NetworkClient.RegisterHandler<AnimationMessage>(NetworkAnimationHandler);
    }

    //Called when the client receives the json text of the maze
    public void ReceiveMazeData(MazeMessage mazeText)
    {
        // Save the mazeText in case we need to regenerate the maze
        MazeDataJson = mazeText.jsonMaze;

        // Don't run this code if the server is also a client as it will cause the maze to double render
        if(!NetworkClient.isHostClient)
        {
            try
            {
                if(mazeText.jsonMaze == null)
                    throw(new Exception("CustomNetworkManager: mazeText.jsonMaze == null, no data sent!"));
                else
                {
                    // The mazeRenderer will probably be null for the incoming client so we'll need to locate it when we join a server
                    if(MazeRenderer == null)
                    {
                        MazeRenderer = Backend.GetMazeRenderer(); // MazeRenderer not loading problem is here - its searching the lobby scene
                        if(MazeRenderer == null)
                            throw(new Exception("CustomNetworkManager: mazeRenderer is still null"));
                    }

                    // Clean the old map and render the new map
                    WallStatus[,] newMaze = JsonConvert.DeserializeObject<WallStatus[,]>(mazeText.jsonMaze); //If mazeText.jsonMaze == null major issues occur
                    ParsedMazeJson = newMaze;
                    MazeRenderer.CleanMap();
                    MazeRenderer.SetMazeDataJson(mazeText.jsonMaze);
                    MazeRenderer.Render(newMaze);
                }
            }
            // Any exceptions regarding
            catch(Exception e)
            {
                Debug.LogError("There was a problem decoding and/or rendering mazeText.jsonMaze resulting in the exception: " + e.Message);

                // If we are not hosting, find the maze generator and generate the maze
                if(mazeText.jsonMaze != null)
                    StartCoroutine(Backend.GetMazeRendererAsync());
            }
        }
    }

    // Fires on the ServerBrowserBackend when GetMazeRendererAsync completes. Generates the maze for the client (hopefully)
    public void OnMazeRendererAsyncComplete()
    {
        Debug.Log("GetMazeRendererAsync completed");
        WallStatus[,] newMaze = JsonConvert.DeserializeObject<WallStatus[,]>(MazeDataJson);
        ParsedMazeJson = newMaze;
        MazeRenderer.CleanMap();
        MazeRenderer.Render(newMaze);
    }

    // Shuts down the client and the host
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        StopHost();
        Debug.Log("OnClientDisconnect");
        ResetVariables();
        ResetClientVariables();
    }

    #endregion

    #region Server Only Code
    // Fires when the client disconnects, forces the host to end the game.
    // Change the offline scene to change the scene the host is transferred to
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        StopHost();
        this.gameObject.GetComponent<CustomNetworkDiscovery>().StopDiscovery();
        SceneManager.LoadScene(offlineScene);
        Debug.Log("OnServerDisconnect");
        ResetVariables();
    }

    // Runs on the server when a client connects
    // Sends the maze to the client from the server
    // Also registers the animation handlers for each player
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        try
        {
            if(MazeRenderer == null)
                Debug.Log("CustomNetworkManager OnserverConnect(): mazeRenderer was null");

            MazeMessage mazeMessage = new()
            {
                jsonMaze = MazeRenderer.GiveMazeDataToNetworkManager() 
            };

            if(mazeMessage.jsonMaze != null)
                conn.Send(mazeMessage);
            else
                Debug.Log("CustomNetworkManager OnServerConnect(): mazeMessage.jsonMaze == null, mazeMessage not being sent to client");
            
        }
        catch(Exception e)
        {
            Debug.LogError("CustomNetworkManager OnServerConnect()" + e);
        }
    }
    
    // Responsible for initial set up of clients (ie. Getting players on the right teams, spawning their characters, etc.)
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log("CustomNetworkManager: OnServerConnect");

        // Determine who is on what team
        if((hostIsRunner && NetworkServer.connections.Count > 1) ||
            (!hostIsRunner && NetworkServer.connections.Count == 1))
        {
            GameObject oldPlayer = conn.identity.gameObject;

            GameObject chaser = Instantiate(spawnPrefabs.FirstOrDefault(prefab => prefab.name.Contains("Chaser")));
            GameObject engineer = Instantiate(spawnPrefabs.FirstOrDefault(prefab => prefab.name.Contains("Engineer")));
            GameObject trapper = Instantiate(spawnPrefabs.FirstOrDefault(prefab => prefab.name.Contains("Trapper")));
            
            // Set guard spawn locations
            SetGuardSpawnLocations();

            // Spawn the guards and assign them client authority
            NetworkServer.Spawn(chaser);
            NetworkServer.Spawn(engineer);
            NetworkServer.Spawn(trapper);

            // Set the player as the chaser
            chaser.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
            NetworkServer.ReplacePlayerForConnection(conn, chaser, true);

           Destroy(oldPlayer);

            Debug.Log("CustomNetworkManager OnServerAddPlayer():  Replaced conID: " + conn.connectionId);
        }

        if(NetworkServer.connections.Count > 1)
        {
            ItemWorld.SpawnChests(27);
            ItemWorld.SpawnKeys();
            RenderMaze.RenderSteamGenerators();
            SteamGeneratorsSpawned = true;
            ClientJoined = true;
        }
        
        // Make the player wait to move until a client joins the game
        if(IsHost)
            StartCoroutine(HostWaitForPlayer(conn));
        
    }

    public override void OnStopHost()
    {
        base.OnStopHost();

        // Reset the end game events
        HandleEvents.endGameEvent = 0;
        HandleEvents.currentEvent = 0;
    }
    #endregion

    #region Game Events And Misc. Handlers
    // Changes the active guard for the guard master (WARNING: DEPRECATED)
    public static void ChangeActiveGuard(NetworkConnectionToClient conn, int nextActiveGuardId)
    {
        string currentActiveGuard = conn.identity.gameObject.name; // Name of the current active guard object
        Debug.Log("CustomNetworkManager ChangeActiveGuard(): currentActiveGuard = " + currentActiveGuard);
        GameObject newGuardObject;                                 // Result of the guard query
        Debug.Log("CustomNetworkManager ChangeActiveGuard(): switch nextActiveGuardId = " + nextActiveGuardId.ToString());

        // Get the next guard's game object and update the active guard identification number
        switch (nextActiveGuardId)
        {
            case ManageActiveCharactersConstants.CHASER:
                newGuardObject = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser(Clone)"));
                break;
            case ManageActiveCharactersConstants.ENGINEER:
                newGuardObject = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Engineer(Clone)"));
                break;
            case ManageActiveCharactersConstants.TRAPPER:
                newGuardObject = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper(Clone)"));
                break;
            default:
                newGuardObject = null;
                Debug.LogError("CustomNetworkManager ChangeActiveGuard(): newGuardObject is null");
                break;
        }

        // Switch guard control from the old guards object to the next guard's object
        if(newGuardObject != null)
            NetworkServer.ReplacePlayerForConnection(conn, newGuardObject, true);
        
        else
            Debug.LogWarning("CustomNetworkManager ChangeActiveGuard(): Could not find a new guard to switch to!");
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
            switch (RandomNumberGenerator.Next(1,4))
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
            switch (RandomNumberGenerator.Next(1,4))
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
            switch (RandomNumberGenerator.Next(1,4))
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

    //Ensure the host cannot play the game while there are no clients connected
    IEnumerator HostWaitForPlayer(NetworkConnectionToClient host)
    {
        Debug.Log("CustomNetworkManager HostWaitForPlayer(): Stopping player movement until a client joins...");
        GameObject hostObject = host.identity.gameObject;
        bool popupDisplayed   = false;

        // Disable popup message
        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("WaitingForOpponent")).SetActive(false);
        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("OpponentJoined")).SetActive(false);

        // Disable movement for the player
        if(IsRunner)
            hostObject.GetComponent<MoveCharacter>().enabled = false;
        else
        {
            GameObject chaser   = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser"));
            GameObject engineer = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Engineer"));
            GameObject trapper  = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper"));

            chaser.GetComponent<MoveCharacter>().enabled = false;
            engineer.GetComponent<MoveCharacter>().enabled = false;
            trapper.GetComponent<MoveCharacter>().enabled = false;
        }
        
        // Display waiting popup for host
        if(NetworkServer.connections.Count <= 1)
        {
            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("WaitingForOpponent")).SetActive(true);
            popupDisplayed = true;
            Debug.Log("Popup Displayed");
        }

        // Wait for a client to join
        while(NetworkServer.connections.Count <= 1)
        {
            yield return null;
        }
        
        // Enable player movement
        if(IsRunner)
            hostObject.GetComponent<MoveCharacter>().enabled = true;
        else
        {
            GameObject chaser   = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser(Clone)"));
            GameObject engineer = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Engineer(Clone)"));
            GameObject trapper  = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper(Clone)"));

            chaser.GetComponent<MoveCharacter>().enabled = true;
            engineer.GetComponent<MoveCharacter>().enabled = true;
            trapper.GetComponent<MoveCharacter>().enabled = true;
        }

        Debug.Log("CustomNetworkManager HostWaitForPlayer(): Player movment is now re-enabled");

        if(popupDisplayed)
        {
            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("WaitingForOpponent")).SetActive(false);
            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("OpponentJoined")).SetActive(true);
            yield return new WaitForSeconds(2);
            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("OpponentJoined")).SetActive(false);
            Debug.Log("Popup Removed");
        }
        
        HostIsFrozen = false;

        yield return null;
    }

    public void ResetVariables()
    {
        // Static variables
        SteamGeneratorsSpawned = false;
        PlayerRoleSet = false;
        ClientJoined = false;
        HostIsFrozen = true;

        // Non-statics
        MazeRenderer = null;

        //GenerateSteam
        GenerateSteam.steam = 0;

        //RenderSmokeScreen
        RenderSmokeScreen.smokeScreensSpawned = 0;
    }

    public void ResetClientVariables()
    {
        // Reset chest RNG variables
        Item.greenScreenSpawnLimit = Item.initialGSSpawnLimit;
        Item.smokeBombSpawnLimit = Item.initialSBSpawnLimit;
        Item.coffeeSpawnLimit = Item.initialCFSpawnLimit;
        Item.sledgehammerSpawnLimit = Item.initialSHSpawnLimit;
        Item.empSpawnLimit = Item.initialEMPSpawnLimit;

        // Utilities
        Utilities.ClearObjectLibrary();
        Utilities.runner = null;
        Utilities.chaser = null;
        Utilities.engineer = null;
        Utilities.trapper = null;
    }
   
    // Originally was supposed to handle animations but it needs to be empty for some reason
    public void NetworkAnimationHandler(AnimationMessage animationState)
    {
        //This empty function is required for the networked animations to run... I don't know why and I'm scared to ask!
    }
    #endregion

    #region Message Structures
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
        public bool attack;
        public bool hurt;
        public float impactDirection;
    }
    #endregion
}
