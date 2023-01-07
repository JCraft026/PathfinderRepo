using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror.Discovery;
using UnityEngine;

public class CustomNetworkManagerDAO : MonoBehaviour
{
    private static GameObject NetworkManagerGameObject;

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
    public void ServerBrowserStartHosting()
    {
        GetServerBrowserBackend().StartHosting();
    }

    // Probably needs changed but tells the backend to look for servers
    public Dictionary<long, ServerResponse> SearchForServers()
    {
        return GetServerBrowserBackend().LookForOtherServers();
    }

    public void ServerBrowserJoinServer(ServerResponse serverInfo)
    {
        GetServerBrowserBackend().JoinServer(serverInfo, GetCustomNetworkManager());
    }

    #endregion ServerBrowserBackend communication
}
