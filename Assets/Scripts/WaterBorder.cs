using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WaterBorder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Waterball")
        {
            //collision.GetComponent<WaterController>().destroyMe();
        }
    }
}
