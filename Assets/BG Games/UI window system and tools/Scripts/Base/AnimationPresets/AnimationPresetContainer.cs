using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BG_Games.Scripts.Base.Enums;
using BG_Games.Scripts.Base.WindowPosition;

namespace BG_Games.Scripts.Base.AnimationPresets
{
    public class AnimationPresetContainer
    {
        public readonly List<ShowAnimationPreset> ShowPresets;
        public readonly List<HideAnimationPreset> HidePresets;

        public Func<Task> this[ShowAnimationType animationType] =>
            ShowPresets.FirstOrDefault(element => element.ShowAnimationType == animationType)?.ShowTask;

        public Func<Task> this[HideAnimationType animationType] =>
            HidePresets.FirstOrDefault(element => element.HideAnimationType == animationType)?.HideAction;
        
        public AnimationPresetContainer(UIWindowAnimator animator)
        {
            WindowPositionContainer windowPositionContainer = new WindowPositionContainer();
             
            async Task AppearAbove() => await animator.MoveAsync(animator._cancellationTokenSource.Token, windowPositionContainer[PositionFromScreen.Above], windowPositionContainer[PositionFromScreen.Center]);
            async Task AppearUnder() => await animator.MoveAsync(animator._cancellationTokenSource.Token, windowPositionContainer[PositionFromScreen.Under], windowPositionContainer[PositionFromScreen.Center]);
            async Task AppearRight() => await animator.MoveAsync(animator._cancellationTokenSource.Token, windowPositionContainer[PositionFromScreen.Right], windowPositionContainer[PositionFromScreen.Center]);
            async Task AppearLeft() => await animator.MoveAsync(animator._cancellationTokenSource.Token, windowPositionContainer[PositionFromScreen.Left], windowPositionContainer[PositionFromScreen.Center]);
            async Task FadeIn() => await animator.FadeInAsync(animator._cancellationTokenSource.Token);
            async Task ScaleUp() => await animator.ScaleUpAsync(animator._cancellationTokenSource.Token);
            async Task ShowInstantly() => await animator.ShowInstantly();
            
            ShowPresets = new List<ShowAnimationPreset>()
            {
                new (ShowAnimationType.None, ShowInstantly),
                new (ShowAnimationType.AppearAbove, AppearAbove),
                new (ShowAnimationType.AppearUnder, AppearUnder),
                new (ShowAnimationType.AppearLeft, AppearLeft),
                new (ShowAnimationType.AppearRight, AppearRight),
                new (ShowAnimationType.ScaleUp, ScaleUp),
                new (ShowAnimationType.FadeIn, FadeIn),
            };

            async Task GoAbove() => await animator.MoveAsync(animator._cancellationTokenSource.Token, windowPositionContainer[PositionFromScreen.Center], windowPositionContainer[PositionFromScreen.Above]);
            async Task GoUnder() => await animator.MoveAsync(animator._cancellationTokenSource.Token, windowPositionContainer[PositionFromScreen.Center], windowPositionContainer[PositionFromScreen.Under]);
            async Task GoLeft() => await animator.MoveAsync(animator._cancellationTokenSource.Token, windowPositionContainer[PositionFromScreen.Center], windowPositionContainer[PositionFromScreen.Left]);
            async Task GoRight() => await animator.MoveAsync(animator._cancellationTokenSource.Token, windowPositionContainer[PositionFromScreen.Center], windowPositionContainer[PositionFromScreen.Right]);
            async Task ScaleDown() => await animator.ScaleDownAsync(animator._cancellationTokenSource.Token);
            async Task FadeOut() => await animator.FadeOutAsync(animator._cancellationTokenSource.Token);
            async Task HideInstantly() => await animator.HideInstantly();
            
            HidePresets = new List<HideAnimationPreset>()
            {
                new (HideAnimationType.None, HideInstantly),
                new (HideAnimationType.GoAbove, GoAbove),
                new (HideAnimationType.GoUnder, GoUnder),
                new (HideAnimationType.GoLeft, GoLeft),
                new (HideAnimationType.GoRight, GoRight),
                new (HideAnimationType.ScaleDown ,ScaleDown),
                new (HideAnimationType.FadeOut, FadeOut),
            };
        }
    }
}