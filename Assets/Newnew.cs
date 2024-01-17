using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Newnew : MonoBehaviour
{
    List<int> a1 = new List<int>();
    List<int> a2 = new List<int>();
    // Start is called before the first frame update
    void Start()
    {
        this.a1.Add(1);
        this.a1.Add(3);
        this.a1.Add(7);
        this.a2 = this.a1;
        this.a2.RemoveAt(0);
        Debug.Log(this.a1[0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


