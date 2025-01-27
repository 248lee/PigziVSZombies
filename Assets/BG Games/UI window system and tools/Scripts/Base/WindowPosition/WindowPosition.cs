using BG_Games.Scripts.Base.Enums;
using UnityEngine;

namespace BG_Games.Scripts.Base.WindowPosition
{
    public class WindowPosition
    {
        public readonly Vector3 Position;
        public readonly PositionFromScreen WindowToScreenPosition;

        public WindowPosition(Vector3 position, PositionFromScreen positionFromScreen)
        {
            Position = position;
            WindowToScreenPosition = positionFromScreen;
        }
    }
}