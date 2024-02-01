using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DeleteMe : MonoBehaviour
{
    [SerializeField] float delta = 0f;
    private Vector3 initPos;
    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = initPos - new Vector3(0f, delta, 0f);
    }
}
