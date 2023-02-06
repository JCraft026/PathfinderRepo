using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Attack : NetworkBehaviour
{
    public Animator animator;   // Character's animator manager
    Transform runner;           // Runner's game objece
    Vector3 runnerPosition,
            guardPosition;

    // Update is called once per frame
    void Update()
    {
        if((Input.GetKeyDown("e") && CustomNetworkManager.isRunner == false) || (animator.GetBool("Attack") == true) && CustomNetworkManager.isRunner == true){
            runner = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner")).transform;
            runnerPosition = runner.transform.position;
            guardPosition  = transform.position;

            animator.SetBool("Attack", true);
            if(Utilities.GetDistanceBetweenObjects(guardPosition, runnerPosition) <= 2.3f){
                var cameraShake = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("CameraHolder(R)")).transform.GetChild(0).GetComponent<CameraShake>();
                switch (animator.GetFloat("Facing Direction"))
                {
                    case MoveCharacterConstants.FORWARD:
                        if((guardPosition.y-runnerPosition.y) >= 1.5f){
                            if(CustomNetworkManager.isRunner == true){
                                StartCoroutine(cameraShake.Shake(.15f, .4f));
                            }
                            HandleEvents.ProcessAttackImpact((int)MoveCharacterConstants.FORWARD);
                        }
                        break;
                    case MoveCharacterConstants.LEFT:
                        if((guardPosition.x - runnerPosition.x) >= 1.5f){
                            if(CustomNetworkManager.isRunner == true){
                                StartCoroutine(cameraShake.Shake(.15f, .4f));
                            }
                            HandleEvents.ProcessAttackImpact((int)MoveCharacterConstants.LEFT);
                        }
                        break;
                    case MoveCharacterConstants.BACKWARD:
                        if((runnerPosition.y-guardPosition.y) >= 1.5f){
                            if(CustomNetworkManager.isRunner == true){
                                StartCoroutine(cameraShake.Shake(.15f, .4f));
                            }
                            HandleEvents.ProcessAttackImpact((int)MoveCharacterConstants.BACKWARD);
                        }
                        break;
                    case MoveCharacterConstants.RIGHT:
                        if((runnerPosition.x-guardPosition.x) >= 1.5f){
                            if(CustomNetworkManager.isRunner == true){
                                StartCoroutine(cameraShake.Shake(.15f, .4f));
                            }
                            HandleEvents.ProcessAttackImpact((int)MoveCharacterConstants.RIGHT);
                        }
                        break;
                }
            }
        }
    }
}
