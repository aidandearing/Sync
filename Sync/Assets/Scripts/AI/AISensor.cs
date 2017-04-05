using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class AISensor : MonoBehaviour
{
    public float sensorDistance = 10.0f;
    public string sensorCriteriaTag = Literals.Strings.Tags.Player;
    public string[] sensorCriteriaLayers;

    public float senseCooldown = 2.0f;
    public float senseCooldownDelta = 1.0f;
    private float senseCooldownCurrent;

    public virtual void Start()
    {
        senseCooldownCurrent = UnityEngine.Random.Range(senseCooldown - senseCooldownDelta, senseCooldown + senseCooldownDelta);
    }

    public virtual void FixedUpdate()
    {
        if (senseCooldownCurrent > 0)
            senseCooldownCurrent -= Time.fixedDeltaTime;
    }

    public bool CanSense()
    {
        return senseCooldownCurrent <= 0;
    }

    public void Sensed()
    {
        senseCooldownCurrent += UnityEngine.Random.Range(senseCooldown - senseCooldownDelta, senseCooldown + senseCooldownDelta);
    }

    public int GetLayerMask()
    {
        return (sensorCriteriaLayers.Length > 0) ? LayerMask.GetMask(sensorCriteriaLayers) : LayerMask.GetMask(Literals.Strings.Physics.Layers.Default);
    }

    public abstract bool CanSense(Transform t);

    public abstract Transform Sense();

    public abstract Transform[] SenseAll();
}
