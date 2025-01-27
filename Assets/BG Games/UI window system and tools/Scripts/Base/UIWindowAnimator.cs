using System;
using System.Threading;
using System.Threading.Tasks;
using BG_Games.Scripts.Base.AnimationPresets;
using BG_Games.Scripts.Base.Enums;
using UnityEngine;

namespace BG_Games.Scripts.Base
{
    [Serializable]
    public class UIWindowAnimator
    {
        public CancellationTokenSource _cancellationTokenSource;
        private CanvasGroup _canvasGroup;
        private Transform _transform;
        private Vector3 _originalScale;
        private AnimationPresetContainer _animationPresetContainer;

        [SerializeField] private float _animationDuration = 0.5f;
        [field: SerializeField] public ShowAnimationType ShowAnimation { get; private set; }
        [field: SerializeField] public HideAnimationType HideAnimation { get; private set; }

        public bool Busy { get; private set; }

        public event Action Shown;
        public event Action Hidden;
        public event Action BeforeShown; 

        /// <summary>
        /// Shows the window using the given animation
        /// </summary>
        public async Task ShowAsync()
        {
            Cancel();
            
            BeforeShown?.Invoke();
            await _animationPresetContainer[ShowAnimation]();
            Shown?.Invoke();
        }

        /// <summary>
        /// Invokes the cancellation token source event to stop async methods
        /// </summary>
        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            ResetLocalScale();
            Busy = false;
        }

        public async Task MoveAsync(CancellationToken cancellationToken, Vector3 initialPos,
            Vector3 targetPos)
        {
            Busy = true;

            _canvasGroup.alpha = 1f;

            float elapsedTime = 0f;
            while (elapsedTime < _animationDuration)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                _transform.position = Vector3.Lerp(initialPos, targetPos, elapsedTime / _animationDuration);
                elapsedTime += Time.deltaTime;
                await Task.Yield();
            }

            _transform.position = Vector3.Lerp(initialPos, targetPos, 1f);
            Busy = false;
        }

        /// <summary>
        /// Hides the window using the given animation
        /// </summary>
        public async Task HideAsync()
        {
            Cancel();
            await _animationPresetContainer[HideAnimation]();
            Hidden?.Invoke();
        }

        public async Task ScaleUpAsync(CancellationToken cancellationToken)
        {
            Busy = true;

            _transform.localScale = Vector3.zero;
            Vector3 currentScale = _transform.localScale;
            Vector3 targetScale = _originalScale;
            _canvasGroup.alpha = 1f;

            float elapsedTime = 0f;
            while (elapsedTime < _animationDuration)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;
                _transform.localScale = Vector3.Lerp(currentScale, targetScale, elapsedTime / _animationDuration);
                elapsedTime += Time.deltaTime;
                await Task.Yield();
            }

            _transform.localScale = targetScale;
            Busy = false;
        }

        public async Task ScaleDownAsync(CancellationToken cancellationToken)
        {
            Busy = true;

            Vector3 currentScale = _transform.localScale;
            Vector3 targetScale = Vector3.zero;

            float elapsedTime = 0f;
            while (elapsedTime < _animationDuration)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                _transform.localScale = Vector3.Lerp(currentScale, targetScale, elapsedTime / _animationDuration);
                elapsedTime += Time.deltaTime;
                await Task.Yield();
            }

            _transform.localScale = targetScale;
            _canvasGroup.alpha = 0f;
            Busy = false;
        }

        public async Task FadeInAsync(CancellationToken cancellationToken)
        {
            Busy = true;

            ResetLocalScale();
            float elapsedTime = 0f;
            while (elapsedTime < _animationDuration)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / _animationDuration);
                elapsedTime += Time.deltaTime;
                await Task.Yield();
            }

            _canvasGroup.alpha = 1f;
            Busy = false;
        }

        public async Task FadeOutAsync(CancellationToken cancellationToken)
        {
            Busy = true;

            float elapsedTime = 0f;
            while (elapsedTime < _animationDuration)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / _animationDuration);
                elapsedTime += Time.deltaTime;
                await Task.Yield();
            }

            _canvasGroup.alpha = 0f;
            Busy = false;
        }

        public async Task ShowInstantly()
        {
            _canvasGroup.alpha = 1f;
        }

        public async Task HideInstantly()
        {
            _canvasGroup.alpha = 0f;
        }

        public void Init(CanvasGroup canvasGroup, Transform transform)
        {
            _canvasGroup = canvasGroup;
            _transform = transform;
            _originalScale = transform.localScale;

            _cancellationTokenSource = new CancellationTokenSource();
            _animationPresetContainer = new AnimationPresetContainer(this);
        }

        private void ResetLocalScale()
        {
            _transform.localScale = _originalScale;
        }
        
        public void SetShowAnimation(ShowAnimationType showAnimationType)
        {
            ShowAnimation = showAnimationType;
        }

        public void SetHideAnimation(HideAnimationType hideAnimationType)
        {
            HideAnimation = hideAnimationType;
        }
    }
}