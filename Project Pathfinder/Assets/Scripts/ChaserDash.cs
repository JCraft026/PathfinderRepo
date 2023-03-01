using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if(timer >= 1.2f){
            this.enabled = false;
            animator.SetBool("Dashing", false);
        }
        else{
            chaserRigidBody.MovePosition(chaserRigidBody.position + 
            dashDirection * 10.0f * Time.fixedDeltaTime);
            timer += 0.01666667f;
        }    
    }

    public void startDash(){
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
        animator.SetBool("Dashing", true);
        timer = 0.0f;
        this.enabled = true;
    }
}
