using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnimalStayAtHomeState : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<AnimalController>().SetGraphAnimatorState("idle");
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimalMovementController animalController = animator.GetComponent<AnimalMovementController>();

        // Move to the right place
        Vector3 targetPosition = AnimalSM.TargetOfHome.GetTargetPosition(animalController);
        if ((animator.transform.position.x - targetPosition.x) > 5 * animalController.stepSize)  // if the animal is righter than the target
        {
            animator.GetComponent<AnimalMovementController>().SetRunThisFrame(direction: Vector3.left);
        }
        else if ((animator.transform.position.x - targetPosition.x) < -5 * animalController.stepSize)  // if the animal is lefter than the target
        {
            animator.GetComponent<AnimalMovementController>().SetRunThisFrame(direction: Vector3.right);
        }
        else  // else, the animal arrived its target
        {
            // pass
        }
    }
}

namespace AnimalSM
{
    public class TargetOfHome
    {
        public static Vector3 GetTargetPosition(AnimalMovementController animalController)
        {
            Vector3 targetPosition;
            if (animalController.isSelfHomeLeftOccupied)
                targetPosition = animalController.selfHomeRightPosition;
            else if (animalController.isSelfHomeRightOccupied)
                targetPosition = animalController.selfHomeLeftPosition;
            else
                targetPosition = animalController.selfHomeMiddlePosition;

            return targetPosition;
        }
    }
}
