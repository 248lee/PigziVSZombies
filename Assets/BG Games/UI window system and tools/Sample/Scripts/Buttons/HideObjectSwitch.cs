using BG_Games.Scripts.Buttons;
using TMPro;
using UnityEngine;

namespace BG_Games.Sample.Scripts.Buttons
{
    /// <summary>
    /// This is a sample switch class
    /// </summary>
    public class HideObjectSwitch : UISwitch
    {
        [Header("Hide object Switch Settings")]
        [SerializeField] private GameObject _gameObjectToHide;
        [SerializeField] private TMP_Text _label;
        
        protected override void OnSwitched()
        {
            base.OnSwitched();

            _label.text = IsOn ? "Hide object above" : "Show object above";
            _gameObjectToHide.SetActive(IsOn);
        }
    }
}