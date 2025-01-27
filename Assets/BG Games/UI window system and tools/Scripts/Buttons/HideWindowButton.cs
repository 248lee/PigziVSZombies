using BG_Games.Scripts.Base;
using BG_Games.Scripts.Base.Enums;
using UnityEngine;

namespace BG_Games.Scripts.Buttons
{
    [AddComponentMenu("UI system & tools/Buttons/Hide Window Button")]
    public class HideWindowButton : UIButton
    {
        [Header("Hide window button")]
        [SerializeField] private UIWindowType _windowType;
        [SerializeField] private bool _hideParentWindow;

        private UIWindow _windowToHide;

        protected override void Awake()
        {
            base.Awake();
            
            if (_hideParentWindow)
            {
                _windowToHide = GetComponentInParent<UIWindow>();
                if (!_windowToHide)
                {
                    Debug.LogError("No UIWindow component found on the button parent");
                }
            }

            AssignAction(HideWindow);
        }

        private async void HideWindow()
        {
            if (_hideParentWindow)
            {
                await _windowToHide.Hide();
                return;
            }

            if (_windowType != UIWindowType.Default)
            {
                await OverlayUI.Instance.HideWindow(_windowType);
            }
        }
    }
}