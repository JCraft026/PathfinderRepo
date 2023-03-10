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

    void Awake(){
        Debug.Log("I'M AWAKERN");
        runner = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
        runnerScript = runner.GetComponent<MoveCharacter>();
        slowTrapped = runner.GetComponent<SlowTrapped>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Utilities.GetDistanceBetweenObjects(transform.position, runner.transform.position) < 1.2f){
            slowTrapped.trapped();
            destroyChestTrap();
        }
    }

    [Command(requiresAuthority = false)]
    public void destroyChestTrap(){
        NetworkServer.Destroy(gameObject);
    }
}
