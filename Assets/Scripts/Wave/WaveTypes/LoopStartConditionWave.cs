using JohnUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class WaveSystem
{
    [System.Serializable]
    public class LoopStartConditionWave : Wave
    {
        public override WaveMode Mode => WaveMode.LoopStartCondition;
        public string labelName = "Unnamed";
        public string runtimeVariableA = "UnknownGlobalVariable";
        public Relation relation;
        public string runtimeVariableB = "UnknownGlobalVariable";
        public string targetLabelName = "Unnamed";

        public override void InitializeWave()
        {
            bool is_goInLoop = false;
            // Check loop condition
            float variableA, variableB;
            if (!float.TryParse(this.runtimeVariableA, out variableA))  // if the string is in the form of float, just convert it to float
            {
                variableA = RuntimeGlobalDictionary.GetVariableFloat(this.runtimeVariableA);  // else, get the actual value from runtime global dictionary
            }
            if (!float.TryParse(this.runtimeVariableB, out variableB))  // if the string is in the form of float, just convert it to float
            {
                variableB = RuntimeGlobalDictionary.GetVariableFloat(this.runtimeVariableB);  // else, get the actual value from runtime global dictionary
            }
            switch (this.relation)
            {
                case Relation.greater:
                    is_goInLoop = variableA > variableB;
                    break;
                case Relation.geq:
                    is_goInLoop = variableA >= variableB;
                    break;
                case Relation.less:
                    is_goInLoop = variableA < variableB;
                    break;
                case Relation.leq:
                    is_goInLoop = variableA <= variableB;
                    break;
                case Relation.equal:
                    is_goInLoop = variableA == variableB;
                    break;
                default:
                    break;
            }
            if (!is_goInLoop)  // skip and jump to the end if the condition is not satisfied
            {
                for (int i = 0; i < instance.waves.Count; i++)  // scan from this wave all the way to the front
                {
                    if (instance.waves[i] is LoopEndLabelWave endLabelWave && endLabelWave.labelName == this.targetLabelName)
                    {
                        instance.nowWaveIndex = i;  // this is like setting the program counter in the CPU to the end label
                        break;
                    }
                }
            }
        }
        public override IEnumerator implementWaveProcess()
        {
            yield return null;  // Do nothing.
        }
    }
}
