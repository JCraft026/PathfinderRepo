using UnityEngine;
using Mirror;

public class TrapperAbility : GuardAbilityBase
{
    public GameObject chestTrap;                // Actual chest trap object
    public GameObject tempChestTrap;            // Temporary chest trap object to instantiate

    public override float AbilityUseageCost => 25f;

    protected override void DoAbility()
    {
        GenerateSteam.steam -= AbilityUseageCost;
        PlaceChestTrap();
        cmd_PlaySyncedAbilityAudio();
    }

    [Command]
    public void PlaceChestTrap()
    {
        tempChestTrap = Instantiate(chestTrap, CharacterMovementController.transform.position, Quaternion.identity);
        NetworkServer.Spawn(tempChestTrap);
        return;
    }
}
