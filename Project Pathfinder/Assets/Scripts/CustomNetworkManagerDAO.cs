using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror.Discovery;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CustomNetworkManagerDAO : MonoBehaviour
{
    private static GameObject NetworkManagerGameObject; // The custom network manager's gameobject (note this is the gameobject containing the actual script)
    private Dictionary<long, ServerResponse> FoundServersCache; // Contains all the servers from the last search

    // Make sure the CustomNetworkManager singleton is up to date
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
    public void ServerBrowserStartHosting(bool hostIsRunner)
    {
        GetCustomNetworkManager().hostIsRunner = hostIsRunner;
        GetServerBrowserBackend().StartHosting();
    }

    public void UpdateServerName()
    {
        var dropdown = gameObject.GetComponent<TMPro.TMP_Dropdown>();
        if(dropdown == null)
        {
            Debug.LogError("Dropdown is null");
        }
        Debug.Log(dropdown.captionText.text);
        GetServerBrowserBackend().serverName = dropdown.captionText.text;
    }

    // Probably needs changed but tells the backend to look for servers
    public Dictionary<long, ServerResponse> SearchForServers()
    {
        return GetServerBrowserBackend().LookForOtherServers();
    }

    // Used for button presses to tell the backend to look for servers (button presses require void functions)
    public void StartClientSearching()
    {
        FoundServersCache = SearchForServers();
    }

    public Dictionary<long, ServerResponse> GetServerCache()
    {
        if(FoundServersCache == null)
        {
            StartClientSearching();
        }
        return FoundServersCache;
    }

    // Tell the backend to join the specified server
    public void ServerBrowserJoinServer(ServerResponse serverInfo)
    {
        GetServerBrowserBackend().JoinServer(serverInfo, GetCustomNetworkManager());
    }
    #endregion ServerBrowserBackend communication
}
