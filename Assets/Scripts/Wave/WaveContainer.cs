using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class WaveContainer : ScriptableObject
{
    // One slot per concrete Wave type.
    // SerializeReference lets Unity keep the *actual* subclass instance.
    [SerializeReference] public List<Wave> waveTypes = new();

    public IReadOnlyList<Wave> WaveTypes => waveTypes;
    [SerializeReference] public Wave currentSelectedWave;

    /// <summary>Get the instance for a specific Wave subtype.</summary>
    public T Get<T>() where T : Wave => waveTypes.OfType<T>().FirstOrDefault();

#if UNITY_EDITOR
    // Keep the list in sync whenever the object changes in the editor.
    private void OnValidate() => EnsureAllSubclassesPresent();
    private void Reset() => EnsureAllSubclassesPresent();

    private void EnsureAllSubclassesPresent()
    {
        // Find all non-abstract, parameterless-constructible subclasses of Wave.
        var allConcrete = TypeCache.GetTypesDerivedFrom<Wave>()
            .Where(t => !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null);

        // Track what we already have.
        var have = new HashSet<Type>(waveTypes.Where(w => w != null).Select(w => w.GetType()));

        bool changed = false;
        foreach (var t in allConcrete)
        {
            if (have.Contains(t)) continue;
            waveTypes.Add((Wave)Activator.CreateInstance(t));
            changed = true;
        }
        // print the count of the waveTypes list if it changed
        Debug.Log($"WaveContainer: waveTypes count = {waveTypes.Count}");

    }
#endif
}