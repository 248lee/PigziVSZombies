using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnimalStayAtHomeState : StateMachineBehaviour
{
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimalController animalController = animator.GetComponent<AnimalController>();
        Vector3 targetPosition = AnimalSM.TargetOfHome.GetTargetPosition(animalController);

        // Move to the right place
        if ((animator.transform.position.x - targetPosition.x) > 5 * animalController.stepSize)  // if the animal is righter than the target
        {
            animator.GetComponent<AnimalController>().SetRunThisFrame(direction: Vector3.left);
        }
        else if ((animator.transform.position.x - targetPosition.x) < -5 * animalController.stepSize)  // if the animal is lefter than the target
        {
            animator.GetComponent<AnimalController>().SetRunThisFrame(direction: Vector3.right);
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
        public static Vector3 GetTargetPosition(AnimalController animalController)
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