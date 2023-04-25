using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Mirror;
using System.Text.RegularExpressions;
using System.Linq;

public class TrapperAbility : GuardAbilityBase
{
    public static bool abilityClicked = false;  // Status of the ability icon being clicked
    public GameObject chestTrap;                // Actual chest trap object
    public GameObject tempChestTrap;            // Temporary chest trap object to instantiate
    private MoveCharacter trapperMoveCharacter; // Trapper's MoveCharacter Script

    void Start(){
        trapperMoveCharacter = gameObject.GetComponent<MoveCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        // When trapper presses "[k]"
        if(((Input.GetKeyDown("k") || abilityClicked) && CustomNetworkManager.isRunner == false 
                && gameObject.GetComponent<ManageActiveCharacters>().guardId == gameObject.GetComponent<ManageActiveCharacters>().activeGuardId)){
            if(GenerateSteam.steam >= 25f){
                // Subtract from steam
                GenerateSteam.steam -= 25f;

                placeChestTrap();
                this.cmd_PlaySyncedAbilityAudio();
            }
            else{
                GameObject.Find("PopupMessageManager").GetComponent<ManagePopups>().ProcessAbilityAlert("<color=red>Not enough steam to use ability</color>", 3f);
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
