using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AnimatorCleaer : MonoBehaviour
{
    /// <summary>
    /// 清除所有的激活中的trigger缓存
    /// </summary>
    public static void ResetAllTriggers(Animator animator)
    {
        AnimatorControllerParameter[] aps = animator.parameters;
        for (int i = 0; i < aps.Length; i++)
        {
            AnimatorControllerParameter paramItem = aps[i];
            if (paramItem.type == AnimatorControllerParameterType.Trigger)
            {
                string triggerName = paramItem.name;
                bool isActive = animator.GetBool(triggerName);
                if (isActive)
                {
                    animator.ResetTrigger(triggerName);
                }
            }
        }
    }

}
