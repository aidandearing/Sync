using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
[AddComponentMenu("Scripts/Synchronisation/Edge Detection Sequencer")]
public class EdgeDetectionSequencer
{
    [Header("References")]
    public UnityStandardAssets.ImageEffects.EdgeDetection edgeShader;

    [Header("Sequencer")]
    public SequencerGradient sequencer;

    [Header("Edge Detection")]
    public Gradient edgeColour;
    public AnimationCurve edgeThreshold;
    public AnimationCurve edgeDistance;
    public AnimationCurve edgeOnly;
    public AnimationCurve edgeDepth;
    public AnimationCurve edgeNormal;

    public void Update()
    {
        if (!sequencer.isInitialised)
            sequencer.Initialise();

        edgeShader.edgesOnly = edgeOnly.Evaluate(sequencer.Evaluate());
        edgeShader.edgesOnlyBgColor = edgeColour.Evaluate(sequencer.Evaluate());
        edgeShader.lumThreshold = edgeThreshold.Evaluate(sequencer.Evaluate());
        edgeShader.sampleDist = edgeDistance.Evaluate(sequencer.Evaluate());
        edgeShader.sensitivityDepth = edgeDepth.Evaluate(sequencer.Evaluate());
        edgeShader.sensitivityNormals = edgeNormal.Evaluate(sequencer.Evaluate());
    }
}
