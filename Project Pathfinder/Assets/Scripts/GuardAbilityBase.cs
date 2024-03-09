using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class GuardAbilityBase : NetworkBehaviour
{
    public MoveCharacter CharacterMovementController { get; set; }
    public abstract float AbilityUseageCost { get; }
    public abstract bool AbilityClicked { get; set; } // Temporary fix for all "abilityClicked" variables being static to the class
    protected bool ShouldDoAbility
    { 
        get =>
        ((Input.GetKeyDown("q") || AbilityClicked) && !CustomNetworkManager.isRunner 
        && gameObject.GetComponent<ManageActiveCharacters>().guardId == gameObject.GetComponent<ManageActiveCharacters>().activeGuardId);
    }



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
        AbilityClicked = false;
    }


    protected abstract void DoAbility();

   protected void SubtractSteam()
    {
        GenerateSteam.steam -= AbilityUseageCost;
    }

}
