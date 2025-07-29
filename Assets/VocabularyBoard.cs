using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JohnUtils;

public class VocabularyBoard : MonoBehaviour
{
    public static VocabularyBoard instance;
    [SerializeField] Transform content;
    [SerializeField] GameObject vocabularyTextPrefab;

    public event EventHandlerWithList<GameObject> OnVocabularyBoardUpdated;

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
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator UpdateVocabularyBoard(List<string> vocabularies)
    {
        if (this.content != null)
        {
            // Clean up all the old vocabulary texts
            foreach (Transform child in this.content)
            {
                Destroy(child.gameObject);
            }

            // Instantiate new vocabulary texts
            if (this.vocabularyTextPrefab != null)
            {
                yield return StartCoroutine(instantiateVocabularyOneByOne(vocabularies));
            }
            else
            {
                Debug.LogError("The vocabularyTextPrefab is not assigned.");
            }
        }
        else
        {
            Debug.LogError("Content transform is not assigned.");
        }
    }
    IEnumerator instantiateVocabularyOneByOne(List<string> vocabularies)
    {
        List<GameObject> instantiatedObjects = new List<GameObject>();
        foreach (string voc in vocabularies)
        {
            yield return new WaitForSeconds(1f);
            GameObject vocabTextObj = Instantiate(this.vocabularyTextPrefab, this.content);
            vocabTextObj.GetComponentInChildren<TextMeshProUGUI>().text = voc;
            instantiatedObjects.Add(vocabTextObj);
        }
        // Notify subscribers that the vocabulary board has been updated
        OnVocabularyBoardUpdated?.Invoke(instantiatedObjects);
    }
}
