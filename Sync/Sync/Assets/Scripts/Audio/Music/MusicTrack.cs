using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class MusicTrack
{
    public SequencerGradient sequencer;

    public GameObject parent;

    public AnimationCurve volume;

    public MusicSource[] sources;

    void Initialise()
    {
        if (!sequencer.isInitialised)
        {
            if ((Synchronism)Blackboard.Global[Literals.StringLiterals.Blackboard.Synchroniser].Value != null)
            {
                sequencer.Initialise();
                
                float v = volume.Evaluate(sequencer.Evaluate());

                foreach (MusicSource source in sources)
                {
                    source.SetVolume(v);
                }

                foreach (MusicSource source in sources)
                {
                    source.parent = parent;
                }
            }
        }
    }

    public void Update()
    {
        if (!sequencer.isInitialised)
            Initialise();
        
        float v = volume.Evaluate(sequencer.Evaluate());

        foreach (MusicSource source in sources)
        {
            source.SetVolume(v);
        }
    }
}
