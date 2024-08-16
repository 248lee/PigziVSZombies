using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
[CustomPropertyDrawer(typeof(Wave))]

public class WaveEditor : PropertyDrawer
{
    void ModeChanged(SerializedPropertyChangeEvent w)
    {
        
}
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement();

        SerializedProperty modeProperty = property.FindPropertyRelative("mode");  // get the property of the wavemode
        PropertyField modeField = new PropertyField(modeProperty);  // generate a property-field for the gotton property
        root.Add(modeField);  // add the generated property-field onto the tree

        VisualElement container1 = new VisualElement();
        //container1.Clear();
        //container1.Add(new PropertyField(property.FindPropertyRelative("v_candidates")));
        //Button subwaveWindowButton = new Button(() => { OpenJohnEditorWindow(property); });
        //subwaveWindowButton.text = "Subwave Editor";
        //container1.Add(subwaveWindowButton);

        modeField.RegisterValueChangeCallback((SerializedPropertyChangeEvent w) => {  // The callback is called when wavemode is changed
            WaveMode newMode = (WaveMode)w.changedProperty.enumValueIndex;  // get the real value of the wavemode property
            if (newMode == WaveMode.Normal || newMode == WaveMode.Boss)
            {
                container1.Clear();
                PropertyField v_candidatesField = new PropertyField();
                // 此處unity有嚴重bug,如果你使用 PropertyField v_candidatesField = new PropertyField(property.FindPropertyRelative("v_candidates"));
                v_candidatesField.BindProperty(property.FindPropertyRelative("v_candidates"));
                Button subwaveWindowButton = new Button(() => { OpenJohnEditorWindow(property); });
                subwaveWindowButton.text = "Subwave Editor";
                container1.Add(subwaveWindowButton);
                container1.Add(v_candidatesField);
            }
            else
            {
                container1.Clear();
                PropertyField lableNameField = new PropertyField();
                lableNameField.BindProperty(property.FindPropertyRelative("labelName"));
                container1.Add(lableNameField);
            }
        });
        root.Add(container1);

        VisualElement testLabel = new Label();
        StyleSheet test_uss = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/Uss/Test.uss");
        testLabel.styleSheets.Add(test_uss);
        testLabel.AddToClassList("johntest");
        root.Add(testLabel);

        //this.thiswave = property.serializedObject;
        return root;
    }

    private void OpenJohnEditorWindow(SerializedProperty thiswave)
    {
        JohnTestEditor jte = ScriptableObject.CreateInstance<JohnTestEditor>();
        jte.SetSubwave_serializedObject(thiswave);
        jte.Show();
    }
}
