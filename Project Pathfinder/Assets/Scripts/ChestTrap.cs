using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class ChestTrap : NetworkBehaviour
{
    GameObject runner;

    MoveCharacter runnerScript;
    SlowTrapped slowTrapped;
    private bool trapped = false;
    Animator chestAnimator;

    void Awake(){
        Debug.Log("I'M AWAKERN");
        runner = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
        runnerScript = runner.GetComponent<MoveCharacter>();
        slowTrapped = runner.GetComponent<SlowTrapped>();
        chestAnimator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Utilities.GetDistanceBetweenObjects(transform.position, runner.transform.position) < 1.2f){
            if(trapped == false){
                slowTrapped.trapped();
                // I did this in the animator event destroyChestTrap();
                trapped = true;
                chestAnimator.SetBool("Exploding", true);
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void destroyChestTrap(){
        NetworkServer.Destroy(gameObject);
    }
}
