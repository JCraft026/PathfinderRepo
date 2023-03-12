using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class ChaserDash : MonoBehaviour
{
    MoveCharacter chaserMoveCharacter; 
    Rigidbody2D chaserRigidBody;
    private Vector2 dashDirection;
    private float timer;
    public Animator animator;     // Character's animator manager

    void Start(){
        chaserMoveCharacter = gameObject.GetComponent<MoveCharacter>();
        chaserRigidBody = gameObject.GetComponent<Rigidbody2D>();
        this.enabled = false;
    }

    // Update is called once per frame
    void Update(){
        // If the time runs out
        if(timer >= 1.0f){
            animator.SetBool("Dashing", false);
            this.enabled = false;
        }
        // While the dash is happening
        else{
            // Make all (including diagonal) directions the same speed
            dashDirection.Normalize();
            // Move the chaser in that direction 
            chaserRigidBody.MovePosition(chaserRigidBody.position + 
            dashDirection * 20.0f * Time.fixedDeltaTime);
            timer += 0.01666667f;
        }   
    }

    // Start the dash ability
    public void startDash(){
        // If the chaser isn't already attacking
        if(animator.GetBool("Attack") == false){
            // If the chaser is moving when "[q]" is pressed
            if(chaserMoveCharacter.movementInput != Vector2.zero){
                dashDirection = chaserMoveCharacter.movementInput;
            }
            // If the chaser was stationary when "[q]" was pressed
            else
            {
                switch(chaserMoveCharacter.facingDirection){
                    case 1f:
                        dashDirection = new Vector2(0,-1);
                        break; 
                    case 2f:
                        dashDirection = new Vector2(-1,0);
                        break; 
                    case 3f:
                        dashDirection = new Vector2(0,1);
                        break; 
                    case 4f:
                        dashDirection = new Vector2(1,0);
                        break; 
                    default:
                        dashDirection = new Vector2(0,-1);
                        Debug.Log("Direction assigned to facing down by default");
                        break;
                }
            }
        } 
        animator.SetBool("Dashing", true);
        timer = 0.0f;
        this.enabled = true;
    }
}
