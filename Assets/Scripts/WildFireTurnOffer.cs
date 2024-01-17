using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WildFireTurnOffer : MonoBehaviour
{
    [SerializeField] ParticleSystem p1, p2, p3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TurnOffWildFire()
    {
        var em1 = this.p1.emission;
        var em2 = this.p2.emission;
        var em3 = this.p3.emission;
        em1.enabled = false;
        em2.enabled = false;
        em3.enabled = false;
    }
}
