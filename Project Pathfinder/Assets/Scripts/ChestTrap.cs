using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class ChestTrap : NetworkBehaviour
{
    GameObject runner;

    MoveCharacter runnerScript;
    TrapperAbility trapperAbility;

    void Awake(){
        Debug.Log("I'M AWAKERN");
        runner = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
        runnerScript = runner.GetComponent<MoveCharacter>();
        trapperAbility = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper")).GetComponent<TrapperAbility>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Utilities.GetDistanceBetweenObjects(transform.position, runner.transform.position) < 1.2f){
            triggerTrap();
        }
    }

    void triggerTrap(){
        trapperAbility.trapCount -= 1;
        runnerScript.moveSpeed = 2.5f;
    }
}
