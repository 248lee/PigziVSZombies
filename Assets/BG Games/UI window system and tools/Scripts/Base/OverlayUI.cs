using System.Collections.Generic;
using System.Threading.Tasks;
using BG_Games.Scripts.Base.Enums;
using UnityEngine;

namespace BG_Games.Scripts.Base
{
    [AddComponentMenu("UI system & tools/Overlay UI")]
    public class OverlayUI : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private UIWindowListSO _uiWindowListSO;

        /// <summary>
        /// You can always replace this singleton with dependency injections 
        /// using Zenject or in any suitable way
        /// </summary>
        public static OverlayUI Instance { get; private set; }
        private void Awake() => Instance = this;

        /// <summary>
        /// Destroys all of the loaded windows
        /// </summary>
        public void UnloadAllWindows()
        {
            foreach (var window in _uiWindowListSO.Windows)
            {
                if (window.Instance != null)
                {
                    UIWindowLoader.Unload(window);
                }
            }
        }

        /// <summary>
        /// Hides all currently shown windows
        /// </summary>
        public async Task HideAllWindows()
        {
            List<Task> hideTasks = new List<Task>();

            foreach (var window in _uiWindowListSO.Windows)
            {
                if (window.Instance != null)
                {
                    hideTasks.Add(window.Instance.Hide());
                }
            }

            await Task.WhenAll(hideTasks);
        }

        /// <summary>
        /// Loads window by type
        /// </summary>
        /// <typeparam name="T">Inheritor of UIWindow</typeparam>
        /// <returns>Window instance of type T with base class UIWindow</returns>
        public T LoadWindow<T>() where T : UIWindow
        {
            return UIWindowLoader.Load(_uiWindowListSO[typeof(T)], _canvas) as T;
        }

        /// <summary>
        /// Loading window by its UIWindowType type. Useful for using in inspector,
        /// for instance - HideWindowButton.cs
        /// </summary>
        /// <param name="windowType">window type of UIWindowType enum</param>
        /// <returns>Window instance that corresponds to windowType from _uiWindowListSO</returns>
        public UIWindow LoadWindow(UIWindowType windowType)
        {
            return UIWindowLoader.Load(_uiWindowListSO[windowType], _canvas);
        }

        /// <summary>
        /// Destroys a given window instance
        /// </summary>
        /// <param name="instance">UIWindow inheritor instance</param>
        public void UnloadWindow(UIWindow instance)
        {
            foreach (var we in _uiWindowListSO.Windows)
            {
                if (we.Instance != instance) continue;
                UIWindowLoader.Unload(we);
            }
        }
        
        /// <summary>
        /// Shows the window by its UIWindowType type. Useful for using in inspector.
        /// </summary>
        /// <param name="windowType"></param>
        public async Task ShowWindow(UIWindowType windowType)
        {
            await UIWindowLoader.Load(_uiWindowListSO[windowType], _canvas).Show(SiblingType.Last);
        }

        /// <summary>
        /// Hides the window by its UIWindowType type. Useful for using in inspector.
        /// </summary>
        /// <param name="windowType"></param>
        public async Task HideWindow(UIWindowType windowType)
        {
            await UIWindowLoader.Load(_uiWindowListSO[windowType], _canvas).Hide();
        }
    }
}