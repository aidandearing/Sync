using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ContraSynchronism : MonoBehaviour
{
    [Header("References")]
    public Material contraMaterial;
    public GameObject discoBall;
    public Material discoBallMaterial;
    public UnityStandardAssets.Utility.AutoMoveAndRotate autoMoveandRotate;
    public Animator controller;
    public UnityStandardAssets.ImageEffects.EdgeDetection edgeShader;

    [Header("Sequencers")]
    public SequencerGradient[] sequencers;

    [Header("Emission")]
    public Gradient emissionGradient;

    [Header("Disco Ball Emission")]
    public Gradient discoBallEmissionGradient;

    [Header("Auto Move and Rotate")]
    public Vector3 autoMoveandRotateStart;
    public Vector3 autoMoveandRotateEnd;

    [Header("Controller")]
    public AnimationCurve controllerSpeed;

    [Header("Edge Detection")]
    public Gradient edgeColour;
    public AnimationCurve edgeThreshold;
    public AnimationCurve edgeDistance;
    public AnimationCurve edgeOnly;
    public AnimationCurve edgeDepth;
    public AnimationCurve edgeNormal;

    void Update()
    {
        foreach (SequencerGradient sequencer in sequencers)
        {
            if (!sequencer.isInitialised)
                sequencer.Initialise();
        }

        contraMaterial.SetColor("_EmissionColor", emissionGradient.Evaluate(sequencers[0].Evaluate()));
        discoBallMaterial.SetColor("_EmissionColor", discoBallEmissionGradient.Evaluate(sequencers[1].Evaluate()));
        autoMoveandRotate.rotateDegreesPerSecond.value = Vector3.Lerp(autoMoveandRotateStart, autoMoveandRotateEnd, sequencers[2].Evaluate());
        controller.SetFloat("speedMultiplier", controllerSpeed.Evaluate(sequencers[3].Evaluate()));

        edgeShader.edgesOnly = edgeOnly.Evaluate(sequencers[4].Evaluate());
        edgeShader.edgesOnlyBgColor = edgeColour.Evaluate(sequencers[4].Evaluate());
        edgeShader.lumThreshold = edgeThreshold.Evaluate(sequencers[4].Evaluate());
        edgeShader.sampleDist = edgeDistance.Evaluate(sequencers[4].Evaluate());
        edgeShader.sensitivityDepth = edgeDepth.Evaluate(sequencers[4].Evaluate());
        edgeShader.sensitivityNormals = edgeNormal.Evaluate(sequencers[4].Evaluate());
    }

    void FixedUpdate()
    {

    }
}
