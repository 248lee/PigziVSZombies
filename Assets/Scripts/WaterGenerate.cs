using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WaterGenerate : MonoBehaviour
{
    [SerializeField] Transform r1, r2;
    [SerializeField] Color color = Color.cyan;
    public Vector3 dir1, dir2;
    // Start is called before the first frame update
    void Start()
    {
        this.dir1 = r1.position - transform.position;
        this.dir2 = r2.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDrawGizmosSelected()
    {
        Vector3 d1 = r1.position - transform.position;
        Vector3 d2 = r2.position - transform.position;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, d1);
        Gizmos.DrawRay(transform.position, d2);
    }
    
    
}
