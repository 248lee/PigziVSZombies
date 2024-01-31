using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DeleteMe : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI paragraphText;
    [SerializeField] GameObject testball;
    // Start is called before the first frame update
    void Start()
    {
        TMP_TextInfo textInfo = this.paragraphText.textInfo;
        TMP_CharacterInfo charInfo = textInfo.characterInfo[10];
        Vector3 charPosition = (charInfo.topRight + charInfo.bottomRight) * 0.5f;
        Vector3 worldPosition = this.paragraphText.transform.TransformPoint(charPosition);
        Instantiate(this.testball, worldPosition, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
