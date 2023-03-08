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

    // Locate the exit game menu
    public GameObject FindPauseCanvas()
    {
       PauseCanvas = SceneManager.GetActiveScene()
                                .GetRootGameObjects()
                                .FirstOrDefault<GameObject>(x => x.name == "PauseCanvas");
        if(PauseCanvas == null)
        {
            throw(new Exception("Could locate exit game menu"));
        }
        else
            return PauseCanvas;
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
                NetworkManager.singleton.StopHost();
                netDiscovery.StopDiscovery();
            }
        }

        // stop client if client-only
        else if(NetworkClient.isConnected && !CustomNetworkManager.isHost)
        {
            Debug.Log("I quit and I am the client");
            NetworkManager.singleton.StopClient();
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
