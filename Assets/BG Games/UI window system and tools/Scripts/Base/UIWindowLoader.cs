using UnityEngine;

namespace BG_Games.Scripts.Base
{
    public static class UIWindowLoader
    {
        /// <summary>
        /// If a window was already loaded -> returns its current instance. Otherwise, instantiates
        /// a window and returns its instance. 
        /// </summary>
        /// <param name="element">Window element</param>
        /// <param name="canvas">Canvas to assign window to</param>
        /// <returns>UI Window instance</returns>
        public static UIWindow Load(UIWindowListElement element, Canvas canvas)
        {
            if (element.Instance != null)
            {
                return element.Instance;
            }

            element.Instance = Object.Instantiate(element.Prefab, canvas.transform).GetComponent<UIWindow>();

            return element.Instance;
        }

        /// <summary>
        /// Unload a window from memory by destroying gameObject
        /// </summary>
        /// <param name="windowToUnloadElement">UIWindowListElement instance</param>
        public static void Unload(UIWindowListElement windowToUnloadElement)
        {
            UIWindow windowToUnload = windowToUnloadElement.Instance;
            
            if (windowToUnload != null)
            {
                Object.Destroy(windowToUnload.gameObject);
            }
        }
    }
}