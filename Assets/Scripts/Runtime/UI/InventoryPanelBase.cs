using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JZQ.InventorySystem.Runtime.UI
{
    /// <summary>
    /// Provides shared UI utility methods for inventory panels.
    /// </summary>
    public abstract class InventoryPanelBase : MonoBehaviour
    {
        private readonly Dictionary<string, UIBehaviour> components = new();

        /// <summary>
        /// Shows the panel and starts any required runtime bindings.
        /// </summary>
        public abstract void Show();

        /// <summary>
        /// Hides the panel and releases any runtime bindings.
        /// </summary>
        public abstract void Hide();

        /// <summary>
        /// Gets whether the panel game object is currently active.
        /// </summary>
        public bool IsActive => gameObject.activeSelf;

        /// <summary>
        /// Finds and caches a named UI component within the panel hierarchy.
        /// </summary>
        /// <typeparam name="T">The UI component type.</typeparam>
        /// <param name="componentName">The child object name to search for.</param>
        /// <returns>The matching UI component, or <c>null</c> if no match is found.</returns>
        public T GetUIComponent<T>(string componentName) where T : UIBehaviour
        {
            if (components.ContainsKey(componentName))
            {
                T comp = components[componentName] as T;
                if (comp == null)
                {
                    Debug.LogError("[Inventory] Component " + componentName + " is not a " + typeof(T).Name);
                }
                return comp;
            }

            List<GameObject> children = new();
            FindChildrenObject(transform, componentName, children);
            if (children.Count <= 0)
            {
                Debug.LogError("[Inventory] No component named " + componentName + " in " + gameObject.name);
                return null;
            }

            if (children.Count > 1)
            {
                Debug.LogWarning($"[Inventory] Multiple children named {componentName} in {gameObject.name}], result may be unpredictable");
                foreach (var child in children)
                {
                    T comp = child.GetComponent<T>();
                    if (comp != null)
                    {
                        components.Add(componentName, comp);
                        return comp;
                    }
                }
            }
            else
            {
                T comp = children[0].GetComponent<T>();
                if (comp != null)
                {
                    components.Add(componentName, comp);
                    return comp;
                }
            }

            Debug.LogError($"[Inventory] No child named {componentName} in {gameObject.name} which has component {typeof(T).Name}");
            return null;
        }

        private void FindChildrenObject(Transform current, string targetName, List<GameObject> result)
        {
            foreach (Transform child in current)
            {
                if (child.name == targetName)
                {
                    result.Add(child.gameObject);
                }
                FindChildrenObject(child, targetName, result);
            }
        }
    }
}
