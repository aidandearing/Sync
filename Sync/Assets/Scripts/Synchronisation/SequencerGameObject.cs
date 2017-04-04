using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
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

    public virtual GameObject Evaluate(float t)
    {
        if (sequencer == null)
        {
            sequencer = new Sequencer<GameObject>(objs);
        }

        return sequencer.Evaluate(t);
    }
}
