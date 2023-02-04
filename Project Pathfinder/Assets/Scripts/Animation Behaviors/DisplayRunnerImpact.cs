using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DisplayRunnerImpact : StateMachineBehaviour
{
    public Animator animator;   // Character's animator manager

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MoveCharacter.canMove = false;
        Debug.Log("Woop");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var runner = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner ["));
        switch (animator.GetFloat("Impact Direction"))
        {
            case MoveCharacterConstants.FORWARD:
                Debug.Log("Runner is thrown upward");
                //runner.transform.position = new Vector3(0,20,0) * Time.deltaTime;
                break;    
            case MoveCharacterConstants.LEFT:
                Debug.Log("Runner is thrown to the left");
                //runner.transform.position = new Vector3(-20,0,0) * Time.deltaTime;
                break;   
            case MoveCharacterConstants.BACKWARD:
                Debug.Log("Runner is thrown downward");
                //runner.transform.position = new Vector3(0,-20,0) * Time.deltaTime;
                break;   
            case MoveCharacterConstants.RIGHT:
                Debug.Log("Runner is thrown to the right");
                //runner.transform.position = new Vector3(20,0,0) * Time.deltaTime;
                break; 
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MoveCharacter.canMove = true;
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
}
