using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[AddComponentMenu("Scripts/Synchronisation/Special/Monolith Synchronism")]
public class MonolithSpecialSynchronism : Synchronism
{
    public MusicPlayer musicPlayer;

    [Header("References")]
    public Material materialEye;

    [Header("Delay")]
    public Synchronisations delaySynchronisation = Synchronisations.BAR_8;
    public Synchroniser delaySynchroniser;
    public int delayBy = 1;
    public int delayCurrent = 0;

    [Header("Delay Specifics")]
    public AnimationCurve delaySpotlightIntensity;
    public Light delaySpotlight;
    public GameObject delaySequence;
    public GameObject delayEndSequence;

    [Header("Sequencing")]
    public Synchronisations sequencerSynchronisation = Synchronisations.BAR_8;
    public Synchroniser sequencerSynchroniser;
    public SequencerGameObjects sequencerObjects;

    protected override void Start()
    {
        Initialise();

        if (delaySynchronisation == sequencerSynchronisation)
        {
            sequencerSynchroniser = delaySynchroniser = synchronisers[delaySynchronisation];
            delaySynchroniser.RegisterCallback(this, CallbackBoth);
        }
        else
        {
            delaySynchroniser = synchronisers[delaySynchronisation];
            delaySynchroniser.RegisterCallback(this, CallbackDelay);

            sequencerSynchroniser = synchronisers[sequencerSynchronisation];
            sequencerSynchroniser.RegisterCallback(this, CallbackBoth);
        }

        Instantiate(delaySequence, transform, false);
    }

    protected override void Update()
    {
        base.Update();

        if (delayCurrent < delayBy)
            delaySpotlight.intensity = delaySpotlightIntensity.Evaluate((delaySynchroniser.Percent + delayCurrent) / delayBy);

        materialEye.SetColor("_EmissionColor", delaySpotlight.color * delaySpotlight.intensity);
    }

    private void CallbackBoth()
    {
        CallbackDelay();
        CallbackSequencer();
    }

    private void CallbackDelay()
    {
        if (delayCurrent < delayBy)
        {
            delayCurrent++;
        }

        if (delayCurrent == delayBy)
        {
            delaySpotlight.intensity = delaySpotlightIntensity.Evaluate(1);
            Instantiate(delayEndSequence, transform, false);
            delayCurrent++;
        }
    }

    private void CallbackSequencer()
    {
        if (delayCurrent >= delayBy)
            Instantiate(sequencerObjects.Evaluate());
    }
}
