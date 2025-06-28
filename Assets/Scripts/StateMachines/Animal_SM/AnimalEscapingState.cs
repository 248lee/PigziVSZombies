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
            animator.GetComponent<AnimalMovementController>().OccupyLeftHome();
            animator.GetComponent<AnimalController>().SetGraphAnimatorState("run");
            // Make animal face left
            animator.GetComponent<AnimalController>().SetFaceLeft();
        }
        else
        {
            animator.GetComponent<AnimalMovementController>().OccupyRightHome();
            animator.GetComponent<AnimalController>().SetGraphAnimatorState("run");
            // Make animal face right
            animator.GetComponent<AnimalController>().SetFaceRight();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimalMovementController animalController = animator.GetComponent<AnimalMovementController>();
        Vector3 targetPosition;
        if (this.escapingTo == EscapingTo.TheLeft)
            targetPosition = animalController.leftHomeTargetPosition;
        else
            targetPosition = animalController.rightHomeTargetPosition;

        if ((animator.transform.position.x - targetPosition.x) > 5 * animalController.stepSize)  // if the animal is righter than the target
        {
            animalController.SetRunThisFrame(direction: Vector3.left);
        }
        else if ((animator.transform.position.x - targetPosition.x) < -5 * animalController.stepSize)  // if the animal is lefter than the target
        {
            animalController.SetRunThisFrame(direction: Vector3.right);
        }
        else  // else, the animal arrived its target
        {
            animator.SetTrigger("FinishEscaping");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("FinishEscaping");
        // Reset animal to face right
        animator.GetComponent<AnimalController>().SetFaceRight();
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
