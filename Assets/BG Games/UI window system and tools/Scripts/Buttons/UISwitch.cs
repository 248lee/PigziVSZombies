using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BG_Games.Scripts.Buttons
{
    [AddComponentMenu("UI system & tools/Buttons/UI Switch")]
    public class UISwitch : UIButton
    {
        [Header("UI SWITCH")] [SerializeField] protected Image SwitchBackgound;

        [SerializeField] protected Color OffColor;
        [SerializeField] protected Color OnColor;

        [SerializeField] protected Transform OffPos;
        [SerializeField] protected Transform OnPos;

        [SerializeField] protected Transform Handle;
        [SerializeField] protected float HandleTransitionTime;

        protected bool IsOn = true;

        private Color _currentStartColor => IsOn ? OnColor : OffColor;
        private Color _currentEndColor => IsOn ? OffColor : OnColor;

        private Vector3 _currentStartPos => IsOn ? OnPos.position : OffPos.position;
        private Vector3 _currentEndPos => IsOn ? OffPos.position : OnPos.position;

        private CancellationTokenSource _cancellationTokenSource;
        private bool _busy;

        protected override void Awake()
        {
            base.Awake();

            _cancellationTokenSource = new CancellationTokenSource();
            AssignAction(SwitchHandle);
        }

        /// <summary>
        /// Set UI Switch State to a corresponding value
        /// </summary>
        /// <param name="state"></param>
        public void SetState(bool state)
        {
            SwitchBackgound.color = state ? OnColor : OffColor;
            Handle.position = state ? OnPos.position : OffPos.position;
            IsOn = state;
        }
        
        protected override void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        private async void SwitchHandle()
        {
            if (!_busy)
            {
                _busy = true;
                await Task.WhenAll(MoveObjectOverTime(_cancellationTokenSource.Token),
                    ChangeImageColorOverTime(_cancellationTokenSource.Token));

                OnSwitched();
            }
        }

        protected virtual void OnSwitched()
        {
            IsOn = !IsOn;
            _busy = false;
        }

        private async Task ChangeImageColorOverTime(CancellationToken cancellationToken)
        {
            float elapsedTime = 0f;

            while (elapsedTime < HandleTransitionTime)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                float t = Mathf.Clamp01(elapsedTime / HandleTransitionTime);
                SwitchBackgound.color = Color.Lerp(_currentStartColor, _currentEndColor, t);
                elapsedTime += Time.deltaTime;
                await Task.Yield();
            }

            SwitchBackgound.color = _currentEndColor;
        }

        private async Task MoveObjectOverTime(CancellationToken cancellationToken)
        {
            float elapsedTime = 0f;

            while (elapsedTime < HandleTransitionTime)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                float t = Mathf.Clamp01(elapsedTime / HandleTransitionTime);
                Handle.position = Vector3.Lerp(_currentStartPos, _currentEndPos, t);
                elapsedTime += Time.deltaTime;
                await Task.Yield();
            }

            Handle.position = _currentEndPos;
        }
    }
}