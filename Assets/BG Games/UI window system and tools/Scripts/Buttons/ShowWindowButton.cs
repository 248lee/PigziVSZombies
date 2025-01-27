using BG_Games.Scripts.Base;
using BG_Games.Scripts.Base.Enums;
using UnityEngine;

namespace BG_Games.Scripts.Buttons
{
    [AddComponentMenu("UI system & tools/Buttons/Show Window Button")]
    public class ShowWindowButton : UIButton
    {
        [Header("Show window button")]
        [SerializeField] private UIWindowType _windowType;

        protected override void Awake()
        {
            base.Awake();
            
            AssignAction(ShowWindow);
        }

        private async void ShowWindow()
        {
            var window = OverlayUI.Instance.LoadWindow(_windowType);
            await window.Show(SiblingType.Last);
        }
    }
}