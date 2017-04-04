using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class SequencerVector3
{
    Sequencer<Vector3> sequencer;

    public Vector3[] objs;

    public virtual Vector3 Evaluate()
    {
        if (sequencer == null)
        {
            sequencer = new Sequencer<Vector3>(objs);
        }

        return sequencer.Evaluate();
    }

    public virtual Vector3 Evaluate(float t)
    {
        if (sequencer == null)
        {
            sequencer = new Sequencer<Vector3>(objs);
        }

        return sequencer.Evaluate(t);
    }
}
