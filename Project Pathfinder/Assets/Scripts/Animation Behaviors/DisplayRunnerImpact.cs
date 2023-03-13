using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
using System;

public class DisplayRunnerImpact : StateMachineBehaviour
{
    public Animator animator; // Character's animator manager
    public int frameCount;    // Amount of frames passed in update statement

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<MoveCharacter>().canMove = false;
        frameCount = 0;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var runner = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
                         // Runner game object
        frameCount += 1; // Amount of frames passed

        // Display runner throwback force from guard master attack
        if(frameCount <= 4 && CustomNetworkManager.isRunner){
            switch (animator.GetFloat("Impact Direction"))
            {
                case MoveCharacterConstants.FORWARD:
                    for(int moveNudges = 30; moveNudges > 0; moveNudges--){
                        if(ImpactTrajectoryClear(runner.transform.position, MoveCharacterConstants.FORWARD))
                        {
                            runner.transform.position += new Vector3(0,1,0) * Time.deltaTime;
                        }
                    }
                    break;    
                case MoveCharacterConstants.LEFT:
                    for(int moveNudges = 30; moveNudges > 0; moveNudges--){
                        if(ImpactTrajectoryClear(runner.transform.position, MoveCharacterConstants.LEFT))
                        {
                            runner.transform.position += new Vector3(-1,0,0) * Time.deltaTime;
                        }
                    }
                    break;   
                case MoveCharacterConstants.BACKWARD:
                    for(int moveNudges = 30; moveNudges > 0; moveNudges--){
                        if(ImpactTrajectoryClear(runner.transform.position, MoveCharacterConstants.BACKWARD))
                        {
                            runner.transform.position += new Vector3(0,-1,0) * Time.deltaTime;
                        }
                    }
                    break;   
                case MoveCharacterConstants.RIGHT:
                    for(int moveNudges = 30; moveNudges > 0; moveNudges--){
                        if(ImpactTrajectoryClear(runner.transform.position, MoveCharacterConstants.RIGHT))
                        {
                            runner.transform.position += new Vector3(1,0,0) * Time.deltaTime;
                        }
                    }
                    break; 
            }
        }
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<MoveCharacter>().canMove = true;
        animator.SetBool("Hurt", false);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
    
    // Determine whether there are no walls in the way of the guard attack impact trajectory to avoid the runner phasing through walls
    public bool ImpactTrajectoryClear(Vector2 characterPosition, float moveDirection){
        bool trajectoryClear = true;
        Regex lrWallExpression = new Regex("Wall_LR"); // Match left and right walls
        Regex tbWallExpression = new Regex("Wall_TB"); // Match top and bottom walls
        Collider2D[] nearByObjects = Physics2D.OverlapCircleAll(characterPosition, 1f);
                                                       // Collider objects near the runner

        // Move the runner in the appropriate direction based on the attack direction to simulate guard attack impact
        switch (moveDirection)
        {
            case MoveCharacterConstants.FORWARD:
                foreach(var nearByObject in nearByObjects){
                    if(tbWallExpression.IsMatch(nearByObject.gameObject.name) && (nearByObject.transform.position.y-characterPosition.y) <= 2f && Math.Abs(nearByObject.transform.position.x-characterPosition.x) < (Utilities.GetCellSize()/2)){
                        trajectoryClear = false;
                    }
                }
                break;
            case MoveCharacterConstants.LEFT:
                foreach(var nearByObject in nearByObjects){
                    if(lrWallExpression.IsMatch(nearByObject.gameObject.name) && (characterPosition.x-nearByObject.transform.position.x) <= 2f && Math.Abs(nearByObject.transform.position.y-characterPosition.y) < (Utilities.GetCellSize()/2)){
                        trajectoryClear = false;
                    }
                }
                break;
            case MoveCharacterConstants.BACKWARD:
                foreach(var nearByObject in nearByObjects){
                    if(tbWallExpression.IsMatch(nearByObject.gameObject.name) && (characterPosition.y-nearByObject.transform.position.y) <= 2f && Math.Abs(nearByObject.transform.position.x-characterPosition.x) < (Utilities.GetCellSize()/2)){
                        trajectoryClear = false;
                    }
                }
                break;
            case MoveCharacterConstants.RIGHT:
                foreach(var nearByObject in nearByObjects){
                    if(lrWallExpression.IsMatch(nearByObject.gameObject.name) && (nearByObject.transform.position.x-characterPosition.x) <= 2f && Math.Abs(nearByObject.transform.position.y-characterPosition.y) < (Utilities.GetCellSize()/2)){
                        trajectoryClear = false;
                    }
                }
                break;
        }

        return trajectoryClear;
    }
}
