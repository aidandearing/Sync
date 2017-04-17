using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class Statistics
{
    public StatisticSerializable[] stats_editor = {
        new StatisticSerializable() { name = "health", valueFloat = 100.0f, type = BlackboardValue.ValueType.Float },
        new StatisticSerializable() { name = "healthMax", valueFloat = 100.0f, type = BlackboardValue.ValueType.Float },
        new StatisticSerializable() { name = "healthRegeneration", valueFloat = 10.0f, type = BlackboardValue.ValueType.Float },
        new StatisticSerializable() { name = "healthRegenerationDelay", valueFloat = 2.5f, type = BlackboardValue.ValueType.Float },
        //new StatisticSerializable() { name = Literals.Strings.Blackboard.Movement. }
    };

    private Blackboard statistics = new Blackboard();

    public void Start()
    {
        foreach(StatisticSerializable stat in stats_editor)
        {
            statistics.Add(stat.name, stat.Dump());
        }
    }

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
