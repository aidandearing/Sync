using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class AISensor : MonoBehaviour
{
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

    public abstract Transform Sense();

    public abstract Transform[] SenseAll();
}
