using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LentoBarrierController : MonoBehaviour
{
    [Header("Controller")]
    public Controller controller;
    public PlayerController lento;

    [Header("Material")]
    public Material material;
    public Gradient colourOverLife;
    public Gradient colourEdgeOverLife;
    //public AnimationCurve perlinSpeedOverLife;
    public AnimationCurve cutoffOverLife;
    public AnimationCurve cutoffRangeOverLife;
    public AnimationCurve cutoffOverSustain;
    public AnimationCurve cutoffRangeOverSustain;

    [Header("Health")]
    public float healthMax = 2500;
    public float health = 0;
    public float healthLast = 0;
    public float healthPerSecond = 250;
    public float healthDelay = 5;
    public float healthDelayCurrent = 0;
    public bool isDelayedOnHit = true;
    public bool isRegenerating = false;

    [Header("Sustain")]
    public float sustainDuration = 1.0f;
    public float sustainDurationCurrent = 0.0f;
    public bool isSustained = false;

    void Start()
    {
        healthLast = health;
    }

    void Update()
    {
        if (isSustained)
        {
            if (sustainDurationCurrent < sustainDuration)
                sustainDurationCurrent += Time.deltaTime;
            else
            {
                if (isRegenerating)
                {
                    if (health < healthMax)
                        health += healthPerSecond * Time.deltaTime;
                    else
                    {
                        health = healthMax;
                        isRegenerating = false;
                    }
                }
                else
                {
                    if (healthDelayCurrent < healthDelay)
                    {
                        healthDelayCurrent += Time.deltaTime;
                    }
                    else
                    {
                        isRegenerating = true;
                    }
                }
            }
        }
        else if (sustainDurationCurrent > 0)
        {
            sustainDurationCurrent -= Time.deltaTime;
        }

        float evaluate = health / healthMax;
        float evaluateSustain = sustainDurationCurrent / sustainDuration;
        material.SetColor("_Colour", colourOverLife.Evaluate(evaluate));
        material.SetColor("_ColourEdge", colourEdgeOverLife.Evaluate(evaluate));
        //material.SetFloat("_PSpeed", perlinSpeedOverLife.Evaluate(evaluate));
        material.SetFloat("_Cutoff", Mathf.Min(cutoffOverLife.Evaluate(evaluate), cutoffOverSustain.Evaluate(evaluateSustain)));
        material.SetFloat("_CutoffRange", Mathf.Min(cutoffRangeOverLife.Evaluate(evaluate), cutoffRangeOverSustain.Evaluate(evaluateSustain)));
    }

    void OnTriggerEnter(Collider other)
    {
        
    }
}
