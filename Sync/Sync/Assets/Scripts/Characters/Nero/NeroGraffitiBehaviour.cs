using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[AddComponentMenu("Scripts/Characters/Nero/NeroGraffitiBehaviour")]
public class NeroGraffitiBehaviour : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The Material this Graffiti object is going to clone on start, and modify based on Nero's distance from it.")]
    public Material graffiti;
    new public Renderer renderer;

    [Header("Colour")]
    public Gradient graffitiColour;

    [Header("Cutout")]
    public AnimationCurve graffitiCutout;

    [Header("Emission")]
    public Gradient graffitiEmission;

    [Header("Distance")]
    [Range(0.0f, 1.0f)]
    public float distance;

    private NeroGraffitiControl controller;

    private float distanceVelocity = 0;

    void Start()
    {
        renderer.material = Instantiate(graffiti);
    }

    void Update()
    {
        //if (controller == null)
        //{
        //    gameObject.SetActive(false);
        //    return;
        //}

        distance = Mathf.SmoothDamp(distance, (controller != null) ? controller.Evaluate(gameObject.transform) : 0.0f, ref distanceVelocity, (controller != null) ? controller.velocityLimit : 1f);

        renderer.material.SetColor("_Color", graffitiColour.Evaluate(distance));
        renderer.material.SetFloat("_Cutoff", graffitiCutout.Evaluate(distance));
        renderer.material.SetColor("_EmissionColor", graffitiEmission.Evaluate(distance));
    }

    public void SetController(NeroGraffitiControl controller)
    {
        this.controller = controller;
    }
}
