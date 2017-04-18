using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class Property
{
    [Header("Statistic")]
    public string target = "health";
    public StatisticModifier.Modifier modifier;
    public enum Modification { Temporary, Permanent };
    public Modification modification = Modification.Temporary;
    public float value = 0.0f;

    [Header("Duration")]
    public float duration = 2.0f;
    public PropertyInstance propertyPrefab;
    public List<PropertyInstance> propertyInstances = new List<PropertyInstance>();

    public StatisticModifier modifierInstance;

    public void Apply(Controller target)
    {
        modifierInstance = new StatisticModifier() { modifier = this.modifier, Value = value };

        if (modification == Modification.Temporary)
        {
            PropertyInstance inst = GameObject.Instantiate(propertyPrefab, target.transform, false) as PropertyInstance;
            inst.Set(Time.time, target, this, modifierInstance);// new PropertyInstance() { lifeStart = Time.time, target = target, origin = this };
            propertyInstances.Add(inst);
            (target.statistics[this.target] as Statistic).AddModifier(modifierInstance);
        }
        else if (modification == Modification.Permanent)
        {
            Debug.Log("Permanent Modification being applied to " + target + "'s " + this.target + " of type" + modifier + " for " + modification);
            BlackboardValue.ValueType type = (target.statistics[this.target] as Statistic).Type;
            (target.statistics[this.target] as Statistic).Value = modifierInstance.Modify((target.statistics[this.target] as Statistic).Value);
            (target.statistics[this.target] as Statistic).Type = type;
        }
    }

    public void Remove(PropertyInstance instance)
    {
        if (modification == Modification.Temporary)
        {
            propertyInstances.Remove(instance);
            (instance.target.statistics[this.target] as Statistic).RemoveModifier(modifierInstance);
        }
    }
}
