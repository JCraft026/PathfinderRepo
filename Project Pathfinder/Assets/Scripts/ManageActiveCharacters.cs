using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

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
