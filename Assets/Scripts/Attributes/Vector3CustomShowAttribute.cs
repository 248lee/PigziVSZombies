using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Vector3CustomShowAttribute : PropertyAttribute
{
    public string bitmask;
    public Vector3CustomShowAttribute(string bitmask)
    {
        this.bitmask = bitmask;
    }
}
