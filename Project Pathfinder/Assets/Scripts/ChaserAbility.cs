using UnityEngine;

public class ChaserAbility : GuardAbilityBase
{
    public static bool abilityClicked = false; // Status of the ability icon being clicked
    ChaserDash chaserDash;                     // ChaserDash instance
    public Animator animator;                  // Chaser's animator controller

    public override float AbilityUseageCost { get => 10f; }
    public override bool AbilityClicked { get => abilityClicked; set => abilityClicked = value; }

    protected override void Start(){
        base.Start();
        chaserDash = gameObject.GetComponent<ChaserDash>();
    }

    protected override void Update()
    {
        if(ShouldDoAbility && !animator.GetBool("Attack"))
            DoAbility();
    }

    protected override void DoAbility()
    {
        if (!animator.GetBool("Attack") && GenerateSteam.steam >= AbilityUseageCost)
        {
            GenerateSteam.steam -= AbilityUseageCost;
            chaserDash.startDash();
            Debug.Log("Started dash");
        }
    }
}
