using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
[CustomPropertyDrawer(typeof(WaveSystem.NormalWave), true)]

public class NormalWaveEditor : PropertyDrawer
{
    static readonly string[] variable_names_to_bind = { "waveName", "background", "numOfVocabularies"};
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement();

        foreach (string var_name in variable_names_to_bind)
        {
            PropertyField field = new PropertyField();
            field.BindProperty(property.FindPropertyRelative(var_name));
            root.Add(field);
        }

        Button subwaveWindowButton = new Button(() => { OpenJohnEditorWindow(property); });
        subwaveWindowButton.text = "Subwave Editor";
        root.Add(subwaveWindowButton);
        return root;
    }
    private void OpenJohnEditorWindow(SerializedProperty thiswave)
    {
        JohnTestEditor jte = ScriptableObject.CreateInstance<JohnTestEditor>();
        jte.SetSubwave_serializedObject(thiswave);
        jte.Show();
    }
}
