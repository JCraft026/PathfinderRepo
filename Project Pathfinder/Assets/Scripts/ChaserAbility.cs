using UnityEngine;

public class ChaserAbility : GuardAbilityBase
{
    ChaserDash chaserDash;                     // ChaserDash instance
    public Animator animator;                  // Chaser's animator controller

    public override float AbilityUseageCost { get => 20f; }

    protected override void Start(){
        base.Start();
        chaserDash = gameObject.GetComponent<ChaserDash>();
    }

    protected override void DoAbility()
    {
        if (!animator.GetBool("Attack") && GenerateSteam.steam >= AbilityUseageCost)
        {
            GenerateSteam.steam -= AbilityUseageCost;
            chaserDash.startDash();
            cmd_PlaySyncedAbilityAudio();
            Debug.Log("Started dash");
        }
    }
}
