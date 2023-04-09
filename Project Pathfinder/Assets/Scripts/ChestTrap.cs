using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class ChestTrap : NetworkBehaviour
{
    GameObject runner;            // Runner's gameobject
    MoveCharacter runnerScript;   // Runner's MoveCharacter script
    SlowTrapped slowTrapped;      // Instance of SlowTrapped script 
    private bool trapped = false; // Whether the player has been trapped or not
    Animator chestAnimator;       // The Chest's animator controller
    public AudioSource explosionNoise;

    // Called when the object is instantiated
    void Awake(){
        runner = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
        runnerScript = runner.GetComponent<MoveCharacter>();
        slowTrapped = runner.GetComponent<SlowTrapped>();
        chestAnimator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the runner comes into contact with the chest
        if(Utilities.GetDistanceBetweenObjects(transform.position, runner.transform.position) < 1.2f && !runner.GetComponent<Animator>().GetBool("isGreen")){
            // If the trap hasn't been set off yet (used to ensure it only happens once per trap)
            if(trapped == false){
                slowTrapped.trapped();
                trapped = true;
                chestAnimator.SetBool("Exploding", true);
                
            }
        }
    }

    // Destroy all networked instances of this object
    [Command(requiresAuthority = false)]
    public void destroyChestTrap(){
        NetworkServer.Destroy(gameObject);
    }
}
