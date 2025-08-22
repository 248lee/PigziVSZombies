using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


[CustomPropertyDrawer(typeof(WaveContainer))]
public class WaveContainerReferenceDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create a container with a red border
        var framedContainer = new VisualElement();
        

        // Draw the reference field (so you can assign the asset)
        framedContainer.Add(new PropertyField(property, "WaveContainer Reference"));

        // If the reference is assigned, draw its inspector
        if (property.objectReferenceValue is WaveContainer container)
        {
            // Create a temporary editor for the referenced asset
            Editor editor = Editor.CreateEditor(container);
            if (editor is WaveContainerEditor waveEditor)
            {
                // Use your custom inspector UI
                framedContainer.Add(waveEditor.CreateInspectorGUI());
            }
            else
            {
                // Fallback: draw default inspector
                IMGUIContainer defaultInspector = new IMGUIContainer(() => editor.OnInspectorGUI());
                framedContainer.Add(defaultInspector);
            }
        }

        return framedContainer;
    }

    
}