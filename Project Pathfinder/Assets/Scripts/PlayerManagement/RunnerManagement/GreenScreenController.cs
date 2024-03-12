using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenScreenController : MonoBehaviour
{
    private float timeStamp;                      // The time PAST the current time that needs to be reached 
    public bool greenScreenIsOver = true;         // Whether or not the coffee cooldown is over  
    public Animator animator;                    // The runner's animator 
    public static GreenScreenController Instance; // Makes an instance of this class to access attribtues

    // Start is called when the game starts
    void Awake(){
        Instance = this;
        timeStamp = 0.0f;
    }

    // Update is called once per frame
    void Update(){
        // Green Screen Suit Item
        if(timeStamp > Time.time){
            greenScreenIsOver = false;
        } 
        else{
            animator.SetBool("isGreen", false);
            greenScreenIsOver = true;
            this.enabled = false;
        }
    
    }

    // Add a number of seconds to the time stamp to control the cooldown length
    public void setCooldown(int seconds){
        if(greenScreenIsOver){
            timeStamp = Time.time + seconds;
            this.enabled = true;
            Debug.Log("Cooldown Set");
        }
        else{
            Debug.Log("Still cooling down");
        }
    }
}