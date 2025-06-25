using System.Collections;
using UnityEngine;

[System.Serializable]
public class Subwave
{
    public float startDelay;
    public int numOfEmmisions;
    public float durationMin;
    public float durationMax;

    [Tooltip("To disable the healball, set this to -1.")]
    public float healBallDelay = 999f;
}
