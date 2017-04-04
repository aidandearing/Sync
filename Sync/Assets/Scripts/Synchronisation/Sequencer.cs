using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequencer<T>
{
    internal T[] objs;
    int step;

    public Sequencer (T[] objs)
    {
        this.objs = objs;
    }

    public virtual T Evaluate()
    {
        return objs[(step++ % objs.Length)];
    }

    public virtual T Evaluate(float t)
    {
        return objs[(int)(t * (objs.Length - 1.0f))];
    }
}
