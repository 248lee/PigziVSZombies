using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

//[CustomEditor(typeof(WaveSystem))]
public class WaveSystemEditor : Editor
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;


   /* public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();

        var mcListView = root.Q<MultiColumnListView>();
        WaveSystem waveSystem = target as WaveSystem;
        //mcListView.itemsSource = waveSystem.waves;


        m_VisualTreeAsset.CloneTree(root);
        return root;
    }*/
   
}
