using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Attack : NetworkBehaviour
{
    public Animator animator;   // Character's animator manager
    Transform runner;           // Runner's game objece
    bool runnerSpawned = false; // Runner existance status
    Vector3 runnerPosition,
            guardPosition;

    // Update is called once per frame
    void Update()
    {
        if(runnerSpawned == false && Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner [")) != null){
            runner = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner [")).transform;
            runnerSpawned = true;
        }

        if(isLocalPlayer){
            if(Input.GetKeyDown("e")){
                runnerPosition = runner.transform.position;
                guardPosition  = transform.position;

                animator.SetBool("Attack", true);
                if(runnerSpawned == true && Utilities.GetDistanceBetweenObjects(guardPosition, runnerPosition) <= 2.0f){
                    switch (animator.GetFloat("Facing Direction"))
                    {
                        case MoveCharacterConstants.FORWARD:
                            if((runnerPosition.y-guardPosition.y) >= 1.5f){
                                HandleEvents.ProcessAttackImpact((int)MoveCharacterConstants.FORWARD);
                            }
                            break;
                        case MoveCharacterConstants.LEFT:
                            if((guardPosition.y - runnerPosition.y) >= 1.5f){
                                HandleEvents.ProcessAttackImpact((int)MoveCharacterConstants.LEFT);
                            }
                            break;
                        case MoveCharacterConstants.BACKWARD:
                            if((guardPosition.y - runnerPosition.y) >= 1.5f){
                                HandleEvents.ProcessAttackImpact((int)MoveCharacterConstants.BACKWARD);
                            }
                            break;
                        case MoveCharacterConstants.RIGHT:
                            if((runnerPosition.x-guardPosition.x) >= 1.5f){
                                HandleEvents.ProcessAttackImpact((int)MoveCharacterConstants.RIGHT);
                            }
                            break;
                    }
                }
            }
        }
    }
}
