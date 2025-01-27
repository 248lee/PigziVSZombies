using System;
using System.Threading.Tasks;
using BG_Games.Scripts.Base.Enums;

namespace BG_Games.Scripts.Base.AnimationPresets
{
    public class HideAnimationPreset
    {
        public readonly HideAnimationType HideAnimationType;
        public readonly Func<Task> HideAction;

        public HideAnimationPreset(HideAnimationType animationType, Func<Task> hideAction)
        {
            HideAnimationType = animationType;
            HideAction = hideAction;
        }
    }
}