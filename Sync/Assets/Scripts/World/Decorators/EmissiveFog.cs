using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[AddComponentMenu("Scripts/World/Decorator/Emissive matches Fog")]
public class EmissiveFog : MonoBehaviour
{
    public Material material;

    public void Update()
    {
        material.SetColor("_EmissionColor", RenderSettings.fogColor);
    }
}
