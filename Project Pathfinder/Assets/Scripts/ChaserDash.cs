using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using Mirror;

public class ChaserDash : NetworkBehaviour
{
    MoveCharacter chaserMoveCharacter; // MoveCharacter component of the chaser
    private Vector3 dashDirection;     // Direction the chaser is dashing
    public Animator animator;          // Character's animator manager
    public int frameCount;             // Amount of frames passed in update statement
    public bool trajectoryClear;       // Reflects whether the chasers current trajectory is clear of walls
    public bool attackLanded = false;  // Status of dash attack being landed

    // Assign chaser's MoveCharacter component
    void Start(){
        chaserMoveCharacter = gameObject.GetComponent<MoveCharacter>();
    }

    // Update is called once per frame
    void Update(){
        frameCount += 1; // Amount of frames passed

        // Reset attack landed status
        if(attackLanded && !Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser")).GetComponent<Animator>().GetBool("Dashing")){
            attackLanded = false;
        }

        // Display chaser dash
        if(frameCount <= 6){
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
                if(trajectoryClear && !CustomNetworkManager.isRunner && gameObject.GetComponent<MoveCharacter>().isDisabled == false)
                {
                    dashDirection.Normalize();
                    gameObject.transform.position += dashDirection * Time.deltaTime;
                }
            }
        }
        else if(animator.GetBool("Dashing")){
            animator.SetBool("Dashing", false);
            gameObject.GetComponent<MoveCharacter>().canMove = true;
        }
    }

    // Start the dash ability
    public void startDash(){
        // Start dash if the chaser is not attacking
        if(animator.GetBool("Attack") == false){
            // If the chaser is already moving, calculate dash direction based on the current movement input
            if(chaserMoveCharacter.movementInput != Vector2.zero){
                dashDirection = chaserMoveCharacter.movementInput;
            }

            // If the chaser is not moving, calculate the dash direction based on the facing direction
            else
            {
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
                        break;
                }
            }
        } 

        // Initialize dash variables
        animator.SetBool("Dashing", true);
        gameObject.GetComponent<MoveCharacter>().canMove = false;
        frameCount = 0;
    }

    // Check if the trajectory of the chaser dash is clear of obsticals
    public bool ImpactTrajectoryClear(Vector2 characterPosition, float moveDirection){
        bool trajectoryClear = true;
        Regex lrWallExpression = new Regex("LR");     // Match left and right walls
        Regex tbWallExpression = new Regex("TB");     // Match top and bottom walls
        Regex runnerExpression = new Regex("Runner"); // Match "Runner"
        int activeGuardId      = gameObject.GetComponent<ManageActiveCharacters>().activeGuardId;
                                                      // Guard ID of the current active guard
        Collider2D[] nearByObjects = Physics2D.OverlapCircleAll(characterPosition, 1f);
                                                      // Collider objects near the runner
        GameObject runner          = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
                                                      // Game object of the runner
        Vector3 runnerPosition     = runner.transform.position;
                                                      // Current position of the runner

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
            // Check if the runner was hit by the chaser
            if(runnerExpression.IsMatch(nearByObject.gameObject.name) && Utilities.GetDistanceBetweenObjects(gameObject.transform.position, runnerPosition) <= 1.0f && gameObject.GetComponent<Animator>().GetBool("Dashing") && attackLanded == false){
                GameObject.Find("ItemAssets").GetComponent<CommandManager>().cmd_TakeDashDamage(activeGuardId);
                attackLanded = true;
            }
        }

        return trajectoryClear;
    }
}
