using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EscapingTo
{
    TheLeft,
    TheRight
}
public class AnimalEscapingState : StateMachineBehaviour
{
    public EscapingTo escapingTo;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (escapingTo == EscapingTo.TheLeft)
        {
            animator.GetComponent<AnimalController>().OccupyLeftHome();
        }
        else
        {
            animator.GetComponent<AnimalController>().OccupyRightHome();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 targetPosition;
        if (this.escapingTo == EscapingTo.TheLeft)
            targetPosition = animator.GetComponent<AnimalController>().leftHomeTargetPosition;
        else
            targetPosition = animator.GetComponent<AnimalController>().rightHomeTargetPosition;

        if ((animator.transform.position.x - targetPosition.x) > .25f)  // if the animal is righter than the target
        {
            animator.GetComponent<AnimalController>().SetRunThisFrame(direction: Vector3.left);
        }
        else if ((animator.transform.position.x - targetPosition.x) < -.25f)  // if the animal is lefter than the target
        {
            animator.GetComponent<AnimalController>().SetRunThisFrame(direction: Vector3.right);
        }
        else  // else, the animal arrived its target
        {
            animator.SetTrigger("FinishEscaping");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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