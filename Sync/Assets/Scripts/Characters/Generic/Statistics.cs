using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class Statistics
{
    public BlackboardValue stats_editor;

    private Blackboard statistics = new Blackboard();

    public bool ContainsKey(string key)
    {
        return statistics.ContainsKey(key);
    }

    public void Add(string key, Statistic stat)
    {
        statistics.Add(key, stat);
    }

    public bool Remove(string key)
    {
        return statistics.Remove(key);
    }

    public Statistic this[string key]
    {
        get
        {
            if (statistics.ContainsKey(key))
            {
                return statistics[key] as Statistic;
            }
            else
            {
                return null;
            }
        }
        set
        {
            if (statistics.ContainsKey(key))
            {
                statistics[key].Value = value;
            }
            else
            {
                statistics.Add(key, value);
            }
        }
    }
}
