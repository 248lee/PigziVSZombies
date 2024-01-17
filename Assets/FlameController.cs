using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameController : MonoBehaviour
{
    DragonController dragon;
    // Start is called before the first frame update
    void Start()
    {
        this.dragon = GetComponentInParent<DragonController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetWildFire(Transform posTrans)
    {
        this.dragon.dropFireballs(posTrans);
    }
    public void switchFire(int st)
    {
        if(st > 0)
            this.dragon.switchFlameParticle(true);
        else
            this.dragon.switchFlameParticle(false);
    }
}
