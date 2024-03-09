using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

// This class manages the exit game interface
public class PauseGame : NetworkBehaviour
{
    public GameObject PauseCanvas;  // Exit menu game object
    public bool pauseCanvasIsEnabled = true;
    public GameObject runnerHTP;   // How to play popup for the runner
    public GameObject guardHTP;    // How to play popup for the guard

    // Start the game with the exit game menu invisble
    public void Start()
    {
        if(pauseCanvasIsEnabled)
            PauseCanvas.SetActive(false);
    }

    // Initialize the exit menu
    public void Awake()
    {
        //PauseCanvas = GameObject.Find("PauseCanvas");
        if(pauseCanvasIsEnabled)
        {
            PauseCanvas = FindPauseCanvas();
            if(PauseCanvas == null)
            {
                StartCoroutine(FindPauseCanvasAsync());
            }
            PauseCanvas.SetActive(true);
        }
    }

    // Open (or close) the exit game menu
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isLocalPlayer)
        {
            Debug.Log(this.gameObject.name + " reacted to escape key");
            //PauseCanvas.gameObject.SetActive(!PauseCanvas.gameObject.activeSelf);
            if(Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner How to Play")).activeSelf || Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Guard How to Play")).activeSelf){
                HideHowToPlay();
            }
            else{
                if(PauseCanvas.gameObject.activeSelf == false && pauseCanvasIsEnabled)
                {
                    OpenPauseCanvas();
                }
                else
                {
                    ClosePauseCanvas();
                } 
            }
        }
    }

    public void ManagePauseCanvas()
    {
        Debug.Log("Click detected");
        //if (isLocalPlayer)
        //{
            //Debug.Log("isLocalPlayer");
            if(PauseCanvas.gameObject.activeSelf == false)
            {
                Debug.Log("Opening pause canvas from pause button");
                OpenPauseCanvas();
            }
            else
            {
                Debug.Log("Closing pause canvas from pause button");
                ClosePauseCanvas();
            }
        //}
    }

    // Locate the exit game menu
    public GameObject FindPauseCanvas()
    {
        try
        {
            PauseCanvas = SceneManager.GetActiveScene()
                                .GetRootGameObjects()
                                .FirstOrDefault<GameObject>(x => x.name == "PauseCanvas").transform.GetChild(0).gameObject;
        }
        catch(Exception e)
        {
            //Debug.LogWarning("PauseGame: Exception: " + e + " Was caught in FindPauseCanvas. The operation is repeating");
            var scene = SceneManager.GetActiveScene();
            if(scene == null)
            {
                throw(new Exception("PauseGame: Active scene is null"));
            }
            var rootGameObjects = scene.GetRootGameObjects();
            if(rootGameObjects == null || rootGameObjects.Count() <= 0)
            {
                throw(new Exception("PauseGame: Root game objects of scene are null or short"));
            }

            var pauseParent = rootGameObjects.FirstOrDefault<GameObject>(x => x.name == "PauseCanvas");

            var pauseParent2 = pauseParent.transform.GetChild(0);

            if(pauseParent2 == null)
                throw(new Exception("PauseGame: Pauseparent2 is null"));
            
            PauseCanvas = pauseParent2.gameObject;
        }

        if(PauseCanvas == null)
        {
            throw(new Exception("Could not locate exit game menu"));
        }
        else
            return PauseCanvas;
    }

    public IEnumerator FindPauseCanvasAsync()
    {
        while(PauseCanvas == null)
        {
            try
            {
                PauseCanvas = FindPauseCanvas();
            }
            catch(Exception e)
            {
                Debug.LogWarning("PauseGame: Tried to find pause canvas in scene: " + SceneManager.GetActiveScene().name + " and failed");
            }
            yield return null;
        }
    }

    // Open the exit game menu
    public void OpenPauseCanvas()
    {
        if(pauseCanvasIsEnabled)
        {
            Debug.Log("Open Exit Menu");
            PauseCanvas.SetActive(true);
        }
    }

    // Close the exit game menu
    public void ClosePauseCanvas()
    {
        if(pauseCanvasIsEnabled)
        {
            Debug.Log("Close Exit Menu");
            PauseCanvas.SetActive(false);
        }
    }

    // Show the How To Play screen to the player
    public void ShowHowToPlay(){
        ClosePauseCanvas();
        if(CustomNetworkManager.isRunner){
            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner How to Play")).SetActive(true);
        }
        else{
            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Guard How to Play")).SetActive(true);
        }
    }

    // Hide the How To Play screen
    public void HideHowToPlay(){
        if(CustomNetworkManager.isRunner){
            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner How to Play")).SetActive(false);
        }
        else{
            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Guard How to Play")).SetActive(false);
        }
    }

    // Executed when the exit game button is pressed.
    public void ExitGame(int index)
    {
        CustomNetworkManager netManager = CustomNetworkManagerDAO.GetNetworkManagerGameObject().GetComponent<CustomNetworkManager>();
                                            // Reference to the network manager
        CustomNetworkDiscovery netDiscovery = netManager.GetComponent<CustomNetworkDiscovery>();
                                            // Reference to the discovery system
        
        // stop host if host mode
        if(CustomNetworkManager.isHost)
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                Debug.Log("I quit and I am the host");
                netManager.StopHost();
                netDiscovery.StopDiscovery();
            }
        }

        // stop client if client-only
        else if(NetworkClient.isConnected && !CustomNetworkManager.isHost)
        {
            Debug.Log("I quit and I am the client");
            netManager.StopClient();
            netDiscovery.StopDiscovery();
        }

        // If we aren't a client or a host but we can still call exit game, something is wrong so throw an error
        else
        {
            throw(new Exception("ExitGame(): Cannot exit, isHost" + CustomNetworkManager.isHost.ToString() + " and NetworkClient.isConnected is " + NetworkClient.isConnected.ToString()));
        }

        // Load into the offline scene
        SceneManager.LoadScene(index);
    }
}
