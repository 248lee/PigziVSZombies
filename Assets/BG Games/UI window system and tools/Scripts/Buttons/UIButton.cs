using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BG_Games.Scripts.Buttons
{
    [AddComponentMenu("UI system & tools/Buttons/UI Button")]
    public class UIButton : Selectable, IPointerClickHandler
    {
        [field: Header("Button Settings")]
        [field: SerializeField] protected bool CanScale { get; private set; } = true;
        [field: SerializeField] protected float ScaleFactor { get; private set; } = 1.1f;

        public int HashCode { get; private set; } 
        public event Action<int> Clicked;
        private event Action OnClick;
        
        private bool _wasTouched;
        
        private TMP_Text _buttonText;
        private Image _innerIcon;
        private Sprite _defaultSprite;
        
        protected override void Awake()
        {
            base.Awake();
            
            _buttonText = GetComponentInChildren<TMP_Text>();
            _innerIcon = GetComponentInChildren<Image>();
            
            HashCode = GetHashCode();
            _defaultSprite = image.sprite;
        }
        
        /// <summary>
        /// Sets the interactable state of the button
        /// </summary>
        /// <param name="state"></param>
        public void SetAvailability(bool state)
        {
            interactable = state;
        }

        /// <summary>
        /// Sets gameobject active or inactive respectively 
        /// </summary>
        /// <param name="state">state of gameobject's activeSelf</param>
        public void SetVisibility(bool state)
        {
            gameObject.SetActive(state);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            
            if(!interactable) return;
            if (_wasTouched) return;

            if (CanScale)
            {
                transform.localScale /= ScaleFactor;
            }

            _wasTouched = true;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            
            if(!interactable) return;
            if (!_wasTouched) return;

            if (CanScale)
            {
                transform.localScale *= ScaleFactor;
            }

            _wasTouched = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(!interactable) return;
            
            OnClick?.Invoke();
            Clicked?.Invoke(HashCode);
        }

        /// <summary>
        /// Sets the action that will be invoked on button click. Removes all previous actions from the event
        /// </summary>
        /// <param name="newAction">Action to be invoked</param>
        public void AssignAction(Action newAction)
        {
            OnClick = null;
            OnClick += newAction;
        }

        public void AssignText(string text)
        {
            if (_buttonText != null)
            {
                _buttonText.text = text;
            }
        }
        
        public void AssignInnerIcon(Sprite sprite)
        {
            if (_innerIcon != null)
            {
                _innerIcon.sprite = sprite;
            }
        }
        
        // sets the color of target graphic corresponding Selectable component state
        public void SetColor(bool isSelected)
        {
            targetGraphic.color = isSelected ? colors.selectedColor : colors.normalColor;
        }
        
        // sets the sprite of target graphic corresponding Selectable component state
        public void AssignSprite(bool isSelected)
        {
            image.sprite = isSelected ? spriteState.selectedSprite : _defaultSprite;
        }
    }
}
