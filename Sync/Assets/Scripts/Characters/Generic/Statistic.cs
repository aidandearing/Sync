using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class Statistic : BlackboardValue
{
    protected string name = "";
    public string Name
    {
        get
        {
            return name;
        }
        protected set
        {
            name = value;
        }
    }

    private List<StatisticModifier> modifiers = new List<StatisticModifier>();

    public override object Value
    {
        get
        {
            object v = base.Value;

            foreach(StatisticModifier modifier in modifiers)
            {
                modifier.Modify(v);
            }

            return v;
        }

        set
        {
            base.Value = value;
        }
    }
}
