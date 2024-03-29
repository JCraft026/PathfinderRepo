using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessFastTravelEnd : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Transport guard to destination maze cell
        animator.gameObject.transform.position = new Vector3(animator.GetFloat("Fast Travel X"), animator.GetFloat("Fast Travel Y"), 0);    
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset Fast Travel parameter variables
        animator.SetBool("Fast Travel Started", false);
        animator.SetBool("Fast Travel Finished", false);
        animator.SetFloat("Fast Travel X", 0.0f);
        animator.SetFloat("Fast Travel Y", 0.0f);

        // Restore guard movement
        animator.gameObject.GetComponent<MoveCharacter>().canMove = true;
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
