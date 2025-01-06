using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTouchFireball : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Fireball")
        {
            collision.GetComponent<FireFireballController>().Wrong_onFloor();
        }
    }
}
