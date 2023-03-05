using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessFastTravelStart : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Freeze guard movement
        animator.gameObject.GetComponent<MoveCharacter>().canMove = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var guardId = animator.gameObject.GetComponent<ManageActiveCharacters>().guardId;
            // Guard ID of the parent guard object

        // Transport guard to waiting area
        if(guardId == ManageActiveCharactersConstants.CHASER){
            animator.gameObject.transform.position = GameObject.Find("TravelIdleC").transform.position;
        }
        else if(guardId == ManageActiveCharactersConstants.ENGINEER){
            animator.gameObject.transform.position = GameObject.Find("TravelIdleE").transform.position;
        }
        else{
            animator.gameObject.transform.position = GameObject.Find("TravelIdleT").transform.position;
        }

        // Initiate idle waiting with timer
        GameObject.Find("EventHandler").GetComponent<ManageFastTravel>().InitiateFastTravelIdle(animator.gameObject.GetComponent<ManageActiveCharacters>().guardId);
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
}
