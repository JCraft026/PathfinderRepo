using Mirror;
using UnityEngine;

public abstract class GuardAbilityBase : NetworkBehaviour
{
    public MoveCharacter CharacterMovementController { get; set; }
    public abstract float AbilityUseageCost { get; }
    public abstract bool AbilityClicked { get; set; } // Temporary fix for all "abilityClicked" variables being static to the class
    protected bool ShouldDoAbility
    { 
        get =>
        ((Input.GetKeyDown("k") || AbilityClicked) && !CustomNetworkManager.isRunner 
        && gameObject.GetComponent<ManageActiveCharacters>().guardId == gameObject.GetComponent<ManageActiveCharacters>().activeGuardId);
    }
    public AudioSource audioSource;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        CharacterMovementController = gameObject.GetComponent<MoveCharacter>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (ShouldDoAbility && GenerateSteam.steam >= AbilityUseageCost)
            DoAbility();
        else if (GenerateSteam.steam < AbilityUseageCost)
            GameObject.Find("PopupMessageManager").GetComponent<ManagePopups>().ProcessAbilityAlert("<color=red>Not enough steam to use ability</color>", 3f);
        AbilityClicked = false;
    }

    // Steam should be subtracted the overrides for this method (The ability does not necessarily have to be used)
    protected abstract void DoAbility();


    [Command]
    public void cmd_PlaySyncedAbilityAudio()
    {
        rpc_PlaySyncedAbilityAudio();
    }

    [ClientRpc]
    public void rpc_PlaySyncedAbilityAudio()
    {
        if(audioSource.isPlaying == false)
            audioSource.Play();
        else
            Debug.Log(gameObject.name + " ability audio source is already playing");
    }
}
