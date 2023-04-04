using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Mirror;
using System.Text.RegularExpressions;
using System.Linq;

public class TrapperAbility : NetworkBehaviour
{
    public static bool abilityClicked = false;  // Status of the ability icon being clicked
    public GameObject chestTrap;                // Actual chest trap object
    public GameObject tempChestTrap;            // Temporary chest trap object to instantiate
    private MoveCharacter trapperMoveCharacter; // Trapper's MoveCharacter Script
    public GameObject abilitySound;             // Sound maker object for the trappers ability
    private AudioSource audioSource;            // Sound maker audiosource

    void Start(){
        trapperMoveCharacter = gameObject.GetComponent<MoveCharacter>();
        audioSource = abilitySound.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // When trapper presses "[q]"
        if(((Input.GetKeyDown("q") || abilityClicked) && CustomNetworkManager.isRunner == false
                && gameObject.GetComponent<ManageActiveCharacters>().guardId == gameObject.GetComponent<ManageActiveCharacters>().activeGuardId)
                && GenerateSteam.steam >= 25f){
            // Subtract from steam
            GenerateSteam.steam -= 25f;

            placeChestTrap();
            if(!audioSource.isPlaying && audioSource.clip != null)
            {
                audioSource.PlayOneShot(audioSource.clip);
                Debug.Log("TrapperAbility: audiosource now playing");
            }
            else if (audioSource.clip == null)
            {
                Debug.Log("TrapperAbility: audioSource.clip == null");
            }
            else if (audioSource.isVirtual)
            {
                Debug.LogError("TrapperAbility: audioSource.isVirtual == true (sound was culled)");
            }
            else
            {
                Debug.Log("TrapperAbility: audioSource is playing");
            }
        }

        // Reset the ability clicked status
        abilityClicked = false;
    }

    // Instantiates and spawns the chest trap
    [Command]
    public void placeChestTrap(){
        tempChestTrap = Instantiate(chestTrap, trapperMoveCharacter.transform.position, Quaternion.identity);
        NetworkServer.Spawn(tempChestTrap);
        return;
    }
}
