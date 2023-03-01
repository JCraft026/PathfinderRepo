using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Text.RegularExpressions;

public class ChaserAbility : NetworkBehaviour
{

    MoveCharacter chaserMoveCharacter;
    ChaserDash chaserDash; 
    public Animator animator;     // Character's animator manager

    void Start(){
        chaserMoveCharacter = gameObject.GetComponent<MoveCharacter>();
        chaserDash = gameObject.GetComponent<ChaserDash>();
    }

    // Update is called once per frame
    void Update()
    {
        if((Input.GetKeyDown("q") && CustomNetworkManager.isRunner == false && gameObject.GetComponent<ManageActiveCharacters>().guardId == gameObject.GetComponent<ManageActiveCharacters>().activeGuardId)){
            if(animator.GetBool("Attack") == false){
                chaserDash.startDash();
            }
        }
    }
}
