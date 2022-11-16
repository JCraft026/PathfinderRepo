using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using Mirror;

static class ManageActiveCharactersConstants{
    public const int CHASER   = 1; // Chaser guard ID
    public const int ENGINEER = 2; // Engineer guard ID
    public const int TRAPPER  = 3; // Trapper guard ID
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
            activeGuardId = CustomNetworkManager.activeGuardId;
        }
    }

    // Run when Object is given authority
    public override void OnStartAuthority(){
        base.OnStartAuthority();

        // If the parent object is the runner, enable its camera and update the isRunner status to label proceeding actions as client side actions
        if(runnerExpression.IsMatch(gameObject.name)){
            cameraHolder.SetActive(true);
            CustomNetworkManager.isRunner = true;
        }

        // If the parent object is the initial active guard, enable its camera and update the isRunner status to label proceeding actions as host side actions
        else if(guardId == activeGuardId){
            cameraHolder.SetActive(true);
            CustomNetworkManager.isRunner = false;
        }
    }

    // Run on every frame
    public void Update()
    {
        // If the user hits space, and is playing as the guard master, process switching guard control to the next guard
        if(Input.GetKeyDown("space") && CustomNetworkManager.isRunner == false && !runnerExpression.IsMatch(gameObject.name)){
            if(activeGuardId == 3){
                nextActiveGuardId = 1;
            }
            else{
                nextActiveGuardId = activeGuardId + 1;
            }
            // If the parent object is the current active guard, disable its camera and give control to the next active guard
            if(guardId == activeGuardId){
                cameraHolder.SetActive(false);
                CustomNetworkManager.ChangeActiveGuard(this.netIdentity.connectionToClient, nextActiveGuardId);
            }
            // If the parent object is the next active guard, enable the camera
            else if(guardId == nextActiveGuardId){
                cameraHolder.SetActive(true);
            }
            activeGuardId = nextActiveGuardId;
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
}
