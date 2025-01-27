using System;
using System.Collections.Generic;
using System.Linq;
using BG_Games.Scripts.Base.Enums;
using UnityEngine;

namespace BG_Games.Scripts.Base
{
    /// <summary>
    /// This is a container of all UIWindow inheritors. 
    /// Using indexers we can get the instance of the window using its UIWindowType value
    /// </summary>
    [CreateAssetMenu(fileName = "UI Window List", menuName = "ScriptableObjects/UI/New Window List", order = 0)]
    public class UIWindowListSO : ScriptableObject
    {
        [field: SerializeField] public List<UIWindowListElement> Windows { get; private set; }

        public UIWindowListElement this[UIWindowType windowType] =>
            Windows.FirstOrDefault(element => element.Type == windowType);

        public UIWindowListElement this[Type uiWindow] =>
            Windows.FirstOrDefault(element => element.Prefab.GetType() == uiWindow);
    }
}