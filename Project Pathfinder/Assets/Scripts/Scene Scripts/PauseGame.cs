using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PauseGame : NetworkBehaviour
{
    public GameObject PauseCanvas;

    public void Start()
    {
        PauseCanvas.SetActive(false);
    }

    public void Awake()
    {
        //PauseCanvas = GameObject.Find("PauseCanvas");
        PauseCanvas = FindPauseCanvas();
        PauseCanvas.SetActive(true);
    }

    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && isLocalPlayer)
        {
            Debug.Log(this.gameObject.name + " reacted to escape key");
            //PauseCanvas.gameObject.SetActive(!PauseCanvas.gameObject.activeSelf);
            if(PauseCanvas.gameObject.activeSelf == false)
            {
                OpenPauseCanvas();
            }
            else
            {
                ClosePauseCanvas();
            }

        }
    }

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

    public void OpenPauseCanvas()
    {
        Debug.Log("Open Exit Menu");
        PauseCanvas.SetActive(true);
    }

    public void ClosePauseCanvas()
    {
        Debug.Log("Close Exit Menu");
        PauseCanvas.SetActive(false);
    }

    // Executed when the exit game button is pressed.
    public void ExitGame(int index)
    {
        CustomNetworkManager netManager = CustomNetworkManagerDAO.GetNetworkManagerGameObject().GetComponent<CustomNetworkManager>();
        CustomNetworkDiscovery netDiscovery = netManager.GetComponent<CustomNetworkDiscovery>();
        
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

        else
        {
            throw(new Exception("ExitGame(): Cannot exit, isHost" + CustomNetworkManager.isHost.ToString() + " and NetworkClient.isConnected is " + NetworkClient.isConnected.ToString()));
        }
        SceneManager.LoadScene(index);
    }
}
