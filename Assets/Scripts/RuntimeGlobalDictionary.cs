using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JohnUtils
{
public class RuntimeGlobalDictionary
{
    private class KeyValuePair<T>
    {
        public string key;
        public T value;
        public bool isRefreshed = true;
        public KeyValuePair(string _key, T _value, bool _isRefreshed)
        {
            this.key = _key;
            this.value = _value;
            this.isRefreshed = _isRefreshed;
        }
    }
    private static List<KeyValuePair<float> > realDicInteger = new();
    private static List<KeyValuePair<float> > bufferDicInteger = new();
    
    public static void CopyBufferToReal()  // This MUST be called in any LateUpdate()!
    {
        // Copy the existing variables from buffer to real
        RuntimeGlobalDictionary.realDicInteger = RuntimeGlobalDictionary.bufferDicInteger.ConvertAll(kvp => new KeyValuePair<float>(kvp.key, kvp.value, kvp.isRefreshed));

        // Clear out the buffer to avoid dirty values
        for (int i = 0; i < RuntimeGlobalDictionary.bufferDicInteger.Count; i++)
        {
            RuntimeGlobalDictionary.bufferDicInteger[i].isRefreshed = false;
        }
    }
    public static void SetVariable(string key, float value)
    {
        for (int i = 0; i < RuntimeGlobalDictionary.bufferDicInteger.Count; i++)
        {
            if (RuntimeGlobalDictionary.bufferDicInteger[i].key == key)
            {
                RuntimeGlobalDictionary.bufferDicInteger[i].value = value;
                RuntimeGlobalDictionary.bufferDicInteger[i].isRefreshed = true;
                return;
            }
        }
        // If this is a new key
        RuntimeGlobalDictionary.bufferDicInteger.Add(new KeyValuePair<float>(key, value, true));  // this sets isRefreshed to true
    }
    public static float GetVariableFloat(string key)
    {
        for (int i = 0; i < RuntimeGlobalDictionary.realDicInteger.Count; i++)
        {
            if (RuntimeGlobalDictionary.realDicInteger[i].key == key)
            {
                if (RuntimeGlobalDictionary.realDicInteger[i].isRefreshed)
                {
                    return RuntimeGlobalDictionary.realDicInteger[i].value;
                }
                else
                {
                    Debug.LogError("The value of \"" + key + "\" you've just accessed is already dirty. Have you refreshed it every frame?");
                    return float.MaxValue;
                }
            }
        }
        Debug.LogError("You've never added the variable \"" + key + "\"! Please add and refresh the variable before you can get access to it!");
        return float.MaxValue;
    }
}
}