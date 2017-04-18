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
        //new StatisticSerializable() { name = Literals.Strings.Blackboard.Movement.CanWalkBackward, valueBool = false, type = BlackboardValue.ValueType.Boolean },
        new StatisticSerializable() { name = Literals.Strings.Blackboard.Movement.Count, valueInt = 0, type = BlackboardValue.ValueType.Integer },
        new StatisticSerializable() { name = Literals.Strings.Blackboard.Movement.Force, valueFloat = 0, type = BlackboardValue.ValueType.Float },
        new StatisticSerializable() { name = Literals.Strings.Blackboard.Movement.GlideDownToForward, valueFloat = 0, type = BlackboardValue.ValueType.Float },
        //new StatisticSerializable() { name = Literals.Strings.Blackboard.Movement.InheritVelocity, valueBool = true, type = BlackboardValue.ValueType.Boolean },
        //new StatisticSerializable() { name = Literals.Strings.Blackboard.Movement.Input, valueVector3 = 0, type = BlackboardValue.ValueType.Vector3 },
        new StatisticSerializable() { name = Literals.Strings.Blackboard.Movement.SpeedAll, valueFloat = 0, type = BlackboardValue.ValueType.Float },
        new StatisticSerializable() { name = Literals.Strings.Blackboard.Movement.SpeedBackward, valueFloat = 1.0f, type = BlackboardValue.ValueType.Float },
        new StatisticSerializable() { name = Literals.Strings.Blackboard.Movement.SpeedForward, valueFloat = 5.0f, type = BlackboardValue.ValueType.Float },
        new StatisticSerializable() { name = Literals.Strings.Blackboard.Movement.SpeedSidestep, valueFloat = 2.0f, type = BlackboardValue.ValueType.Float },
        new StatisticSerializable() { name = Literals.Strings.Blackboard.Movement.SpeedTurn, valueFloat = 1800, type = BlackboardValue.ValueType.Float },
        new StatisticSerializable() { name = Literals.Strings.Blackboard.Movement.TeleportDistance, valueFloat = 0, type = BlackboardValue.ValueType.Float },
        new StatisticSerializable() { name = Literals.Strings.Blackboard.Movement.TeleportTarget, valueFloat = 0, type = BlackboardValue.ValueType.Float },
        new StatisticSerializable() { name = Literals.Strings.Blackboard.Movement.TeleportThroughWalls, valueFloat = 0, type = BlackboardValue.ValueType.Float },
        new StatisticSerializable() { name = Literals.Strings.Blackboard.Movement.TeleportToTarget, valueFloat = 0, type = BlackboardValue.ValueType.Float },
        //new StatisticSerializable() { name = Literals.Strings.Blackboard.Movement.ThrustCurve, valueFloat = 0, type = BlackboardValue.ValueType.Float },
        //new StatisticSerializable() { name = Literals.Strings.Blackboard.Movement.ThrustSequencer, valueFloat = 0, type = BlackboardValue.ValueType.Float },
        //new StatisticSerializable() { name = Literals.Strings.Blackboard.Movement.Vectoring, valueBool = 0, type = BlackboardValue.ValueType.Boolean },
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
