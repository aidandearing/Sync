using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    [Header("Sequencer")]
    public SequencerGradient sequencer;
    [Header("Environmental")]
    public Gradient sunlight;
    public Gradient fog;
    [Range(0.0000001f, 1.0f)]
    public float fogDensityDay = 0.0005f;
    [Range(0.0000001f, 1.0f)]
    public float fogDensityNight = 0.001f;
    [Header("Euler Rotations")]
    public Vector3 day;
    public Vector3 night;
    [Header("Transition")]
    [Range(0.0f,1.0f)]
    public float transitionStart = 0.49f;
    [Range(0.0f,1.0f)]
    public float transitionEnd = 0.51f;

    [Header("References")]
    new public Light light;
    public Light cityLight;
    public ParticleSystem[] particles;

    [Header("City")]
    public Material city;
    public Gradient cityDiffuse;
    public AnimationCurve cityMetallic;
    public AnimationCurve citySmoothness;
    public AnimationCurve cityLightRange;
    
    [Header("Water")]
    public Material water;
    public Gradient waterDiffuse;
    public AnimationCurve waterSmoothness;

    [Header("Particles")]
    public AnimationCurve[] particleEmitRate;

    public bool isDay;

    void Update()
    {
        if (!sequencer.isInitialised)
            sequencer.Initialise();

        float evaluate = sequencer.Evaluate();

        light.color = sunlight.Evaluate(evaluate);
        RenderSettings.fogColor = fog.Evaluate(evaluate);

        if (evaluate >= transitionStart && evaluate <= transitionEnd)
        {
            float range = (evaluate - transitionStart) / (transitionEnd - transitionStart);

            light.transform.rotation = Quaternion.Euler(Vector3.Lerp(night, day, range));
            RenderSettings.fogDensity = Mathf.Lerp(fogDensityNight, fogDensityDay, range);
            cityLight.range = cityLightRange.Evaluate(range);

            city.SetColor("_Color", cityDiffuse.Evaluate(range));
            city.SetFloat("_Glossiness", citySmoothness.Evaluate(range));
            city.SetFloat("_Metallic", cityMetallic.Evaluate(range));

            water.SetColor("_Color", waterDiffuse.Evaluate(range));
            water.SetFloat("_Glossiness", waterSmoothness.Evaluate(range));

            for (int i = 0; i < particles.Length; i++)
            {
                ParticleSystem.EmissionModule module = particles[i].emission;
                module.rateOverTime = particleEmitRate[i].Evaluate(range);
            }
        }
        else if (evaluate < 0.5)
        {
            // NIGHT
            if (isDay == true)
            {
                light.transform.rotation = Quaternion.Euler(night);
                RenderSettings.fogDensity = fogDensityNight;

                city.SetColor("_Color", cityDiffuse.Evaluate(0));
                city.SetFloat("_Glossiness", citySmoothness.Evaluate(0));
                city.SetFloat("_Metallic", cityMetallic.Evaluate(0));

                water.SetColor("_Color", waterDiffuse.Evaluate(0));
                water.SetFloat("_Glossiness", waterSmoothness.Evaluate(0));

                for (int i = 0; i < particles.Length; i++)
                {
                    ParticleSystem.EmissionModule module = particles[i].emission;
                    module.rateOverTime = particleEmitRate[i].Evaluate(0);
                }

                isDay = false;
            }
        }
        else
        {
            // DAY
            if (isDay == false)
            {
                light.transform.rotation = Quaternion.Euler(day);
                RenderSettings.fogDensity = fogDensityDay;

                city.SetColor("_Color", cityDiffuse.Evaluate(1));
                city.SetFloat("_Glossiness", citySmoothness.Evaluate(1));
                city.SetFloat("_Metallic", cityMetallic.Evaluate(1));

                water.SetColor("_Color", waterDiffuse.Evaluate(1));
                water.SetFloat("_Glossiness", waterSmoothness.Evaluate(1));

                for (int i = 0; i < particles.Length; i++)
                {
                    ParticleSystem.EmissionModule module = particles[i].emission;
                    module.rateOverTime = particleEmitRate[i].Evaluate(1);
                }

                isDay = true;
            }
        }
    }
}
