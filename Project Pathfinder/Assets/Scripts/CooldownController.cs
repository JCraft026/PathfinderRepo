using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownController : MonoBehaviour
{
    private float coffeeTimeStamp,             // The time PAST the current time that needs to be reached 
                  greenScreenTimeStamp;        // The time PAST the current time that needs to be reached
    public bool coffeeIsOver      = true,      // Whether or not the coffee cooldown is over  
                greenScreenIsOver = true;      // Whether or not the Green Screen cooldown is over
    public static CooldownController Instance; // Makes an instance of this class to access attribtues
    public Item.ItemType actionType;           // Holds the enum type of the item or action

    // Start is called when the game starts
    void Awake(){
        Instance = this;
        greenScreenTimeStamp = 0.0f;
        coffeeTimeStamp      = 0.0f;
    }

    // Update is called once per frame
    void Update(){
        switch(actionType){
            default:
                Debug.Log("Invalid Item Type");
                this.enabled = false;
                break;
            // Coffee Item    
            case Item.ItemType.Coffee:
                if(coffeeTimeStamp > Time.time){
                    coffeeIsOver = false;
                } 
                else{
                    coffeeIsOver = true;
                    MoveCharacter runnerMovementScript  = gameObject.GetComponent<MoveCharacter>();
                    runnerMovementScript.moveSpeed = 5.0f;
                    if(greenScreenIsOver){
                        actionType = Item.ItemType.Keys_0;
                    }
                }
                break;
            // Green Screen Suit Item
            case Item.ItemType.GreenScreenSuit:
                if(greenScreenTimeStamp > Time.time){
                    greenScreenIsOver = false;
                } 
                else{
                    greenScreenIsOver = true;
                    MoveCharacter runnerMovementScript  = gameObject.GetComponent<MoveCharacter>();
                    runnerMovementScript.notGreenScreen();
                    if(coffeeIsOver){
                        actionType = Item.ItemType.Keys_0;
                    }
                }
                break;
        }
    
    }

    // Add a number of seconds to the time stamp to control the cooldown length
    public void setCooldown(int seconds, Item.ItemType incommingItemType){
        if(coffeeIsOver && incommingItemType == Item.ItemType.Coffee){
            coffeeTimeStamp = Time.time + seconds;
            this.enabled = true;
            Debug.Log("Cooldown Set");
            actionType = incommingItemType;
        }
        else if(greenScreenIsOver && incommingItemType == Item.ItemType.GreenScreenSuit){
            greenScreenTimeStamp = Time.time + seconds;
            Debug.Log("Cooldown Set");
            this.enabled = true;
            actionType = incommingItemType;
        }
        else{
            Debug.Log("Still cooling down");
        }
    }
}