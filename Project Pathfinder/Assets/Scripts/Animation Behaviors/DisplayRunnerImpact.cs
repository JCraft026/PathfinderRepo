using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DisplayRunnerImpact : StateMachineBehaviour
{
    public Animator animator;   // Character's animator manager
    public int frameCount;       // Amount of frames passed in update statement

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MoveCharacter.canMove = false;
        frameCount = 0;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var runner = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));

        frameCount += 1;

        if(frameCount <= 4){
            switch (animator.GetFloat("Impact Direction"))
            {
                case MoveCharacterConstants.FORWARD:
                    runner.transform.position += new Vector3(0,30,0) * Time.deltaTime;
                    break;    
                case MoveCharacterConstants.LEFT:
                    runner.transform.position += new Vector3(-30,0,0) * Time.deltaTime;
                    break;   
                case MoveCharacterConstants.BACKWARD:
                    runner.transform.position += new Vector3(0,-30,0) * Time.deltaTime;
                    break;   
                case MoveCharacterConstants.RIGHT:
                    runner.transform.position += new Vector3(30,0,0) * Time.deltaTime;
                    break; 
            }
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
