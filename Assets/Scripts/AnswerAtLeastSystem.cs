using JohnUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerAtLeastSystem : MonoBehaviour
{
    public static AnswerAtLeastSystem instance;
    public event EventHandlerWithVoid OnAnswerAtLeastUpdated;
    [SerializeField] private List<AnswerAtLeastController> answerAtLeastControllers;
    public ProgressCounter totalAnsweredCounter { get; private set; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        this.answerAtLeastControllers = new List<AnswerAtLeastController>();
    }
    private void Start()
    {
        VocabularyBoard.instance.OnVocabularyBoardUpdated += this.SetVocabularyWordsUIs;
    }
    private void SetVocabularyWordsUIs(List<GameObject> vocabularyWordsUIs)
    {
        List<ProgressCounter> progressCounters = new List<ProgressCounter>();

        this.answerAtLeastControllers.Clear();  // Clear previous controllers
        foreach (GameObject ui in vocabularyWordsUIs)
        {
            AnswerAtLeastController controller = ui.GetComponent<AnswerAtLeastController>();
            controller.Setup(ui.GetComponentInChildren<TMPro.TextMeshProUGUI>().text, 2); // Assuming max times is 2 for each vocabulary
            if (controller != null)
            {
                this.answerAtLeastControllers.Add(controller);
                progressCounters.Add(controller.answeredCounter);
            }
            else
            {
                Debug.LogError("The GameObject does not have an AnswerAtLeastController component.");
            }
        }

        this.totalAnsweredCounter = new ProgressCounter(progressCounters, 0, "Answer at least system: whole progress");  // Re-initialize the total answered counter
        VocabularyBoard.instance.OnVocabularyBoardUpdated -= this.SetVocabularyWordsUIs; // Unsubscribe to avoid multiple setups
    }
    public void OneVocabularyAnswered(string vocabulary)
    {
        foreach (AnswerAtLeastController controller in this.answerAtLeastControllers)
        {
            if (controller.vocabulary == vocabulary)
            {
                bool is_effective = controller.Answered();  // This automatically counts up the total answered counter, if effective
                if (is_effective)
                    this.OnAnswerAtLeastUpdated?.Invoke(); // Notify subscribers that the answer at least system has been updated

                Debug.Log($"Vocabulary '{vocabulary}' answered. Current count: {this.totalAnsweredCounter.currentCount}/{this.totalAnsweredCounter.fullCount}");
                return;
            }
        }
        Debug.LogError($"No AnswerAtLeastController found for vocabulary: {vocabulary}");
    }
}
