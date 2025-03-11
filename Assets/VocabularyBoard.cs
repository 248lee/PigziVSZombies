using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VocabularyBoard : MonoBehaviour
{
    [SerializeField] Transform content;
    [SerializeField] GameObject vocabularyTextPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateVocabularyBoard(List<string> vocabularies)
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
                StartCoroutine(instantiateVocabularyOneByOne(vocabularies));
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
        foreach (string voc in vocabularies)
        {
            yield return new WaitForSeconds(1f);
            Instantiate(this.vocabularyTextPrefab, this.content).GetComponent<TextMeshProUGUI>().text = voc;
        }
    }
}
