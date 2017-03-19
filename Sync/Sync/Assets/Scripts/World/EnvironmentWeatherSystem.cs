using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class EnvironmentWeatherSystem
{
    public string name = "System";

    [Header("Environmental")]
    public Gradient sunlight;
    public Gradient fog;
    public bool hasClouds = false;
    public Gradient clouds;
    [Range(0.0000001f, 100.0f)]
    public float cloudsDensityDay = 0.01f;
    [Range(0.0000001f, 100.0f)]
    public float cloudsDensityNight = 0.1f;
    [Range(0, 1)]
    public float cloudsScatteringDensityDay = 0.001f;
    [Range(0, 1)]
    public float cloudsScatteringDensityNight = 0.001f;
    public float cloudsAlphaRangeDay = 0.5f;
    public float cloudsAlphaRangeNight = 0.5f;
    public float cloudsAlphaThresholdDay = 0.5f;
    public float cloudsAlphaThresholdNight = 0.5f;
    public float cloudsStratification = 1.0f;
    public Vector3 windDirection = new Vector3(1.0f, 0.0f, 0.0f);
    [Range(0.0000001f, 1.0f)]
    public float fogDensityDay = 0.0005f;
    [Range(0.0000001f, 1.0f)]
    public float fogDensityNight = 0.001f;
    [Header("Euler Rotations")]
    public Vector3 day;
    public Vector3 night;
    [Header("Transition")]
    [Range(0.0f, 1.0f)]
    public float transitionStart = 0.49f;
    [Range(0.0f, 1.0f)]
    public float transitionEnd = 0.51f;

    [Header("City")]
    public Gradient cityDiffuse;
    public AnimationCurve cityMetallic;
    public AnimationCurve citySmoothness;
    public AnimationCurve cityLightRange;

    [Header("Water")]
    public Gradient waterDiffuse;
    public AnimationCurve waterSmoothness;

    [Header("System Duration")]
    public int durationMin = 1;
    public int durationMax = 10;
    public Synchronism.Synchronisations synchronisation = Synchronism.Synchronisations.BAR_8;
    private Synchroniser synchroniser;

    private EnvironmentController controller;

    public void Update()
    {
        if (controller == null)
            controller = Blackboard.Global[Literals.Strings.Blackboard.Controllers.Environment].Value as EnvironmentController;

        float evaluate = controller.sequencer.Evaluate();

        controller.light.color = sunlight.Evaluate(evaluate);
        RenderSettings.fogColor = fog.Evaluate(evaluate);

        if (evaluate >= transitionStart && evaluate <= transitionEnd)
        {
            EvaluateTimeOfDay(evaluate);
        }
        else if (evaluate < 0.5)
        {
            // NIGHT
            if (controller.isDay == true)
            {
                EvaluateTimeOfDay(0);

                controller.isDay = false;
            }
        }
        else
        {
            // DAY
            if (controller.isDay == false)
            {
                EvaluateTimeOfDay(1);

                controller.isDay = true;
            }
        }

        if (hasClouds)
            EvaluateWind(evaluate);
    }

    public void EvaluateWind(float evaluate)
    {
        float cloudsDensity = Mathf.Lerp(cloudsDensityNight, cloudsDensityDay, evaluate);

        for (int i = 0; i < controller.clouds.Length; i++)
        {
            float strata = (float)(i * cloudsStratification) / ((controller.clouds.Length - 1) / cloudsDensity);
            controller.clouds[i].SetTextureOffset("_MainTex", windDirection * Time.time);
        }
    }

    private void EvaluateTimeOfDay(float evaluate)
    {
        float range = (evaluate - transitionStart) / (transitionEnd - transitionStart);

        controller.light.transform.rotation = Quaternion.Euler(Vector3.Lerp(night, day, range));
        RenderSettings.fogDensity = Mathf.Lerp(fogDensityNight, fogDensityDay, range);
        controller.cityLight.range = cityLightRange.Evaluate(range);

        controller.city.SetColor("_Color", cityDiffuse.Evaluate(range));
        controller.city.SetFloat("_Glossiness", citySmoothness.Evaluate(range));
        controller.city.SetFloat("_Metallic", cityMetallic.Evaluate(range));

        controller.water.SetColor("_Color", waterDiffuse.Evaluate(range));
        controller.water.SetFloat("_Glossiness", waterSmoothness.Evaluate(range));

        for (int i = 0; i < controller.particles.Length; i++)
        {
            ParticleSystem.EmissionModule module = controller.particles[i].emission;
            module.rateOverTime = controller.particleEmitRate[i].Evaluate(range);
        }

        if (hasClouds)
        {
            for (int i = 0; i < controller.clouds.Length; i++)
            {
                controller.clouds[i].SetColor("_Color", clouds.Evaluate(range));
                controller.clouds[i].SetColor("_Sun", controller.light.color);
                controller.clouds[i].SetFloat("_Scattering", Mathf.Lerp(cloudsScatteringDensityNight, cloudsScatteringDensityDay, range));
                controller.clouds[i].SetFloat("_Density", Mathf.Lerp(cloudsDensityNight, cloudsDensityDay, range));
                controller.clouds[i].SetFloat("_AlphaRange", Mathf.Lerp(cloudsAlphaRangeNight, cloudsAlphaRangeDay, range));
                controller.clouds[i].SetFloat("_AlphaThreshold", Mathf.Lerp(cloudsAlphaThresholdNight, cloudsAlphaThresholdDay, range));
            }
        }
        else
        {
            for (int i = 0; i < controller.clouds.Length; i++)
            {
                controller.clouds[i].SetColor("_Color", new Color(0,0,0,0));
                controller.clouds[i].SetColor("_Sun", new Color(0,0,0,0));
            }
        }
    }
}
