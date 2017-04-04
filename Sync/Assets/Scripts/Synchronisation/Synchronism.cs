using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Synchronisation/Synchronism")]
public class Synchronism : MonoBehaviour
{
    public enum Synchronisations { NONE, WHOLE_NOTE, HALF_NOTE, QUARTER_NOTE, EIGHTH_NOTE, SIXTEENTH_NOTE, THIRTYSECOND_NOTE, BAR, BAR_2, BAR_4, BAR_8 };
    public Dictionary<Synchronisations, Synchroniser> synchronisers = new Dictionary<Synchronisations, Synchroniser>();

    /// <summary>
    /// The Beats Per Minute of the song
    /// </summary>
    public float BPM = 120;
    public float Beats = 4;
    public float Bar = 4;

    protected virtual void Initialise()
    {
        double beatPerBar = Beats;
        double beat = 60.0f / BPM;
        
        synchronisers.Add(Synchronisations.BAR_8, new Synchroniser(beatPerBar * beat * 8));
        synchronisers.Add(Synchronisations.BAR_4, new Synchroniser(beatPerBar * beat * 4));
        synchronisers.Add(Synchronisations.BAR_2, new Synchroniser(beatPerBar * beat * 2));
        synchronisers.Add(Synchronisations.BAR, new Synchroniser(beatPerBar * beat));
        synchronisers.Add(Synchronisations.WHOLE_NOTE, new Synchroniser(beat * 4));
        synchronisers.Add(Synchronisations.HALF_NOTE, new Synchroniser(beat * 2));
        synchronisers.Add(Synchronisations.QUARTER_NOTE, new Synchroniser(beat));
        synchronisers.Add(Synchronisations.EIGHTH_NOTE, new Synchroniser(beat * 0.5));
        synchronisers.Add(Synchronisations.SIXTEENTH_NOTE, new Synchroniser(beat * 0.25));
        synchronisers.Add(Synchronisations.THIRTYSECOND_NOTE, new Synchroniser(beat * 0.125));
        synchronisers.Add(Synchronisations.NONE, new Synchroniser(0));

        if (!Blackboard.Global.ContainsKey(Literals.Strings.Blackboard.Synchronisation.Synchroniser))
            Blackboard.Global.Add(Literals.Strings.Blackboard.Synchronisation.Synchroniser, new BlackboardValue() { Value = this });
        else
            Blackboard.Global[Literals.Strings.Blackboard.Synchronisation.Synchroniser].Value = this;
    }

    protected virtual void Start()
    {
        Initialise();
    }

    protected virtual void Update()
    {
        // Updating all the note waves
        foreach (KeyValuePair<Synchronisations, Synchroniser> synchs in synchronisers)
        {
            synchs.Value.Update();
        }
    }
}

