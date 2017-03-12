using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
[AddComponentMenu("Scripts/UI/Raw Image Colour Sequencer")]
public class RawImageColourSequencer : MonoBehaviour
{
    [Header("Sequencer")]
    public SequencerGradient sequencer;
    [Header("References")]
    public RawImage image;
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
            if (((Synchronism)Blackboard.Global[Literals.StringLiterals.Blackboard.Synchroniser].Value) != null)
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

        image.color = Color.Lerp(start, end, sequencer.Evaluate());
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
