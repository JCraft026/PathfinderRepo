using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Text.RegularExpressions;

public class ChaserAbility : NetworkBehaviour
{
    public static bool abilityClicked = false; // Status of the ability icon being clicked
    MoveCharacter chaserMoveCharacter;         // Chaser's MoveCharacter script
    ChaserDash chaserDash;                     // ChaserDash instance
    public Animator animator;                  // Chaser's animator controller
    public GameObject abilitySound;            // Chaser's ability sound game object
    private AudioSource audioSource;           // Chaser's ability audiosource

    void Start(){
        chaserMoveCharacter = gameObject.GetComponent<MoveCharacter>();
        chaserDash = gameObject.GetComponent<ChaserDash>();
        audioSource = abilitySound.GetComponent<AudioSource>();
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
                audioSource.Play();
                Debug.Log("Started dash");
            }
        }

        // Reset the ability clicked status
        abilityClicked = false;
    }
}
