using BG_Games.Scripts.Base;
using BG_Games.Scripts.Buttons;

namespace BG_Games.Sample.Scripts.Buttons
{
    public class UnloadAllWindowsButton : UIButton
    {
        protected override void Start()
        {
            base.Start();
            AssignAction(UnloadWindows);
        }

        private void UnloadWindows()
        {
            OverlayUI.Instance.UnloadAllWindows();
        }
    }
}