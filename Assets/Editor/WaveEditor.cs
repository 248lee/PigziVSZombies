using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
[CustomPropertyDrawer(typeof(Wave), true)]

public class WaveEditor : PropertyDrawer
{
    public static Color HashStringToColor(string input)
    {
        // Step 1: Compute the hash of the input string using MD5
        using (MD5 md5 = MD5.Create())
        {
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Step 2: Extract the first 3 bytes from the hash to use as RGB values
            byte r = hashBytes[0];
            byte g = hashBytes[1];
            byte b = hashBytes[2];

            // Step 3: Normalize the RGB values to the range [0, 1] for Unity's Color structure
            float rFloat = r / 255f;
            float gFloat = g / 255f;
            float bFloat = b / 255f;

            // Step 4: Return the resulting color
            return new Color(rFloat, gFloat, bFloat);
        }
    }
    // 所有可用的 Wave 子類型
    private static readonly Type[] waveTypes = new Type[]
    {
        typeof(WaveSystem.NormalWave),
        typeof(WaveSystem.BossWave),
        typeof(WaveSystem.LoopStartConditionWave),
        typeof(WaveSystem.LoopEndLabelWave)
    };
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement();

        // This provides a dropdown menu to let the designer select a mode.
        // 取得目前的子類型
        string typeName = property.managedReferenceFullTypename;
        int currentTypeIndex = Array.FindIndex(waveTypes, t => typeName.Contains(t.Name));

        // 型別選單
        var typePopup = new PopupField<string>(
            "Wave Type",
            new List<string> { "Normal", "Boss", "LoopStartCondition", "LoopEndLabel" },
            currentTypeIndex >= 0 ? currentTypeIndex : 0
        );
        root.Add(typePopup);

        // 切換型別時，替換物件
        typePopup.RegisterValueChangedCallback(evt =>
        {
            int selectedIndex = typePopup.index;
            Type selectedType = waveTypes[selectedIndex];
            property.managedReferenceValue = Activator.CreateInstance(selectedType);
            property.serializedObject.ApplyModifiedProperties();
        });

        VisualElement container1 = new VisualElement();
        VisualElement cutLine = new Label();
        //modeField.RegisterValueChangeCallback((SerializedPropertyChangeEvent w) => {  // The callback is called when wavemode is changed
            //WaveMode newMode = (WaveMode)w.changedProperty.enumValueIndex;  // get the real value of the wavemode property
            if (typeName.Contains("NormalWave") || typeName.Contains("BossWave"))
            {
                container1.Clear();
                // PropertyField v_candidatesField = new PropertyField();
                // 此處unity有嚴重bug,如果你使用 PropertyField v_candidatesField = new PropertyField(property.FindPropertyRelative("v_candidates"));
                // v_candidatesField.BindProperty(property.FindPropertyRelative("v_candidates"));

                PropertyField waveNameField = new PropertyField();
                waveNameField.BindProperty(property.FindPropertyRelative("waveName"));

                PropertyField numOfVocabulariesField = new PropertyField();
                numOfVocabulariesField.BindProperty(property.FindPropertyRelative("numOfVocabularies"));
                numOfVocabulariesField.tooltip = "If you want to use the vocabulary set of previous wave, set it to -1.";

                PropertyField backgroundField = new PropertyField();
                backgroundField.BindProperty(property.FindPropertyRelative("background"));

                Button subwaveWindowButton = new Button(() => { OpenJohnEditorWindow(property); });
                subwaveWindowButton.text = "Subwave Editor";
                container1.Add(subwaveWindowButton);
                container1.Add(waveNameField);
                container1.Add(backgroundField);
                container1.Add(numOfVocabulariesField);
                // container1.Add(v_candidatesField);

                // Boss: field of p_numOfVocabularies
                if (typeName.Contains("BossWave"))
                {
                    PropertyField p_numOfVocabulariesField = new PropertyField();
                    p_numOfVocabulariesField.BindProperty(property.FindPropertyRelative("p_numOfVocabularies"));
                    container1.Add(p_numOfVocabulariesField);

                    PropertyField dragonDataField = new PropertyField();
                    dragonDataField.BindProperty(property.FindPropertyRelative("dragonData"));
                    container1.Add(dragonDataField);
                }

                // Setup Red Cutline
                StyleSheet cutLine_uss = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/Uss/Test.uss");
                cutLine.styleSheets.Add(cutLine_uss);
                cutLine.AddToClassList("originalCutLine");
                cutLine.style.backgroundColor = Color.red;
                container1.Add(cutLine);
            }
            else if (typeName.Contains("LoopStartConditionWave"))
            {
                container1.Clear();
                PropertyField labelNameField = new PropertyField();
                PropertyField runtimeVariableAField = new PropertyField();
                PropertyField relationField = new PropertyField();
                PropertyField runtimeVariableBField = new PropertyField();
                labelNameField.BindProperty(property.FindPropertyRelative("labelName"));
                runtimeVariableAField.BindProperty(property.FindPropertyRelative("runtimeVariableA"));
                relationField.BindProperty(property.FindPropertyRelative("relation"));
                runtimeVariableBField.BindProperty(property.FindPropertyRelative("runtimeVariableB"));
                container1.Add(labelNameField);
                container1.Add(runtimeVariableAField);
                container1.Add(relationField);
                container1.Add(runtimeVariableBField);

                labelNameField.RegisterValueChangeCallback((SerializedPropertyChangeEvent lbname) => {
                    cutLine.style.backgroundColor = WaveEditor.HashStringToColor(property.FindPropertyRelative("labelName").stringValue);
                });  // If the value of labelName is changed, change the corresponding cutline color

                // Setup Cutline
                StyleSheet cutLine_uss = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/Uss/Test.uss");
                cutLine.styleSheets.Add(cutLine_uss);
                cutLine.AddToClassList("originalCutLine");
                cutLine.style.backgroundColor = WaveEditor.HashStringToColor(property.FindPropertyRelative("labelName").stringValue);
                container1.Add(cutLine);
            }
            else if (typeName.Contains("LoopEndLabelWave"))
            {
                container1.Clear();
                PropertyField targetLabelNameField = new PropertyField();
                
                targetLabelNameField.BindProperty(property.FindPropertyRelative("targetLabelName"));
                
                container1.Add(targetLabelNameField);
                

                targetLabelNameField.RegisterValueChangeCallback((SerializedPropertyChangeEvent lbname) => {
                    cutLine.style.backgroundColor = WaveEditor.HashStringToColor(property.FindPropertyRelative("targetLabelName").stringValue);
                });  // If the value of targetLabelName is changed, change the corresponding cutline color

                // Setup Cutline
                StyleSheet cutLine_uss = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/Uss/Test.uss");
                cutLine.styleSheets.Add(cutLine_uss);
                cutLine.AddToClassList("originalCutLine");
                cutLine.style.backgroundColor = WaveEditor.HashStringToColor(property.FindPropertyRelative("targetLabelName").stringValue);
                container1.Add(cutLine);
            }
        //});
        root.Add(container1);

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
