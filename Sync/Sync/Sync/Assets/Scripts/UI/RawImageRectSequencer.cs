using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Scripts/UI/Raw Image Rectangular Sequencer")]
public class RawImageRectSequencer : MonoBehaviour
{
    [Header("Sequencer")]
    public SequencerGradient sequencer;

    [Header("References")]
    public RawImage image;

    [Header("Automation")]
    public AnimationCurve X;
    public AnimationCurve Y;
    public AnimationCurve W;
    public AnimationCurve H;

    void Update()
    {
        if (!sequencer.isInitialised)
            sequencer.Initialise();

        float t = sequencer.Evaluate();

        Rect r = new Rect(X.Evaluate(t), Y.Evaluate(t), W.Evaluate(t), H.Evaluate(t));
        image.uvRect = r;
    }
}
