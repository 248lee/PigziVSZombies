using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(Vector3CustomShowAttribute))]
public class Vector3CustomEditor : PropertyDrawer
{
    const string Vector3NamePaths = "xyz";
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement root = new VisualElement();
        root.Add(new ToolbarSpacer());
        Label name_of_vector3 = new Label(property.name);
        root.Add(name_of_vector3);

        Vector3CustomShowAttribute target_attribute = attribute as Vector3CustomShowAttribute;

        for (int i = 0; i < 3; i++)
        {
            if (target_attribute.bitmask[i] == '1')
            {
                PropertyField vector3field = new PropertyField();
                vector3field.BindProperty(property.FindPropertyRelative(Vector3NamePaths[i].ToString()));
                vector3field.style.paddingLeft = 15;
                root.Add(vector3field);
            }
        }
        
        return root;
    }
}
