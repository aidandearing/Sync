using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//[Serializable]
public class Statistic : BlackboardValue
{
    protected string name = "";
    public string Name
    {
        get
        {
            return name;
        }
        /*protected*/ set
        {
            name = value;
        }
    }

    private List<StatisticModifier> modifiers = new List<StatisticModifier>();
    private List<StatisticModifier> modifiersToRemove = new List<StatisticModifier>();

    public void AddModifier(StatisticModifier modifier)
    {
        modifiers.Add(modifier);
    }

    public void RemoveModifier(StatisticModifier modifier)
    {
        modifiersToRemove.Add(modifier);
    }

    public override object Value
    {
        get
        {
            object v = base.Value;

            foreach(StatisticModifier modifier in modifiers)
            {
                modifier.Modify(v);
            }

            foreach (StatisticModifier modifier in modifiersToRemove)
            {
                modifiers.Remove(modifier);
            }

            return v;
        }

        set
        {
            base.Value = value;
        }
    }

    public int Integer
    {
        get
        {
            return (int)Value;
        }
        set
        {
            Value = value;
        }
    }

    public float Float
    {
        get
        {
            return (float)Value;
        }
        set
        {
            Value = value;
        }
    }

    public double Double
    {
        get
        {
            return (double)Value;
        }
        set
        {
            Value = value;
        }
    }

    public Vector2 Vector2
    {
        get
        {
            return (Vector2)Value;
        }
        set
        {
            Value = value;
        }
    }

    public Vector3 Vector3
    {
        get
        {
            return (Vector3)Value;
        }
        set
        {
            Value = value;
        }
    }

    public Vector4 Vector4
    {
        get
        {
            return (Vector4)Value;
        }
        set
        {
            Value = value;
        }
    }

    public string String
    {
        get
        {
            return (string)Value;
        }
        set
        {
            Value = value;
        }
    }

    public GameObject GameObject
    {
        get
        {
            return (GameObject)Value;
        }
        set
        {
            Value = value;
        }
    }
}

[Serializable]
public class StatisticSerializable
{
    public string name;
    public BlackboardValue.ValueType type = BlackboardValue.ValueType.Integer;
    public int valueInt;
    public float valueFloat;
    public double valueDouble;
    public Vector2 valueVector2;
    public Vector3 valueVector3;
    public Vector4 valueVector4;
    public string valueString;
    public GameObject valueGameObject;

    public Statistic Dump()
    {
        switch (type)
        {
            case BlackboardValue.ValueType.Integer:
                return new Statistic() { Value = valueInt, Name = name };
            case BlackboardValue.ValueType.Float:
                return new Statistic() { Value = valueFloat, Name = name };
            case BlackboardValue.ValueType.Double:
                return new Statistic() { Value = valueDouble, Name = name };
            case BlackboardValue.ValueType.Vector2:
                return new Statistic() { Value = valueVector2, Name = name };
            case BlackboardValue.ValueType.Vector3:
                return new Statistic() { Value = valueVector3, Name = name };
            case BlackboardValue.ValueType.Vector4:
                return new Statistic() { Value = valueVector4, Name = name };
            case BlackboardValue.ValueType.String:
                return new Statistic() { Value = valueString, Name = name };
            case BlackboardValue.ValueType.GameObject:
                return new Statistic() { Value = valueGameObject, Name = name };
            case BlackboardValue.ValueType.Object:
                return new Statistic() { Value = valueGameObject, Name = name };
        }

        return null;
    }
}