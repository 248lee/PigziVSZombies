using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class WaveSystem
{
    public class LoopEndLabelWave : Wave
    {
        public override WaveMode Mode => WaveMode.LoopEndLabel;
        public string labelName = "Unnamed";
        public string targetLabelName = "Unnamed";

        public override void InitializeWave()
        {
            // Loop end label, jump back to the loop start condition
            for (int i = instance.nowWaveIndex; i >= 0; i--)  // scan from this wave all the way to the front
            {
                if (instance.waves[i] is LoopStartConditionWave startConditionWave && startConditionWave.targetLabelName == this.labelName)
                {
                    instance.nowWaveIndex = i - 1;  // this is like setting the program counter in the CPU to the loop label
                    break;
                }
            }
        }
        public override IEnumerator implementWaveProcess()
        {
            yield return null;  // Do nothing.
        }
    }
}
