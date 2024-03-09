using UnityEngine;
using Mirror;

public class TrapperAbility : GuardAbilityBase
{
    public static bool abilityClicked = false;  // Status of the ability icon being clicked
    public GameObject chestTrap;                // Actual chest trap object
    public GameObject tempChestTrap;            // Temporary chest trap object to instantiate

    public override float AbilityUseageCost => 25f;

    public override bool AbilityClicked { get => abilityClicked; set => abilityClicked = value; }
    protected override void DoAbility()
    {
        GenerateSteam.steam -= AbilityUseageCost;
        PlaceChestTrap();
    }

    //void Start(){
    //    trapperMoveCharacter = gameObject.GetComponent<MoveCharacter>();
    //}

    // Update is called once per frame
    //void Update()
    //{
    //    // When trapper presses "[q]"
    //    if(((Input.GetKeyDown("q") || abilityClicked) && CustomNetworkManager.isRunner == false && gameObject.GetComponent<ManageActiveCharacters>().guardId == gameObject.GetComponent<ManageActiveCharacters>().activeGuardId) && GenerateSteam.steam >= 25f){
    //        // Subtract from steam
    //        GenerateSteam.steam -= 25f;
    //
    //        placeChestTrap();
    //    }
    //
    //    // Reset the ability clicked status
    //    abilityClicked = false;
    //}

    // Instantiates and spawns the chest trap
    [Command]
    public void PlaceChestTrap(){
        tempChestTrap = Instantiate(chestTrap, CharacterMovementController.transform.position, Quaternion.identity);
        NetworkServer.Spawn(tempChestTrap);
        return;
    }
    
}
