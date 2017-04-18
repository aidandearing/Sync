using UnityEngine;
using System;

public class PropertyInstance : MonoBehaviour
{
    public float lifeStart;
    public Controller target;
    public Property origin;
    public StatisticModifier modifier;

    void Update()
    {
        if (Time.time - lifeStart > origin.duration)
        {
            (target.statistics[origin.target] as Statistic).RemoveModifier(modifier);
            origin.Remove(this);
            Destroy(gameObject);
        }
    }

    public void Set(float time, Controller target, Property origin, StatisticModifier modifier)
    {
        lifeStart = time;
        this.target = target;
        this.origin = origin;
        this.modifier = modifier;
    }
}