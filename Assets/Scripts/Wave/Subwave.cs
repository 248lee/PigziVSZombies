using System.Collections;
using UnityEngine;

[System.Serializable]
public class Subwave
{
    public float startDelay;
    public int numOfEmmisions;
    public float durationMin;
    public float durationMax;

    [Tooltip("If all emissions have completed before the healBallDuration is passed, then the healBall will be omitted. Set this to a big number if you don't want a healBall to appear in this subwave.")]
    public float healBallDelay = 999f;
}
