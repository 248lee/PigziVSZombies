using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalHome : MonoBehaviour
{
    public int index;

    [Vector3CustomShow("100")]
    public Vector3 leftPosition;
    [Vector3CustomShow("100")]
    public Vector3 rightPosition;

    private bool leftOccupied = false;
    private bool rightOccupied = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        // Set the gizmo color
        Gizmos.color = Color.green;

        // Get the current position of the object (transform.position is a Vector3)
        Vector3 origin = transform.position;

        // Optionally, draw a small sphere at the end of the vector
        Gizmos.DrawSphere(origin + this.leftPosition, 0.1f); // Radius of the sphere can be adjusted
        Gizmos.DrawSphere(origin + this.rightPosition, 0.1f); // Radius of the sphere can be adjusted
    }
}
