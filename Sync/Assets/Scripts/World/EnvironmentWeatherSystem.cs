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
    public Gradient ambient;
    public Gradient fog;
    public bool hasClouds = false;
    public Gradient cloudsLow;
    public Gradient cloudsHigh;
    public AnimationCurve cloudHeight;
    public AnimationCurve cloudDensity;
    public AnimationCurve cloudScattering;
    public AnimationCurve cloudAlphaRange;
    public AnimationCurve cloudAlphaThreshold;
    public float cloudsStratification = 1.0f;
    public Vector3 windDirection = new Vector3(1.0f, 0.0f, 0.0f);
    public AnimationCurve fogDensity;
    [Header("Euler Rotations")]
    public Vector3 day;
    public Vector3 night;
    public bool useEulerArray = false;
    public SequencerVector3 eulerArray;
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

    //private EnvironmentController controller;

    public void Start(EnvironmentController controller)
    {
        for(int i = 0; i < controller.cloudTransforms.Length; i++)
        {
            float s = cloudHeight.Evaluate(1.0f - i / (controller.cloudTransforms.Length - 1.0f));
            Vector3 scale = new Vector3(s, s, s);
            controller.cloudTransforms[i].transform.localScale = scale;
        }
    }

    public void Update(EnvironmentController controller)
    {
        //if (controller == null)
        //    controller = Blackboard.Global[Literals.Strings.Blackboard.Controllers.Environment].Value as EnvironmentController;

        float evaluate = controller.sequencer.Evaluate();

        controller.light.color = sunlight.Evaluate(evaluate);
        RenderSettings.ambientLight = ambient.Evaluate(evaluate);
        RenderSettings.fogColor = fog.Evaluate(evaluate);

        if (evaluate >= transitionStart && evaluate <= transitionEnd)
        {
            EvaluateTimeOfDay(controller, evaluate);
        }
        else if (evaluate < 0.5)
        {
            // NIGHT
            if (controller.isDay == true)
            {
                EvaluateTimeOfDay(controller, 0);

                controller.isDay = false;
            }
        }
        else
        {
            // DAY
            if (controller.isDay == false)
            {
                EvaluateTimeOfDay(controller, 1);

                controller.isDay = true;
            }
        }

        if (hasClouds)
            EvaluateWind(controller, evaluate);
    }

    public void EvaluateWind(EnvironmentController controller, float evaluate)
    {
        float cloudsDensity = cloudDensity.Evaluate(evaluate);

        for (int i = 0; i < controller.clouds.Length; i++)
        {
            //float strata = (float)(i * cloudsStratification) / ((controller.clouds.Length - 1) / cloudsDensity);
            controller.clouds[i].SetTextureOffset("_MainTex", windDirection * Time.time);
        }
    }

    private void EvaluateTimeOfDay(EnvironmentController controller, float evaluate)
    {
        float range = Mathf.Clamp((evaluate - transitionStart) / (transitionEnd - transitionStart),0,1);

        if (!useEulerArray)
            controller.light.transform.rotation = Quaternion.Euler(Vector3.Lerp(night, day, range));
        else
        {
            int length = eulerArray.objs.Length - 1;
            int low = Mathf.FloorToInt(range * length);
            int high = Mathf.Clamp(low + 1, 0, length);
            float lowPercent = (low / (float)length);
            float highPercent = (high / (float)length);
            float localPercent = (range - lowPercent) / (highPercent - lowPercent);
            controller.light.transform.rotation = Quaternion.Euler(Vector3.Lerp(eulerArray.Evaluate(lowPercent), eulerArray.Evaluate(highPercent), localPercent));
        }

        RenderSettings.fogDensity = fogDensity.Evaluate(range);
        controller.cityLight.range = cityLightRange.Evaluate(range);

        controller.city.SetColor("_Color", cityDiffuse.Evaluate(range));
        controller.city.SetFloat("_Glossiness", citySmoothness.Evaluate(range));
        controller.city.SetFloat("_Metallic", cityMetallic.Evaluate(range));

        controller.water.SetColor("_Color", waterDiffuse.Evaluate(range));
        controller.water.SetFloat("_Glossiness", waterSmoothness.Evaluate(range));

        for (int i = 0; i < controller.particles.Length; i++)
        {
            ParticleSystem.EmissionModule module = controller.particles[i].emission;
            float rate = controller.particleEmitRate[i].Evaluate(range);
            if (rate <= 0)
                controller.particles[i].gameObject.SetActive(false);
            else
            {
                if (!controller.particles[i].gameObject.activeSelf)
                    controller.particles[i].gameObject.SetActive(true);
                module.rateOverTime = rate;
            }
        }

        if (hasClouds)
        {
            for (int i = 0; i < controller.clouds.Length; i++)
            {
                controller.clouds[i].SetColor("_Color", cloudsLow.Evaluate(range));
                controller.clouds[i].SetColor("_Sun", cloudsHigh.Evaluate(range));
                controller.clouds[i].SetFloat("_Scattering", cloudScattering.Evaluate(range));
                controller.clouds[i].SetFloat("_Density", cloudDensity.Evaluate(range));
                controller.clouds[i].SetFloat("_AlphaRange", cloudAlphaRange.Evaluate(range));
                controller.clouds[i].SetFloat("_AlphaThreshold", cloudAlphaThreshold.Evaluate(range));
                controller.clouds[i].SetColor("_RimColour", fog.Evaluate(range));
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

        controller.cloudsHigh = cloudsHigh.Evaluate(range);
    }
}
