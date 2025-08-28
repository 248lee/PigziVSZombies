using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class WaveSystem
{
    [System.Serializable]
    public class NormalWave : Wave
    {
        [SerializeField] List<Subwave> subwaves = new();
        public override WaveMode Mode => WaveMode.Normal;
        public int numOfVocabularies = 0;

        public override IEnumerator implementWaveProcess()
        {
            if (this.numOfVocabularies > 0)
                yield return VocabularyBoard.instance.UpdateVocabularyBoard(this.v_candidates);  // This plays the animation of setting up the vocabulary board
            yield return this.IterateThroughSubwaves();
            while (FireballSysrem.instance.fire_onScreen.Count != 0) // busy waiting until the fire on screen is empty
            {
                yield return null;
            }
            yield return new WaitForSeconds(3f);  // After the wave ends, rest for a while~
            instance.nowWaveIndexForPlayer++;
        }
        protected IEnumerator IterateThroughSubwaves()
        {
            foreach (Subwave subwave in this.subwaves)
            {
                Coroutine waitingForHealballCoroutine = null;
                if (subwave.healBallDelay >= 0)
                    waitingForHealballCoroutine = instance.StartCoroutine(this.WaitForHealball(subwave.healBallDelay));
                yield return new WaitForSeconds(subwave.startDelay); // Implementing Subwave process here
                for (int i = 0; i < subwave.numOfEmmisions; i++)
                {
                    Debug.Log($"Now emmiting {i}th fireball");
                    Question question = instance.AskForAQuestion(this);
                    FireballSysrem.instance.generateFireball(question);
                    float delayTime = UnityEngine.Random.Range(subwave.durationMin, subwave.durationMax);
                    if (i < subwave.numOfEmmisions - 1)  // If this is not the last fireball, wait for a random time
                        yield return new WaitForSeconds(delayTime);
                }
                
                //if (waitingForHealballCoroutine != null)
                //{
                //    StopCoroutine(waitingForHealballCoroutine);  // If the healball delay is too long, just simply cancel it.
                //    this.healballCountdownUI.SetActive(false);
                //    Debug.LogWarning("The healball delay is too long. It is canceled");
                //}
            }
        }
        public override void InitializeWave()
        {
            this.WordsOutgive();
            // Update the background
            if (this.background != null)
            {
                instance.currentBackground.SetActive(false);  // Deactivate the previous background
                this.background.SetActive(true);  // Activate the new background
                instance.currentBackground = this.background;  // Update the current background reference
            }
        }
        public virtual void WordsOutgive()
        {
            if (this.numOfVocabularies > 0)
                instance.wordBankOfThisStage.WordsOutgive(this);
            else
            {
                if (instance.nowWaveIndex > 0)
                {
                    Wave previous_wave = instance.waves[instance.nowWaveIndex - 1];
                    this.v_candidates = new List<string>(previous_wave.v_candidates);  // Copy the v_candidates of the previous wave to this wave
                }
                else
                {
                    Debug.LogError("If you set numOfVocabularies to -1, you are asking for the vocabulary candidates of the previous wave. However, THIS IS LITERALLY THE FIRST WAVE!");
                }
            }
        }
        public IEnumerator WaitForHealball(float seconds)
        {
            instance.healballCountdownUI.SetActive(true);
            float countdown = seconds;
            while (countdown > 0)
            {
                instance.healballCountdownUI.GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText(((int)countdown + 1).ToString());  // Draw the countdown UI (+1 for graphic delay)
                countdown -= Time.deltaTime;
                yield return null;
            }

            Question[] questions = new Question[4];
            for (int i = 0; i < 4; i++)
                questions[i] = instance.AskForAQuestion(this);

            FireballSysrem.instance.generateFourHealballs(questions);  // Generate Healballs

            // Show the second 0 in the UI for graphic delay
            instance.healballCountdownUI.GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText("0");
            yield return new WaitForSeconds(1f);
            instance.healballCountdownUI.SetActive(false);
        }
    }

}