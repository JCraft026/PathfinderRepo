using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ManageRunnerProximityAlert : MonoBehaviour
{
    public Animator animator;              // Character's animator manager
    private bool alertActive = false;      // Alert active status
    Vector3 runnerPosition,                // Scene position of the runner
            guardPosition;                 // Scene position of the attacking guard master
    int[] runnerCell         = new int[2]; // Cell the runner is standing in
    int[] guardCell          = new int[2]; // Cell the guard parent object is standing in
    bool lineOfSightClear    = false;      // Reflects whether the line of sight between the guard and the runner is clear

    void Start(){
        // Assign animator to parent guard object animator
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Process runner proximity alert if the player is the guard master and the parent guard object is inactive
        if(!CustomNetworkManager.isRunner && gameObject.GetComponent<ManageActiveCharacters>().activeGuardId != gameObject.GetComponent<ManageActiveCharacters>().guardId){
            
            // Calculate runner and guard location data
            var runner     = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
            runnerPosition = runner.transform.position;
            guardPosition  = transform.position;
            runnerCell     = Utilities.GetCharacterCellLocation(ManageActiveCharactersConstants.RUNNER);
            guardCell      = Utilities.GetCharacterCellLocation(gameObject.GetComponent<ManageActiveCharacters>().guardId);

            // Check if the guards line of sight is clear to the runner
            RaycastHit2D hit = Physics2D.Linecast(runnerPosition, guardPosition);
            if(hit.collider == null || hit.transform.gameObject.name == gameObject.name || hit.transform.gameObject.name == runner.name){
                lineOfSightClear = true;
            }
            else{
                lineOfSightClear = false;
            }

            // Check if the runner is close to the parent guard object
            if(Utilities.GetDistanceBetweenObjects(guardPosition, runnerPosition) <= 3.5f && lineOfSightClear){

                // Check if the parent guard object if facing the appropriate direction to detect the runner and display the detection alert
                switch (animator.GetFloat("Facing Direction"))
                {
                    case MoveCharacterConstants.FORWARD:
                        if((guardPosition.y-runnerPosition.y) > 0f){
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("alertBackground")).SetActive(true);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("alertText")).SetActive(true);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("RunIcon")).SetActive(true);
                            alertActive = true;
                        }
                        break;
                    case MoveCharacterConstants.LEFT:
                        if((guardPosition.x - runnerPosition.x) > 0f){
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("alertBackground")).SetActive(true);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("alertText")).SetActive(true);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("RunIcon")).SetActive(true);
                            alertActive = true;
                        }
                        break;
                    case MoveCharacterConstants.BACKWARD:
                        if((runnerPosition.y-guardPosition.y) > 0f){
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("alertBackground")).SetActive(true);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("alertText")).SetActive(true);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("RunIcon")).SetActive(true);
                            alertActive = true;
                        }
                        break;
                    case MoveCharacterConstants.RIGHT:
                        if((runnerPosition.x-guardPosition.x) > 0f){
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("alertBackground")).SetActive(true);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("alertText")).SetActive(true);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("RunIcon")).SetActive(true);
                            alertActive = true;
                        }
                        break;    
                }
            }

            // Reset the detection alert if the runner is not in proximity to the inactive guard object
            else{
                if(alertActive == true){
                    Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("alertBackground")).SetActive(false);
                    Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("alertText")).SetActive(false);
                    Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("RunIcon")).SetActive(false);
                    alertActive = false;
                }
            }
        }

        // Reset the detection alert when the guard that detected the runner becomes active
        else{
            if(alertActive == true){
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("alertBackground")).SetActive(false);
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("alertText")).SetActive(false);
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("RunIcon")).SetActive(false);
                alertActive = false;
            }
        }
    }
}
