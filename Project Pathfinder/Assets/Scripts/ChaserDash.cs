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
        if(timer >= 1.0f){
            animator.SetBool("Dashing", false);
            this.enabled = false;
        }
        else{
            dashDirection.Normalize();
            // if(ImpactTrajectoryClear(chaserRigidBody.position, MoveC)){
                
            // }
            chaserRigidBody.MovePosition(chaserRigidBody.position + 
            dashDirection * 20.0f * Time.fixedDeltaTime);
            timer += 0.01666667f;
        }   
    }

    public void startDash(){
        if(animator.GetBool("Attack") == false){
            if(chaserMoveCharacter.movementInput != Vector2.zero){
                dashDirection = chaserMoveCharacter.movementInput;
            }
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

    // public bool ImpactTrajectoryClear(Vector2 characterPosition, float moveDirection){
    //     bool trajectoryClear = true;
    //     Regex lrWallExpression = new Regex("Wall_LR"); // Match left and right walls
    //     Regex tbWallExpression = new Regex("Wall_TB"); // Match top and bottom walls
    //     Collider2D[] nearByObjects = Physics2D.OverlapCircleAll(characterPosition, 1f);
    //                                                    // Collider objects near the runner

    //     // Move the runner in the appropriate direction based on the attack direction to simulate guard attack impact
    //     switch (moveDirection)
    //     {
    //         case MoveCharacterConstants.FORWARD:
    //             foreach(var nearByObject in nearByObjects){
    //                 if(tbWallExpression.IsMatch(nearByObject.gameObject.name) && (nearByObject.transform.position.y-characterPosition.y) <= 2f && Math.Abs(nearByObject.transform.position.x-characterPosition.x) < (Utilities.GetCellSize()/2)){
    //                     trajectoryClear = false;
    //                 }
    //             }
    //             break;
    //         case MoveCharacterConstants.LEFT:
    //             foreach(var nearByObject in nearByObjects){
    //                 if(lrWallExpression.IsMatch(nearByObject.gameObject.name) && (characterPosition.x-nearByObject.transform.position.x) <= 2f && Math.Abs(nearByObject.transform.position.y-characterPosition.y) < (Utilities.GetCellSize()/2)){
    //                     trajectoryClear = false;
    //                 }
    //             }
    //             break;
    //         case MoveCharacterConstants.BACKWARD:
    //             foreach(var nearByObject in nearByObjects){
    //                 if(tbWallExpression.IsMatch(nearByObject.gameObject.name) && (characterPosition.y-nearByObject.transform.position.y) <= 2f && Math.Abs(nearByObject.transform.position.x-characterPosition.x) < (Utilities.GetCellSize()/2)){
    //                     trajectoryClear = false;
    //                 }
    //             }
    //             break;
    //         case MoveCharacterConstants.RIGHT:
    //             foreach(var nearByObject in nearByObjects){
    //                 if(lrWallExpression.IsMatch(nearByObject.gameObject.name) && (nearByObject.transform.position.x-characterPosition.x) <= 2f && Math.Abs(nearByObject.transform.position.y-characterPosition.y) < (Utilities.GetCellSize()/2)){
    //                     trajectoryClear = false;
    //                 }
    //             }
    //             break;
    //     }

    //     return trajectoryClear;
    // }
}
