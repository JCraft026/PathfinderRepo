using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror.Discovery;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
    *This class is used to grab references to network components when a gameobject does not have a way to access said component
    *This class can also be used to give common commands to said network components without requiring a reference to be returned
*/
public class CustomNetworkManagerDAO : MonoBehaviour
{
    private static GameObject NetworkManagerGameObject;         // The custom network manager's gameobject (note this is the gameobject containing the actual script)
    private Dictionary<long, ServerResponse> FoundServersCache; // Contains all the servers from the last search

    // Makes sure the CustomNetworkManager singleton is up to date
    private static void RefreshSingletonReference()
    {
        if(CustomNetworkManager.singleton == null)
        {
            Debug.LogError("singleton null");
        }
        NetworkManagerGameObject = CustomNetworkManager.singleton.gameObject;
    }

    #region Getters for the CustomNetworkManager
    // Return a reference to the CustomNetworkManager game object
    public static GameObject GetNetworkManagerGameObject()
    {
        RefreshSingletonReference();
        return NetworkManagerGameObject;
    }

    // Return a reference to the CustomNetworkManager its self
    public CustomNetworkManager GetCustomNetworkManager()
    {
        RefreshSingletonReference();
        return NetworkManagerGameObject.GetComponent(Type.GetType("CustomNetworkManager"))
            as CustomNetworkManager;
    }

    // Return a reference to the CustomNetworkDiscovery
    public CustomNetworkDiscovery GetCustomNetworkDiscovery()
    {
        RefreshSingletonReference();
        return NetworkManagerGameObject.GetComponent(Type.GetType("CustomNetworkDiscovery"))
            as CustomNetworkDiscovery;
    }

    // Return a reference to the ServerBrowserBackend
    public ServerBrowserBackend GetServerBrowserBackend()
    {
        RefreshSingletonReference();
        return NetworkManagerGameObject.GetComponent(Type.GetType("ServerBrowserBackend"))
            as ServerBrowserBackend;
    }
    #endregion Getters for the CustomNetworkManager

    #region ServerBrowserBackend communication

    // Commands the ServerBrowserBackend to start hosting a game
    public void ServerBrowserStartHosting(bool hostIsRunner)
    {
        GetCustomNetworkManager().hostIsRunner = hostIsRunner;
        UpdateServerName();
        GetServerBrowserBackend().StartHosting();
    }

    // Sets the name that should be advertised on the server browser (NOTE: The name can only come from the dropdown box in the host options screen)
    public void UpdateServerName()
    {
        var dropdownAdjective1 = GameObject.Find("Dropdown (Adjective1)").GetComponent<TMPro.TMP_Dropdown>();
        var dropdownAdjective2 = GameObject.Find("Dropdown (Adjective2)").GetComponent<TMPro.TMP_Dropdown>();
        var dropdownTitle      = GameObject.Find("Dropdown (Title)"     ).GetComponent<TMPro.TMP_Dropdown>();
        
        GetServerBrowserBackend().serverName
        =       dropdownAdjective1.captionText.text
        + " " + dropdownAdjective2.captionText.text
        + " " + dropdownTitle.captionText.text;
    }

    // Tells the backend to look for servers
    public Dictionary<long, ServerResponse> SearchForServers()
    {
        return GetServerBrowserBackend().LookForOtherServers();
    }

    // Used for button presses to tell the backend to look for servers (button presses require void functions)
    public void StartClientSearching()
    {
        FoundServersCache = SearchForServers();
    }

    // Updates the server cache and returns it
    public Dictionary<long, ServerResponse> GetServerCache()
    {
        if(FoundServersCache == null)
        {
            StartClientSearching();
        }
        return FoundServersCache;
    }
    
    #endregion ServerBrowserBackend communication
}
