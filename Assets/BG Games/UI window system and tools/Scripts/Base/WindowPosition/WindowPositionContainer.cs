using System.Collections.Generic;
using System.Linq;
using BG_Games.Scripts.Base.Enums;
using UnityEngine;

namespace BG_Games.Scripts.Base.WindowPosition
{
    public class WindowPositionContainer
    {
        public readonly List<WindowPosition> WindowPositions;

        public Vector3 this[PositionFromScreen position] =>
            WindowPositions.FirstOrDefault(element => element.WindowToScreenPosition == position).Position;

        public WindowPositionContainer()
        {
            float halfScreenWidth = Screen.width / 2f;
            float halfScreenHeight = Screen.height / 2f;
            
            WindowPositions = new List<WindowPosition>()
            {
                new WindowPosition(new Vector3(halfScreenWidth, halfScreenHeight, 0f), PositionFromScreen.Center),
                new WindowPosition(new Vector3(halfScreenWidth - Screen.width, halfScreenHeight, 0f), PositionFromScreen.Left),
                new WindowPosition(new Vector3(halfScreenWidth + Screen.width, halfScreenHeight, 0f), PositionFromScreen.Right),
                new WindowPosition(new Vector3(halfScreenWidth, halfScreenHeight + Screen.height, 0f), PositionFromScreen.Above),
                new WindowPosition(new Vector3(halfScreenWidth, halfScreenHeight - Screen.height, 0f), PositionFromScreen.Under)
            };
        }
    }
}