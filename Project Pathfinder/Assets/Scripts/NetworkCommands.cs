using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System.Linq;

/*
    *This class is used to allow the client to command the host to do something
*/
public class NetworkCommands : NetworkBehaviour
{
    public CustomNetworkManagerDAO dao;             // The way to communicate with the network manager/server browser/discovery
    public static NetworkCommands instance;         // This is a really bad implementation of a singleton
    public static GameObject instanceGameObject;    // The game object of the really bad singleton implementation
    public static bool instanceSet = false;         // Has the singleton been created

    #region Misc.
    void  Awake() 
    {
        instanceGameObject = this.gameObject;
        instance = this;
        instanceSet = true;
        Debug.Log("instances set");
        
        LoadSceneParameters cmdSceneParams = new LoadSceneParameters(LoadSceneMode.Additive);
        var cmdScene = SceneManager.LoadScene("CommandScene", cmdSceneParams);
        
        SceneManager.MoveGameObjectToScene(instanceGameObject, cmdScene);
        
        RefreshInstances();
    }

    // Returns the singleton
    public static NetworkCommands GetNetworkCommands()
    {
        RefreshInstances();
        return instance;
    }

    // Refreshes the singleton if the reference somehow becomes null (which yes it does, like I said the singleton implementation was bad)
    public static void RefreshInstances()
    {
        instanceGameObject = SceneManager.GetSceneByName("CommandScene")
                                .GetRootGameObjects()
                                .FirstOrDefault<GameObject>(x => x.name
                                    .Contains("Commands"));
        if(instanceGameObject == null)
            Debug.LogError("instanceGameObject is null");

        instance = instanceGameObject.GetComponent<NetworkCommands>();
        if(instance == null)
            Debug.LogError("instance is null");
    }
    #endregion Misc.


}
