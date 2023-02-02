using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using Mirror;
using System;
using System.Linq;

static class ManageActiveCharactersConstants{
    public const int RUNNER   = 0; // Runner character ID
    public const int CHASER   = 1; // Chaser character ID
    public const int ENGINEER = 2; // Engineer character ID
    public const int TRAPPER  = 3; // Trapper character ID
}

public class ManageActiveCharacters : NetworkBehaviour
{
    public GameObject cameraHolder;               // Parent object camera
    public Vector3 offset;                        // Camera position offset
    public int guardId;                           // Parent object's guard ID
    public int activeGuardId;                     // Guard ID of the current active guard
    public int nextActiveGuardId;                 // Guard ID of the next active guard
    Regex runnerExpression = new Regex("Runner"); // Match "Runner"

    // Run when object is created
    public void Start(){
        // If the parent object is a guard, initialize its cooresponding guard ID and get the ID of the current active guard
        if(!runnerExpression.IsMatch(gameObject.name)){
            InitializeGuardIdentification();
            activeGuardId = CustomNetworkManager.initialActiveGuardId;
        }
    }

    // Run when Object is given authority
    public override void OnStartAuthority(){
        base.OnStartAuthority();

        // If the parent object is the runner, enable its camera and update the isRunner status to label proceeding actions as client side actions
        if(runnerExpression.IsMatch(gameObject.name)){
            cameraHolder.SetActive(true);
            SetUICamera(cameraHolder.transform.Find("Camera").gameObject.GetComponent<Camera>());
            CustomNetworkManager.isRunner = true; //these were commented out before
        }

        // If the parent object is the initial active guard, enable its camera and update the isRunner status to label proceeding actions as host side actions
        else if(guardId == activeGuardId){
            cameraHolder.SetActive(true);
            SetUICamera(cameraHolder.transform.Find("Camera").gameObject.GetComponent<Camera>());
            CustomNetworkManager.isRunner = false; //these were commented out before
        }
    }

    // Run on every frame
    public void Update()
    {
        // If the user hits the space key, and is playing as the guard master, process switching guard control to the next guard
        if(Input.GetKeyDown("space") && CustomNetworkManager.isRunner == false && !runnerExpression.IsMatch(gameObject.name)){
            Debug.Log("active guard ID: " + activeGuardId.ToString());
            Debug.Log("next guard ID: " + nextActiveGuardId.ToString());
            if(activeGuardId == 3 || activeGuardId < 1){
                nextActiveGuardId = 1;
            }
            else{
                nextActiveGuardId = activeGuardId + 1;
            }

            // If the parent object is the current active guard, disable its camera and give control to the next active guard
            if(guardId == activeGuardId){
                cameraHolder.SetActive(false);
                ChangeActiveGuard(this.netIdentity, nextActiveGuardId);
            }

            // If the parent object is the next active guard, enable the camera
            else if(guardId == nextActiveGuardId){
                cameraHolder.SetActive(true);
                SetUICamera(cameraHolder.transform.Find("Camera").gameObject.GetComponent<Camera>());
            }
            activeGuardId = nextActiveGuardId;
            if(activeGuardId == 0)
            {
                Debug.LogError("activeGuardID == 0");
            }
        }
        cameraHolder.transform.position = transform.position + offset;
    }

    // Assign the appropriate guard ID to the guard script owner
    public void InitializeGuardIdentification(){
        Regex chaserExpression   = new Regex("Chaser");   // Match "Chaser"
        Regex engineerExpression = new Regex("Engineer"); // Match "Engineer"
        Regex trapperExpression  = new Regex("Trapper");  // Match "Trapper"

        // Assign the appropirate guard identification number to the parent object
        if(chaserExpression.IsMatch(gameObject.name)){
            guardId = ManageActiveCharactersConstants.CHASER;
        }
        else if(engineerExpression.IsMatch(gameObject.name)){
            guardId = ManageActiveCharactersConstants.ENGINEER;
        }
        else if(trapperExpression.IsMatch(gameObject.name)){
            guardId = ManageActiveCharactersConstants.TRAPPER;
        }
    }

    // Set the render camera for the UI canvas
    public void SetUICamera(Camera camera){
        Canvas canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>(); // UI Canvas

        canvas.worldCamera = camera;
    }

    [Command]
    public void ChangeActiveGuard(NetworkIdentity conn, int nextActiveGuardId)
    {
        if(conn == null)
        {
            throw(new Exception("NetworkConnectionToClient is null"));
        }
        string currentActiveGuard = conn.connectionToClient.identity.gameObject.name; // Name of the current active guard object
        GameObject newGuardObject;                                 // Result of the guard query
        Debug.Log("nextActiveGuardId in ChangeActiveGuard(): " + nextActiveGuardId.ToString());
        if(nextActiveGuardId < 1 || nextActiveGuardId > 3)
        {
            Debug.LogWarning("nextActiveGuardId is out of bounds (nextActiveGuardId = " + nextActiveGuardId.ToString() + ")");
        }
        // Get the next guard's game object and update the active guard identification number
        switch (nextActiveGuardId)
        {
            case ManageActiveCharactersConstants.CHASER:
                newGuardObject = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser"));
                break;
            case ManageActiveCharactersConstants.ENGINEER:
                newGuardObject = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Engineer"));
                break;
            case ManageActiveCharactersConstants.TRAPPER:
                newGuardObject = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper"));
                break;
            default:
                newGuardObject = null;
                break;
        }

        // Switch guard control from the old guards object to the next guard's object
        if(newGuardObject != null)
        {
            NetworkServer.ReplacePlayerForConnection(conn.connectionToClient, newGuardObject);
        }
        else
        {
            Debug.LogWarning("Could not find a new guard to switch to!");
        }
    }

}
