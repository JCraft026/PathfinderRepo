using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

public class ChaserDash : MonoBehaviour
{
    MoveCharacter chaserMoveCharacter; 
    public Rigidbody2D chaserRigidBody;
    private Vector3 dashDirection;
    private float timer;
    public Animator animator;     // Character's animator manager
    public int frameCount;        // Amount of frames passed in update statement
    public bool trajectoryClear;  // Reflects whether the chasers current trajectory is clear of walls

    void Start(){
        chaserMoveCharacter = gameObject.GetComponent<MoveCharacter>();
        this.enabled = false;
    }

    // Update is called once per frame
    void Update(){
        frameCount += 1; // Amount of frames passed

        // Display chaser dash
        if(frameCount <= 2 && !CustomNetworkManager.isRunner){
            for(int moveNudges = 30; moveNudges > 0; moveNudges--){
                switch (animator.GetFloat("Facing Direction"))
                {
                    case MoveCharacterConstants.FORWARD:
                        trajectoryClear = ImpactTrajectoryClear(gameObject.transform.position, MoveCharacterConstants.FORWARD);
                        break;    
                    case MoveCharacterConstants.LEFT:
                        trajectoryClear = ImpactTrajectoryClear(gameObject.transform.position, MoveCharacterConstants.LEFT);
                        break;   
                    case MoveCharacterConstants.BACKWARD:
                        trajectoryClear = ImpactTrajectoryClear(gameObject.transform.position, MoveCharacterConstants.BACKWARD);
                        break;   
                    case MoveCharacterConstants.RIGHT:
                        trajectoryClear = ImpactTrajectoryClear(gameObject.transform.position, MoveCharacterConstants.RIGHT);
                        break; 
                }
                if(trajectoryClear)
                {
                    dashDirection.Normalize();
                    gameObject.transform.position += dashDirection * Time.deltaTime;
                }
            }
        }
        else{
            animator.SetBool("Dashing", false);
            gameObject.GetComponent<MoveCharacter>().canMove = true;
            this.enabled = false;
        }
    }

    // Start the dash ability
    public void startDash(){
        // If the chaser isn't already attacking
        if(animator.GetBool("Attack") == false){
            // If the chaser is moving when "[q]" is pressed
            if(chaserMoveCharacter.movementInput != Vector2.zero){
                Debug.Log("chaser was moving when activated");
                dashDirection = chaserMoveCharacter.movementInput;
            }
            // If the chaser was stationary when "[q]" was pressed
            else
            {
                Debug.Log("chaser was still when activated");
                switch(chaserMoveCharacter.facingDirection){
                    case 1f:
                        dashDirection = new Vector3(0,-1,0);
                        animator.SetFloat("Vertical Movement", -1f);
                        break; 
                    case 2f:
                        dashDirection = new Vector3(-1,0,0);
                        animator.SetFloat("Horizontal Movement", -1f);
                        break; 
                    case 3f:
                        dashDirection = new Vector3(0,1,0);
                        animator.SetFloat("Vertical Movement", 1f);
                        break; 
                    case 4f:
                        dashDirection = new Vector3(1,0,0);
                        animator.SetFloat("Horizontal Movement", 1f);
                        break; 
                    default:
                        dashDirection = new Vector3(0,-1,0);
                        Debug.Log("Direction assigned to facing down by default");
                        break;
                }
            }
        } 
        animator.SetBool("Dashing", true);
        timer = 0.0f;
        this.enabled = true;
        gameObject.GetComponent<MoveCharacter>().canMove = false;
        frameCount = 0;
    }

    public bool ImpactTrajectoryClear(Vector2 characterPosition, float moveDirection){
        bool trajectoryClear = true;
        Regex lrWallExpression = new Regex("LR"); // Match left and right walls
        Regex tbWallExpression = new Regex("TB"); // Match top and bottom walls
        Collider2D[] nearByObjects = Physics2D.OverlapCircleAll(characterPosition, 1f);
                                                  // Collider objects near the runner

        foreach(var nearByObject in nearByObjects){
            // Check if there are any walls close to the left
            if(dashDirection.x < 0){
                if(lrWallExpression.IsMatch(nearByObject.gameObject.name) && (characterPosition.x-nearByObject.transform.position.x) <= 2f && Math.Abs(nearByObject.transform.position.y-characterPosition.y) < (Utilities.GetCellSize()/2)){
                    trajectoryClear = false;
                }
            }
            // Check if there are any walls close to the right
            if(dashDirection.x > 0){
                if(lrWallExpression.IsMatch(nearByObject.gameObject.name) && (nearByObject.transform.position.x-characterPosition.x) <= 2f && Math.Abs(nearByObject.transform.position.y-characterPosition.y) < (Utilities.GetCellSize()/2)){
                    trajectoryClear = false;
                }
            }
            // Check if there are any walls close to the bottom
            if(dashDirection.y < 0){
                if(tbWallExpression.IsMatch(nearByObject.gameObject.name) && (characterPosition.y-nearByObject.transform.position.y) <= 2f && Math.Abs(nearByObject.transform.position.x-characterPosition.x) < (Utilities.GetCellSize()/2)){
                    trajectoryClear = false;
                }
            }
            // Check if there are any walls close to the top
            if(dashDirection.y > 0){
                if(tbWallExpression.IsMatch(nearByObject.gameObject.name) && (nearByObject.transform.position.y-characterPosition.y) <= 2f && Math.Abs(nearByObject.transform.position.x-characterPosition.x) < (Utilities.GetCellSize()/2)){
                    trajectoryClear = false;
                }
            }
        }

        return trajectoryClear;
    }
}
