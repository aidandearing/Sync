﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    [Header("Sequencer")]
    public SequencerGradient sequencer;

    [Header("Weather")]
    public EnvironmentWeatherSystem[] weather;

    [Header("References")]
    new public Light light;
    public Light cityLight;
    public ParticleSystem[] particles;
    public Material[] clouds;

    [Header("City")]
    public Material city;
    
    [Header("Water")]
    public Material water;

    [Header("Particles")]
    public AnimationCurve[] particleEmitRate;

    public bool isDay;

    void Start()
    {
        Blackboard.Global.Add(Literals.Strings.Blackboard.Controllers.Environment, new BlackboardValue() { Value = this });
    }

    void Update()
    {
        if (!sequencer.isInitialised)
            sequencer.Initialise();

        weather[0].Update();
    }
}
