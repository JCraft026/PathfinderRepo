using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeController : MonoBehaviour
{
    private float timeStamp;                 // The time PAST the current time that needs to be reached 
    public bool coffeeIsOver = true;               // Whether or not the coffee cooldown is over  
    public static CoffeeController Instance; // Makes an instance of this class to access attribtues

    // Start is called when the game starts
    void Awake(){
        Instance = this;
        timeStamp = 0.0f;
    }

    // Update is called once per frame
    void Update(){
        // Coffee Item    
        if(timeStamp > Time.time){
            coffeeIsOver = false;
        } 
        else{
            coffeeIsOver = true;
            MoveCharacter runnerMovementScript  = gameObject.GetComponent<MoveCharacter>();
            runnerMovementScript.moveSpeed = 5.0f;
            this.enabled = false;
        }
    }   

    // Add a number of seconds to the time stamp to control the cooldown length
    public void setCooldown(int seconds){
        if(coffeeIsOver){
            timeStamp = Time.time + seconds;
            this.enabled = true;
            Debug.Log("Cooldown Set");
        }
        else{
            Debug.Log("Still cooling down");
        }
    }
}