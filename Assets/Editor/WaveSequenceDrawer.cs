using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(WaveSequence), true)]
public class WaveSequenceDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement();
        var listProp = property.FindPropertyRelative("waveContainers"); // the List<WaveContainer> we are drawing

        // We'll drive the ListView with indices, so we don't clone data.
        var indices = new List<int>();

        var listView = new ListView
        {
            selectionType = SelectionType.None,
            showAddRemoveFooter = false, // we¡¦ll make our own footer
            virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
            //fixedItemHeight = 60f,
            showBorder = true
        };

        listView.makeItem = () => new PropertyField();
        // IMPORTANT: always unbind before rebind to avoid stale SerializedProperty references
        listView.unbindItem = (element, i) =>
        {
            var pf = (PropertyField)element;
            pf.Unbind();
        };

        // Bind each row to the array element at index i
        listView.bindItem = (element, i) =>
        {
            var pf = (PropertyField)element;
            

            // Fetch a *fresh* element property each time
            var elemProp = listProp.GetArrayElementAtIndex(i);

            // Ensure clean state then bind
            pf.Unbind();
            pf.BindProperty(elemProp);
        };

        void Refresh()
        {
            // Make sure SerializedObject is in sync before touching properties
            listProp.serializedObject.Update();

            // Use a simple index list as the itemsSource
            var count = listProp.arraySize;
            var indices = new List<int>(count);
            for (int i = 0; i < count; i++) indices.Add(i);

            // Re-assign to force a clean rebuild
            listView.itemsSource = null;
            listView.itemsSource = indices;
            listView.Rebuild();
        }

        // Custom footer with + / - buttons (replacement for listView.footer)
        var footer = new VisualElement();
        footer.style.flexDirection = FlexDirection.Row;
        footer.style.justifyContent = Justify.FlexEnd;
        //footer.style.gap = 4;

        // ADD
        footer.Add(new Button(() =>
        {
            // Create new WaveContainer asset (assumes WaveContainer : ScriptableObject)
            // Build target directory: Assets/_RealGame/WaveSequence/{SceneName}
            var sceneName = SceneManager.GetActiveScene().name;
            sceneName = string.IsNullOrEmpty(sceneName) ? "UnnamedScene" : SanitizeForFileName(sceneName);

            var relDir = $"Assets/_RealGame/WaveSequence/{sceneName}";
            var absDir = Path.Combine(Application.dataPath, "_RealGame/WaveSequence", sceneName);
            Directory.CreateDirectory(absDir); // ensure folder exists

            // Filename: wave_{current length}.asset
            int index = listProp.arraySize;
            string relPath = $"{relDir}/wave_{index}.asset";

            // Ensure uniqueness with (1), (2), ...
            relPath = MakeUniquePathWithParentheses(relPath);

            // Create new asset (assumes WaveContainer : ScriptableObject)
            var newContainer = ScriptableObject.CreateInstance<WaveContainer>();
            AssetDatabase.CreateAsset(newContainer, relPath);
            AssetDatabase.SaveAssets();

            // Append to the serialized list
            listProp.arraySize++;
            var newElem = listProp.GetArrayElementAtIndex(listProp.arraySize - 1);
            newElem.objectReferenceValue = newContainer;
            listProp.serializedObject.ApplyModifiedProperties();

            Refresh();
        })
        { text = "+" });

        // REMOVE (removes the last element, adjust as you wish)
        footer.Add(new Button(() =>
        {
            int last = listProp.arraySize - 1;
            if (last >= 0)
            {
                var elemProp = listProp.GetArrayElementAtIndex(last);
                var toDelete = elemProp.objectReferenceValue as WaveContainer;

                // Clear reference first so Unity doesn't keep a dangling ref
                elemProp.objectReferenceValue = null;
                listProp.DeleteArrayElementAtIndex(last);
                listProp.serializedObject.ApplyModifiedProperties();

                // Delete the asset from disk if it exists
                if (toDelete != null)
                {
                    var assetPath = AssetDatabase.GetAssetPath(toDelete);
                    if (!string.IsNullOrEmpty(assetPath))
                        AssetDatabase.DeleteAsset(assetPath);
                    AssetDatabase.SaveAssets();
                }
                Refresh();
            }
        })
        { text = "-" });

        root.Add(listView);
        root.Add(footer);

        Refresh();
        return root;
    }
    private static string MakeUniquePathWithParentheses(string relPath)
    {
        // relPath like "Assets/.../wave_3.asset"
        if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(relPath) == null)
            return relPath;

        string dir = Path.GetDirectoryName(relPath)?.Replace("\\", "/") ?? "Assets";
        string nameNoExt = Path.GetFileNameWithoutExtension(relPath);
        string ext = Path.GetExtension(relPath);

        int n = 1;
        string candidate;
        do
        {
            candidate = $"{dir}/{nameNoExt} ({n}){ext}";
            n++;
        }
        while (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(candidate) != null);

        return candidate;
    }

    private static string SanitizeForFileName(string s)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            s = s.Replace(c, '_');
        return s;
    }
}
