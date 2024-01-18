using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(Wave))]
public class WaveEditor : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {

        var root = new VisualElement();

        root.Add(new PropertyField(property.FindPropertyRelative("attr1")));
        root.Add(new PropertyField(property.FindPropertyRelative("attr2")));
        root.Add(new PropertyField(property.FindPropertyRelative("v_candidates")));

        Button test_button = new Button(() => { OpenJohnEditorWindow(property); });
        test_button.text = "Subwave Editor";
        root.Add(test_button);

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
