using System.Collections.Generic;
using UnityEngine;

namespace BG_Games.Scripts.Buttons.Controllers
{
    /// <summary>
    /// Manages the state of a collection of UI buttons.
    /// </summary>
    public class ButtonStateController : MonoBehaviour
    {
        /// <summary>
        /// List of UI buttons managed by this controller.
        /// </summary>
        [SerializeField] private List<UIButton> _buttons;

        /// <summary>
        /// Registers the OnButtonClicked event handler to each button's Clicked event.
        /// </summary>
        private void Awake()
        {
            _buttons.ForEach(button => button.Clicked += OnButtonClicked);
        }

        /// <summary>
        /// Event handler for button click events.
        /// Updates the color and sprite of each button based on whether it matches the clicked button.
        /// </summary>
        /// <param name="hashCode">The hash code of the clicked button.</param>
        private void OnButtonClicked(int hashCode)
        {
            foreach (UIButton button in _buttons)
            {
                bool isCurrentButton = button.HashCode == hashCode;

                button.SetColor(isCurrentButton);
                button.AssignSprite(isCurrentButton);
            }
        }
    }
}