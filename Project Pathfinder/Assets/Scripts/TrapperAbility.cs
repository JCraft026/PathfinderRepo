using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Mirror;
using System.Text.RegularExpressions;
using System.Linq;

public class TrapperAbility : NetworkBehaviour
{
    public GameObject chestTrap;                // Actual chest trap object
    public GameObject tempChestTrap;            // Temporary chest trap object to instantiate
    private MoveCharacter trapperMoveCharacter; // Trapper's MoveCharacter Script

    void Start(){
        trapperMoveCharacter = gameObject.GetComponent<MoveCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        // When trapper presses "[q]"
        if((Input.GetKeyDown("q") && CustomNetworkManager.isRunner == false && gameObject.GetComponent<ManageActiveCharacters>().guardId == gameObject.GetComponent<ManageActiveCharacters>().activeGuardId) && GenerateSteam.steam >= 25f){
            // Subtract from steam
            GenerateSteam.steam -= 25f;

            placeChestTrap();
        }
    }

    // Instantiates and spawns the chest trap
    [Command]
    public void placeChestTrap(){
        tempChestTrap = Instantiate(chestTrap, trapperMoveCharacter.transform.position, Quaternion.identity);
        NetworkServer.Spawn(tempChestTrap);
        return;
    }
}
