using BG_Games.Scripts.Buttons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BG_Games.Sample.Scripts.Buttons
{
    /// <summary>
    /// This is a sample switch class 
    /// </summary>
    public class ChangeColorSwitch : UISwitch
    {
        [Header("Change Color Switch Settings")]
        [SerializeField] private Image _imageToChangeColor;
        [SerializeField] private Color _initial;
        [SerializeField] private TMP_Text _label;

        private Color _changed;
        
        protected override void OnSwitched()
        {
            base.OnSwitched();
            
            _changed = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 255f);
            _imageToChangeColor.color = IsOn ? _initial : _changed;

            _label.text = IsOn ? "Change to New Color" : "Change to Initial Color";
        }
    }
}