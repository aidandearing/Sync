using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class MonolithSequence : MonoBehaviour
{
    public bool overLife;
    protected float lastPercent;

    public abstract void End();
}
