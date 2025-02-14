using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GPTProgressUISystem : MonoBehaviour
{
    [SerializeField] Transform contentPlacee;
    [SerializeField] GameObject gptProgressUIprefab;

    List<ProgressCounter> progressCounters;
    List<HPBarController> progressUIs;
    ProgressCounter totalProgressCounter;
    [SerializeField] HPBarController totalProgressUI;
    // Start is called before the first frame update
    void Start()
    {
        this.progressCounters = new List<ProgressCounter>();
        this.progressUIs = new List<HPBarController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.progressCounters.Count != this.progressUIs.Count)
        {
            Debug.LogError("JOHNLEE: The numbers of ProgressCounters and UIs should be the same!");
            return;
        }

        // Update the UIs for each progress counter
        for (int i = 0; i < this.progressCounters.Count; i++)
        {
            this.progressUIs[i].SetHP(this.progressCounters[i].currentCount);
        }

        // Update the UI for the total progress counter
        if (this.totalProgressCounter != null && this.totalProgressUI != null)
        {
            this.totalProgressUI.SetHP(this.totalProgressCounter.currentCount);
        }
    }
    public void SetupProgressBars(List<ProgressCounter> progressCounters)
    {
        this.progressCounters = progressCounters;
        foreach (ProgressCounter pc in this.progressCounters)
        {
            GameObject uiEntry = Instantiate(this.gptProgressUIprefab, this.contentPlacee);
            uiEntry.GetComponentInChildren<TextMeshProUGUI>().text = pc.name;  // setup the ui text

            // Initialize the progress bar ui.
            HPBarController bar = uiEntry.GetComponentInChildren<HPBarController>();
            bar.SetMaxHP(pc.fullCount);
            this.progressUIs.Add(bar);
        }

        // If total progress bar is needed, also initialize it here.
        if (this.totalProgressUI != null)
        {
            this.totalProgressCounter = new ProgressCounter(progressCounters, 0, "total progress");
            this.totalProgressUI.SetMaxHP(this.totalProgressCounter.fullCount);
        }
    }
}
