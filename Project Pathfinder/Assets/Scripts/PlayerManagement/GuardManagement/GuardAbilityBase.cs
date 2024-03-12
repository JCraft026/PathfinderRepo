using Mirror;
using UnityEngine;

public abstract class GuardAbilityBase : NetworkBehaviour
{
    public AudioSource audioSource;
    public MoveCharacter CharacterMovementController { get; set; }
    public abstract float AbilityUseageCost { get; }
    public bool AbilityClicked { get; set; } = false; protected bool ShouldDoAbility
    { 
        get =>
        ((Input.GetKeyDown("k") || AbilityClicked) && !CustomNetworkManager.IsRunner 
        && gameObject.GetComponent<ManageActiveCharacters>().guardId == gameObject.GetComponent<ManageActiveCharacters>().activeGuardId);
    }

    // Steam should be subtracted the overrides for this method (The ability does not necessarily have to be used)
    protected abstract void DoAbility();

    // Start is called before the first frame update
    protected virtual void Start()
    {
        CharacterMovementController = gameObject.GetComponent<MoveCharacter>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        var shouldDoAbilityThisFrame = ShouldDoAbility; // So we don't have to recalculate the condition every frame
        if (shouldDoAbilityThisFrame && GenerateSteam.steam >= AbilityUseageCost)
            DoAbility();
        else if (shouldDoAbilityThisFrame && GenerateSteam.steam < AbilityUseageCost) // This little boi isn't working as I'd expect him to (popup doesn't show up when they can't afford it)
            DisplayAbilityAlert("Not enough steam to use ability");
        AbilityClicked = false;
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
