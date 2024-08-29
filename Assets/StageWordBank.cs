using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JohnUtils;

public class StageWordBank : MonoBehaviour
{
    public List<string> regularWords = new();
    public Dictionary<string, List<string>> waveSpecifiedWords = new();
    private Dictionary<string, Queue<string>> sentences;
    private Dictionary<int, Queue<Paragraph>> paragraphs;
    private Dictionary<string, Paragraph> waveSpecifiedParagraphs;
    // Start is called before the first frame update
    void Start()
    {
        this.sentences = new Dictionary<string, Queue<string>>();
        this.paragraphs = new Dictionary<int, Queue<Paragraph>>();
        this.waveSpecifiedParagraphs = new Dictionary<string, Paragraph>();
    }

    public void WordsOutgive(Wave wave)
    {
        if (wave.waveName == "")  // if the wave's a regularWave
        {
            if (wave.numOfVocabularies < regularWords.Count)
                Debug.LogError("JohnLee: We expect that you should have more words in this stage. This doesn't even enough for a wave!!");

            // Randomly select words
            if (regularWords.Count < wave.numOfVocabularies * 25)  // if the number of words of this stage is less, we use O(num_of_stage_words) method
            {
                List<string> copy_of_regularWords = new List<string>(this.regularWords);
                IListExtensions.Shuffle<string>(copy_of_regularWords);
                wave.v_candidates = new List<string>();
                for (int i = 0; i < wave.numOfVocabularies; i++)  // append the words into wave.v_candidates
                    wave.v_candidates.Add(copy_of_regularWords[i]);
            }
            else  // if the number of words is huge, then we use O(wave.numOfVocabularies^2) method
            {
                wave.v_candidates = new List<string>();
                for (int i = 0; i < wave.numOfVocabularies; i++)
                {
                    int random_index;
                    bool isRepeated = false;
                    do
                    {
                        random_index = UnityEngine.Random.Range(0, this.regularWords.Count);
                        foreach (string selected_vocabulary in wave.v_candidates)
                        {
                            if (selected_vocabulary == this.regularWords[random_index])
                            {
                                isRepeated = true;
                                break;
                            }
                        }
                    } while (isRepeated);  // keep randomly pick a index until no repeat
                    wave.v_candidates.Add(this.regularWords[random_index]);
                }
            }
        }
        else  // if it is a specified wave
        {

        }
    }

}
