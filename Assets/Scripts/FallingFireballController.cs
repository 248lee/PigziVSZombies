using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingFireballController : FireballController
{
    public float speed = -1.1f, realSpeed;
    private void FixedUpdate()
    {
        this.rid.velocity = new Vector2(0f, this.realSpeed);
    }
    public virtual void Wrong()
    {

    }
    public virtual void Wrong_onFireTree()
    {

    }
    public virtual void Wrong_onFloor()
    {

    }
}
