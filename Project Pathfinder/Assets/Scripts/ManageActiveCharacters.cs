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
    public GameObject cameraHolder;
    public Vector3 offset;
    public int guardId;
    public int activeGuardId;
    public int nextActiveGuardId;
    Regex runnerExpression = new Regex("Runner");
    public bool isRunner   = false;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        if(runnerExpression.IsMatch(gameObject.name)){
            cameraHolder.SetActive(true);
        }
        else{
            InitializeGuardIdentification();
            activeGuardId = CustomNetworkManager.activeGuardId;
            if(guardId == activeGuardId){
                cameraHolder.SetActive(true);
            }
        }
    }

    public void Update()
    {
        if(Input.GetKeyDown("space")){
            if(activeGuardId == 3){
                nextActiveGuardId = 1;
            }
            else{
                nextActiveGuardId++;
            }
            if(guardId == activeGuardId){
                cameraHolder.SetActive(false);
                CustomNetworkManager.ChangeActiveGuard(this.netIdentity.connectionToClient, nextActiveGuardId);
            }
            else if(guardId == nextActiveGuardId){
                cameraHolder.SetActive(true);
            }
            activeGuardId = nextActiveGuardId;
        }
        cameraHolder.transform.position = transform.position + offset;
    }

    // Assign the appropriate guard ID to the guard script owner
    public void InitializeGuardIdentification(){
        Regex chaserExpression   = new Regex("Chaser");
        Regex engineerExpression = new Regex("Engineer");
        Regex trapperExpression  = new Regex("Trapper");

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
