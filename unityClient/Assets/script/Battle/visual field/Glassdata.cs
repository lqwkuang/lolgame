using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class Glassdata
{
    public List<int> x;
    public List<int> z;
    public List<int> ID;
    public Glassdata()
    {
        x = new List<int>();
        z = new List<int>();
        ID = new List<int>();
    }
}

