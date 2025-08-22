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
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement();
        root.Add(new PropertyField(property, "Wave Edit")); // <- one line to draw all child fields
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
