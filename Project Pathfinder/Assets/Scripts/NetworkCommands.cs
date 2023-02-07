using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System.Linq;

public class NetworkCommands : NetworkBehaviour
{
    public CustomNetworkManagerDAO dao;
    public static NetworkCommands instance;
    public static GameObject instanceGameObject;
    public static bool instanceSet = false;

    #region Client To Server Commands

    [Command]
    public void IsHostRunnerFromClient()
    {
        CustomNetworkManager.IsServerRunnerMessage message = new();
        //message.isServerRunner = dao.GetCustomNetworkManager().IsHostRunner();
        message.isServerRunner = CustomNetworkManager.isRunner;
        NetworkServer.SendToAll<CustomNetworkManager.IsServerRunnerMessage>(message);
    }

    #endregion Client To Server Commands

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

    public static NetworkCommands GetNetworkCommands()
    {
        RefreshInstances();
        return instance;
    }

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
