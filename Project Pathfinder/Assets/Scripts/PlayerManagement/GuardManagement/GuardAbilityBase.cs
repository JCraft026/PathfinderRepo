using Mirror;
using UnityEngine;

public abstract class GuardAbilityBase : NetworkBehaviour
{
    public AudioSource audioSource;
    public MoveCharacter CharacterMovementController { get; set; }
    public abstract float AbilityUseageCost { get; }
    public bool AbilityClicked { get; set; } = false; 
    protected ManageActiveCharacters ManageActiveCharacters { get; set; }
    protected int? GuardId => ManageActiveCharacters?.guardId;
    protected int? ActiveGuardId => ManageActiveCharacters?.activeGuardId;
    protected bool ShouldDoAbility
    { 
        get =>
        (AbilityClicked && ShouldDoAbilityKeyIndependent);
    }

    protected bool ShouldDoAbilityKeyIndependent => !CustomNetworkManager.IsRunner && GuardId == ActiveGuardId && GuardId != null;

    // Steam should be subtracted the overrides for this method (The ability does not necessarily have to be used)
    protected abstract void DoAbility();

    // Start is called before the first frame update
    protected virtual void Start()
    {
        ManageActiveCharacters = gameObject.GetComponent<ManageActiveCharacters>();
        CharacterMovementController = gameObject.GetComponent<MoveCharacter>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (ShouldDoAbility)
        {
            if (GenerateSteam.steam >= AbilityUseageCost)
                DoAbility();
            else // This little boi isn't working as I'd expect him to (popup doesn't show up when they can't afford it)
                DisplayAbilityAlert("Not enough steam to use ability");
            AbilityClicked = false;
        }
    }

    void OnUseAbility()
    {
        if (ShouldDoAbilityKeyIndependent && GenerateSteam.steam >= AbilityUseageCost)
            DoAbility();
        else if (GenerateSteam.steam < AbilityUseageCost)
            DisplayAbilityAlert("Not enough steam to use ability");
    }

    protected void DisplayAbilityAlert(string message, float duration=3f) => 
        GameObject.Find("PopupMessageManager").GetComponent<ManagePopups>().ProcessAbilityAlert($@"<color=red>{message}</color>", duration);


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
