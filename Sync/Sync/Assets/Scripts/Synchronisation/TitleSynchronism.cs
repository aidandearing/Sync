using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[AddComponentMenu("Scripts/Synchronisation/Title Synchronism")]
public class TitleSynchronism : Synchronism
{
    public MusicPlayer musicPlayer;

    protected override void Start()
    {
        Initialise();

        synchronisers[Synchronisations.BAR_8].RegisterCallback(this, OnTimeBar8);
        synchronisers[Synchronisations.BAR_4].RegisterCallback(this, OnTimeBar4);
        synchronisers[Synchronisations.BAR_2].RegisterCallback(this, OnTimeBar2);
        synchronisers[Synchronisations.BAR].RegisterCallback(this, OnTimeBar);
        synchronisers[Synchronisations.WHOLE_NOTE].RegisterCallback(this, OnTimeWhole);
        synchronisers[Synchronisations.HALF_NOTE].RegisterCallback(this, OnTimeHalf);
        synchronisers[Synchronisations.QUARTER_NOTE].RegisterCallback(this, OnTimeQuarter);
        synchronisers[Synchronisations.EIGHTH_NOTE].RegisterCallback(this, OnTimeEighth);
        synchronisers[Synchronisations.SIXTEENTH_NOTE].RegisterCallback(this, OnTimeSixteenth);
        synchronisers[Synchronisations.THIRTYSECOND_NOTE].RegisterCallback(this, OnTimeThirtySecond);
    }

    protected override void Update()
    {
        // Updating all the note waves
        foreach (KeyValuePair<Synchronisations, Synchroniser> synchs in synchronisers)
        {
            synchs.Value.Update();
        }
    }

    protected override void OnTimeBar8()
    {

    }

    protected override void OnTimeBar4()
    {

    }

    protected override void OnTimeBar2()
    {

    }

    protected override void OnTimeBar()
    {

    }

    protected override void OnTimeWhole()
    {

    }

    protected override void OnTimeHalf()
    {

    }

    protected override void OnTimeQuarter()
    {

    }

    protected override void OnTimeEighth()
    {

    }

    protected override void OnTimeSixteenth()
    {

    }

    protected override void OnTimeThirtySecond()
    {

    }
}
