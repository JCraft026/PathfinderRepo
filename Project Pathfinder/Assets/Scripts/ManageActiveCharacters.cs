using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

static class ManageActiveCharactersConstants{
    public const int ENGINEER = 1; // Engineer guard ID
    public const int MECHANIC = 2; // Mechanic guard ID
    public const int TRAPPER  = 3; // Trapper guard ID
}

public class ManageActiveCharacters : NetworkBehaviour
{
    public GameObject cameraHolder;
    public Vector3 offset;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        /*
        if(isRunner){
            cameraHolder.SetActive(true);
        }
        else{
            InitializeGuardIdentification();
            activeGuardId = CustomNetwoorkManager.initalActiveGuardId
            if(guardId == activeGuardId){
                cameraHolder.SetActive(true);
            }
        }
        */
        cameraHolder.SetActive(true);
    }

    public void Update()
    {
        /*
        if(Input.GetKeyDown("space")){
            if(activeGuardId == 3){
                nextActiveGuardId = 1;
            }
            else{
                nextActiveGuardId++;
            }
            if(guardId == activeGuardId){
                cameraHolder.SetActive(false);
            }
            else if(guardId == nextActiveGuardId){
                CustomNetorkManager.ChangeActiveGuard(activeGuardId, nextActiveGuardId);
                cameraHolder.SetActive(true);
            }
            activeGuardId = nextActiveGuardId;
        }
        cameraHolder.transform.position = transform.position + offset;
        */
        cameraHolder.transform.position = transform.position + offset;
    }
}
