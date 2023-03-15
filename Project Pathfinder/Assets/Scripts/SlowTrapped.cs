using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTrapped : MonoBehaviour
{
    float timeStamp = 0.0f;     // used to mark how long the player is slowed
    MoveCharacter runnerScript; // runner's MoveCharacter script

    // Start is called before the first frame update
    void Start()
    {
        this.enabled = false;
        timeStamp = 0.0f;
        runnerScript = gameObject.GetComponent<MoveCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        // Run when the time == the previously set timeStamp
        if(Time.time >= timeStamp){
            runnerScript.moveSpeed = 5.0f;
            this.enabled = false;
        }
    }

    // Slows and damages the runner, and starts the timer
    public void trapped(){
        this.enabled = true;
        timeStamp = Time.time + 4f;
        runnerScript.moveSpeed = 2.5f;
        if(gameObject.GetComponent<ManageRunnerStats>().health <= 2){
            HandleEvents.endGameEvent = HandleEventsConstants.RUNNER_TRAPPED;
        }
        gameObject.GetComponent<ManageRunnerStats>().TakeDamage(2);
    } 
}
