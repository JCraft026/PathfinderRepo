using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Text.RegularExpressions;

public class Attack : NetworkBehaviour
{
    public Animator animator;                                // Character's animator manager
    Vector3 runnerPosition,                                  // Scene position of the runner
            guardPosition;                                   // Scene position of the attacking guard master
    public bool damageTaken = false;                         // Status of runner taking damage durring current attack
    public Regex chaserExpression = new Regex("Chaser");     // Match "Chaser"
    public Regex engineerExpression = new Regex("Engineer"); // Match "Engineer"
    public Regex trapperExpression = new Regex("Trapper");   // Match "Trapper"
    public CameraShake cameraShake;                          // Holds the camera shaker script

    // Update is called once per frame
    void Update()
    {
        // Trigger attack processing on both the runner and guard master side if the guard master hits the "E" key
        if((Input.GetKeyDown("e") && CustomNetworkManager.isRunner == false && animator.GetBool("Attack Triggered") == false && animator.GetBool("Dashing") == false && gameObject.GetComponent<ManageActiveCharacters>().guardId == gameObject.GetComponent<ManageActiveCharacters>().activeGuardId) || ((animator.GetBool("Attack Triggered") == true) && CustomNetworkManager.isRunner == true)){
            var runner = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
            runnerPosition = runner.transform.position;
            guardPosition  = transform.position;

            // Trigger guard master attack animation
            animator.SetBool("Attack", true);
            animator.SetBool("Attack Triggered", true);
            
            // If the runner is within attack range, process guard master attack
            if(Utilities.GetDistanceBetweenObjects(guardPosition, runnerPosition) <= 2.3f){
                // Assign camera shaker to the appropriate camera
                if(CustomNetworkManager.isRunner){
                    cameraShake = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("CameraHolder(R)")).transform.GetChild(0).GetComponent<CameraShake>();
                }
                else{
                    if(chaserExpression.IsMatch(gameObject.name)){
                        cameraShake = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("CameraHolder(C)")).transform.GetChild(0).GetComponent<CameraShake>();
                    }
                    else if(engineerExpression.IsMatch(gameObject.name)){
                        cameraShake = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("CameraHolder(E)")).transform.GetChild(0).GetComponent<CameraShake>();
                    }
                    else if(trapperExpression.IsMatch(gameObject.name)){
                        cameraShake = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("CameraHolder(T)")).transform.GetChild(0).GetComponent<CameraShake>();                        
                    }
                }
                

                // Process attack impact and effects based on the approaprate guard master facing direction
                switch (animator.GetFloat("Facing Direction"))
                {
                    case MoveCharacterConstants.FORWARD:
                        if((guardPosition.y-runnerPosition.y) > 0f){
                            StartCoroutine(cameraShake.Shake(.15f, .7f));
                            HandleEvents.ProcessAttackImpact((int)MoveCharacterConstants.FORWARD);
                        }
                        break;
                    case MoveCharacterConstants.LEFT:
                        if((guardPosition.x - runnerPosition.x) > 0f){
                            StartCoroutine(cameraShake.Shake(.15f, .7f));
                            HandleEvents.ProcessAttackImpact((int)MoveCharacterConstants.LEFT);
                        }
                        break;
                    case MoveCharacterConstants.BACKWARD:
                        if((runnerPosition.y-guardPosition.y) > 0f){
                            StartCoroutine(cameraShake.Shake(.15f, .7f));
                            HandleEvents.ProcessAttackImpact((int)MoveCharacterConstants.BACKWARD);
                        }
                        break;
                    case MoveCharacterConstants.RIGHT:
                        if((runnerPosition.x-guardPosition.x) > 0f){
                            StartCoroutine(cameraShake.Shake(.15f, .7f));
                            HandleEvents.ProcessAttackImpact((int)MoveCharacterConstants.RIGHT);
                        }
                        break;    
                }

                // Subtract HP from the runner
                if(damageTaken == false){
                    runner.GetComponent<ManageRunnerStats>().TakeDamage(2);
                    damageTaken = true;
                }
                
            }

            // Disable attack triggered status if the player is the runner to reset attack processing
            if(CustomNetworkManager.isRunner){
                animator.SetBool("Attack Triggered", false);
            }
        }
        
        // Reset damageTaken
        else{
            damageTaken = false;
        }
    }
}
