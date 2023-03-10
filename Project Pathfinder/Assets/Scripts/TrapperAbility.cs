using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Mirror;
using System.Text.RegularExpressions;
using System.Linq;

public class TrapperAbility : NetworkBehaviour
{
    public GameObject chestTrap;
    public GameObject tempChestTrap;
    private int[] trapperLocation;
    private MoveCharacter trapperMoveCharacter; 
    private Vector3 trapLocation;

    void Start(){
        trapperMoveCharacter = gameObject.GetComponent<MoveCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        // When trapper presses "[q]"
        if((Input.GetKeyDown("q") && CustomNetworkManager.isRunner == false && gameObject.GetComponent<ManageActiveCharacters>().guardId == gameObject.GetComponent<ManageActiveCharacters>().activeGuardId)){
            placeChestTrap();
        }
        else if((Input.GetKeyDown("q") && CustomNetworkManager.isRunner == false && gameObject.GetComponent<ManageActiveCharacters>().guardId == gameObject.GetComponent<ManageActiveCharacters>().activeGuardId)){
            Debug.Log("Maximum amount of traps placed (2)");    
        }
    }

    [Command]
    public void placeChestTrap(){
        Debug.Log("Tried to place a ChestTrap");
        tempChestTrap = Instantiate(chestTrap, trapperMoveCharacter.transform.position, Quaternion.identity);
        NetworkServer.Spawn(tempChestTrap);
        return;
    }
}
