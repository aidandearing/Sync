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
    public List<PropertyInstance> propertyInstances = new List<PropertyInstance>();

    public StatisticModifier modifierInstance;

    public void Apply(Controller target)
    {
        modifierInstance = new StatisticModifier() { modifier = this.modifier, Value = value };
        if (modification == Modification.Temporary)
        {
            PropertyInstance inst = new PropertyInstance() { lifeStart = Time.time, target = target, origin = this };
            propertyInstances.Add(GameObject.Instantiate(inst, target.transform, false));
            (target.statistics[this.target].Value as Statistic).AddModifier(modifierInstance);
        }
        else if (modification == Modification.Permanent)
        {
            BlackboardValue.ValueType type = (target.statistics[this.target].Value as Statistic).Type;
            (target.statistics[this.target].Value as Statistic).Value = modifierInstance.Modify((target.statistics[this.target].Value as Statistic).Value);
            (target.statistics[this.target].Value as Statistic).Type = type;
        }
    }

    public void Remove(PropertyInstance instance)
    {
        if (modification == Modification.Temporary)
        {
            propertyInstances.Remove(instance);
            (instance.target.statistics[this.target].Value as Statistic).RemoveModifier(modifierInstance);
        }
    }
}

[Serializable]
public class PropertyInstance : MonoBehaviour
{
    public float lifeStart;
    public Controller target;
    public Property origin;

    void Update()
    {
        if (Time.time - lifeStart > origin.duration)
        {
            origin.Remove(this);
        }
    }
}
