using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Text.RegularExpressions;

public class ChaserAbility : GuardAbilityBase
{
    public static bool abilityClicked = false; // Status of the ability icon being clicked
    MoveCharacter chaserMoveCharacter;         // Chaser's MoveCharacter script
    ChaserDash chaserDash;                     // ChaserDash instance
    public Animator animator;                  // Chaser's animator controller

    void Start(){
        chaserMoveCharacter = gameObject.GetComponent<MoveCharacter>();
        chaserDash = gameObject.GetComponent<ChaserDash>();
    }

    // Update is called once per frame
    void Update()
    {
        // When the chaser presses "[q]"
        if(((Input.GetKeyDown("q") || abilityClicked)
                && CustomNetworkManager.isRunner == false
                && gameObject.GetComponent<ManageActiveCharacters>().guardId == gameObject.GetComponent<ManageActiveCharacters>().activeGuardId)){
            // If the chaser wasn't already attacking
            if(animator.GetBool("Attack") == false && GenerateSteam.steam >= 10f){
                // Subtract from steam
                GenerateSteam.steam -= 10f;

                chaserDash.startDash();
                cmd_PlaySyncedAbilityAudio();
                Debug.Log("Started dash");
            }
        }

        // Reset the ability clicked status
        abilityClicked = false;
    }
}
