using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTrapped : MonoBehaviour
{

    float timeStamp = 0.0f;
    MoveCharacter runnerScript;

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
        if(Time.time >= timeStamp){
            runnerScript.moveSpeed = 5.0f;
            this.enabled = false;
        }
        else{
            Debug.Log("running!");
        }
    }

    public void trapped(){
        this.enabled = true;
        timeStamp = Time.time + 4f;
        runnerScript.moveSpeed = 2.5f;
        gameObject.GetComponent<ManageRunnerStats>().TakeDamage(2);
    } 
}
