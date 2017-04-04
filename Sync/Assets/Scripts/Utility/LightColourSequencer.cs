﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LightColourSequencer : MonoBehaviour
{
    [Header("Sequencer")]
    public SequencerGradient sequencer;
    [Header("References")]
    public Light light;
    [Header("Animation")]
    public Gradient gradient;
    public enum Animation { Lerp, FadeTo, FadeFrom };
    new public Animation animation = Animation.FadeTo;
    public Gradient fade;
    private Color start;
    private Color end;

    private bool isInitialised = false;

    void Initialise()
    {
        if (!isInitialised)
        {
            if (((Synchronism)Blackboard.Global[Literals.Strings.Blackboard.Synchronisation.Synchroniser].Value) != null)
            {
                sequencer.Initialise();
                sequencer.callback = Callback;
                Callback();
                isInitialised = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialised)
            Initialise();

        switch (animation)
        {
            case Animation.Lerp:
                light.color = Color.Lerp(start, end, sequencer.Evaluate());
                break;
            case Animation.FadeTo:
                light.color = Color.Lerp(start, end, sequencer.synchroniser.Percent);
                break;
            case Animation.FadeFrom:
                light.color = Color.Lerp(start, end, sequencer.synchroniser.Percent);
                break;
        }
    }

    void Callback()
    {
        float evaluate = sequencer.Evaluate();

        start = end;
        end = gradient.Evaluate(evaluate);

        switch (animation)
        {
            case Animation.Lerp:
                break;
            case Animation.FadeTo:
                start = end;
                end = fade.Evaluate(evaluate);
                break;
            case Animation.FadeFrom:
                start = fade.Evaluate(evaluate);
                break;
        }
    }
}
