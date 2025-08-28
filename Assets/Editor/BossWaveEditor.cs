using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
[CustomPropertyDrawer(typeof(WaveSystem.BossWave), true)]
public class BossWaveEditor : NormalWaveEditor
{
    static readonly string[] boss_variable_names_to_bind = { "p_numOfVocabularies", "dragonData" };
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = base.CreatePropertyGUI(property);
        foreach (string var_name in boss_variable_names_to_bind)
        {
            PropertyField field = new PropertyField();
            field.BindProperty(property.FindPropertyRelative(var_name));
            root.Add(field);
        }
        return root;

    }
}
