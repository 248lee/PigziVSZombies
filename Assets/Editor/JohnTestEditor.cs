using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


public class JohnTestEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private PropertyField subwavelist;
    [MenuItem("Window/UI Toolkit/JohnTestEditor")]
    public static void ShowExample()
    {
        JohnTestEditor wnd = GetWindow<JohnTestEditor>();
        wnd.titleContent = new GUIContent("Subwave Editor");
    }
    private void OnEnable()
    {
        this.subwavelist = new PropertyField();
    }
    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        /*Label testlabel = new Label();
        testlabel.bindingPath = "nowWave";
        WaveSystem selectedObject = FindObjectOfType<WaveSystem>();
        Debug.Log(selectedObject.nowWave);
        SerializedObject so = new SerializedObject(selectedObject);
        testlabel.Bind(so);
        root.Add(testlabel);*/

        root.Add(subwavelist);

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        /*VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);*/
    }

    public void SetSubwave_serializedObject(SerializedProperty wave)
    {
        this.subwavelist.BindProperty(wave.FindPropertyRelative("subwaves"));
    }
}
