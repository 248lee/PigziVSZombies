using System;
using System.Threading.Tasks;
using BG_Games.Scripts.Base.Enums;
using UnityEngine;

namespace BG_Games.Scripts.Base
{
    [AddComponentMenu("UI system & tools/UI Window")]
    [RequireComponent(typeof(CanvasGroup))]
    public class UIWindow : MonoBehaviour
    {
        public event Action Shown;
        public event Action Hidden;
        public event Action BeforeShown;

        [SerializeField] private UIWindowAnimator _uiWindowAnimator;
        private CanvasGroup _canvasGroup;

        protected virtual async void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            await SetCanvasGroupVisibility(false);
            _uiWindowAnimator.Init(_canvasGroup, transform);

            _uiWindowAnimator.BeforeShown += OnBeforeShown;
            _uiWindowAnimator.Shown += OnShown;
            _uiWindowAnimator.Hidden += OnHidden;
        }

        /// <summary>
        /// Shows the window and sets it`s transform sibling as last or first depending on the passed parameter.
        /// Play`s the show animation if any was selected
        /// </summary>
        /// <param name="sibling">transform sibling type</param>
        public async Task Show(SiblingType sibling)
        {
            if (_uiWindowAnimator.Busy) return;

            await SetSibling(sibling);
            
            await _uiWindowAnimator.ShowAsync();
            await SetCanvasGroupVisibility(true);
        }

        /// <summary>
        /// Hides the window with the according animation 
        /// </summary>
        public async Task Hide()
        {
            if (_uiWindowAnimator.Busy) return;

            await _uiWindowAnimator.HideAsync();
            await SetCanvasGroupVisibility(false);
        }

        private async Task SetCanvasGroupVisibility(bool state)
        {
            _canvasGroup.alpha = state ? 1f : 0f;
            _canvasGroup.interactable = state;
            _canvasGroup.blocksRaycasts = state;

            await Task.CompletedTask;
        }

        private async Task SetSibling(SiblingType sibling)
        {
            if (sibling == SiblingType.First)
                transform.SetAsFirstSibling();
            else
                transform.SetAsLastSibling();

            await Task.CompletedTask;
        }

        private void OnDestroy()
        {
            _uiWindowAnimator.Cancel();
        }

        /// <summary>
        /// Stop the animation that is currently playing on panel
        /// </summary>
        public void CancelAnimation()
        {
            _uiWindowAnimator.Cancel();
        }

        /// <summary>
        /// Called when the window show process is finished
        /// </summary>
        protected virtual void OnShown()
        {
            Shown?.Invoke();
        }

        /// <summary>
        /// Called right before the window show process is started
        /// </summary>
        protected virtual void OnBeforeShown()
        {
            BeforeShown?.Invoke();
        }

        /// <summary>
        /// Called when the window hide process is finished
        /// </summary>
        protected virtual void OnHidden()
        {
            Hidden?.Invoke();
        }

        /// <summary>
        /// Sets a new show animation type for the window. Removes the previous one
        /// </summary>
        /// <param name="animationType">New animation type</param>
        public void SetShowAnimationType(ShowAnimationType animationType)
        {
            _uiWindowAnimator.SetShowAnimation(animationType);
        }

        /// <summary>
        /// Sets a new hide animation type for the window. Removes the previous one
        /// </summary>
        /// <param name="animationType">New animation type</param>
        public void SetHideAnimationType(HideAnimationType animationType)
        {
            _uiWindowAnimator.SetHideAnimation(animationType);
        }
    }
}