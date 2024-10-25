using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TreeController))]
public class AnimalHome : MonoBehaviour
{
    public int index;

    [Vector3CustomShow("100")]
    public Vector3 leftPositionOffset;
    [Vector3CustomShow("100")]
    public Vector3 rightPositionOffset;
    public Vector3 leftPosition
    {
        get => transform.position + this.leftPositionOffset;
    }
    public Vector3 rightPosition
    {
        get => transform.position + this.rightPositionOffset;
    }

    public bool leftOccupied { get; set; } = false;
    public bool rightOccupied{ get; set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool isTreeAlive
    {
        get => GetComponent<TreeController>().hp > 0;
    }
    private void OnDrawGizmos()
    {
        // Set the gizmo color
        Gizmos.color = Color.green;

        // Optionally, draw a small sphere at the end of the vector
        Gizmos.DrawSphere(this.leftPosition, 0.1f); // Radius of the sphere can be adjusted
        Gizmos.DrawSphere(this.rightPosition, 0.1f); // Radius of the sphere can be adjusted
    }
}
