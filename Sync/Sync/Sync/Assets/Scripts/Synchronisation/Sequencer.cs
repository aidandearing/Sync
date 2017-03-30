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
}

[System.Serializable]
[AddComponentMenu("Scripts/Synchronisation/Sequencer")]
public class SequencerGameObjects
{
    Sequencer<GameObject> sequencer;

    public GameObject[] objs;

    public virtual GameObject Evaluate()
    {
        if (sequencer == null)
        {
            sequencer = new Sequencer<GameObject>(objs);
        }

        return sequencer.Evaluate();
    }
}
