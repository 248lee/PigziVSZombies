using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class WaveSystem
{
    [System.Serializable]
    public class BossWave : NormalWave
    {
        public override WaveMode Mode => WaveMode.Boss;
        public int p_numOfVocabularies = 0;
        public DragonController.DragonWaveData dragonData;
        public Paragraph dragon_paragraph = null;
        public override void WordsOutgive()
        {
            instance.wordBankOfThisStage.ParagraphAndWordsOutgive(this);
        }

        /* We do not override the InitializeWave() here! */
        //public override void InitializeWave()
        //{

        //}
        public override IEnumerator implementWaveProcess()
        {
            if (this.numOfVocabularies > 0)
                yield return VocabularyBoard.instance.UpdateVocabularyBoard(this.v_candidates);  // This plays the animation of setting up the vocabulary board
            instance.dragon.Born(this);
            // Call the base class implementation to handle subwaves
            yield return instance.StartCoroutine(base.IterateThroughSubwaves());
            while (instance.dragon.is_on_stage == true)  // wait until the dragon flies away
            {
                yield return null;
            }
            yield return new WaitForSeconds(3f);  // After the wave ends, rest for a while~
            instance.nowWaveIndexForPlayer++;
        }
    }
}
