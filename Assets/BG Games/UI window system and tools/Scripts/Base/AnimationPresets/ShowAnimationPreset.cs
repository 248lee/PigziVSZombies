using System;
using System.Threading.Tasks;
using BG_Games.Scripts.Base.Enums;

namespace BG_Games.Scripts.Base.AnimationPresets
{
    public class ShowAnimationPreset
    {
        public readonly ShowAnimationType ShowAnimationType;
        public readonly Func<Task> ShowTask;

        public ShowAnimationPreset(ShowAnimationType animationType, Func<Task> showTask)
        {
            ShowAnimationType = animationType;
            ShowTask = showTask;
        }

        public async void InvokeAsyncAction(Func<Task> asyncAction)
        {
            await asyncAction();
        }
    }
}