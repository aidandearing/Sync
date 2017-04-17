using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class StatisticModifier : BlackboardValue
{
    public enum Modifier { Add, Subtract, Multiply, Divide };
    public Modifier modifier = Modifier.Add;

    public object Modify(object v)
    {
        switch(modifier)
        {
            case Modifier.Add:
                if (v.GetType() == typeof(int))
                    return (int)v + (int)Value;
                else if (v.GetType() == typeof(float))
                    return (float)v + (float)Value;
                else if (v.GetType() == typeof(double))
                    return (double)v + (double)Value;
                else if (v.GetType() == typeof(Vector2))
                    return (Vector2)v + (Vector2)Value;
                else if (v.GetType()== typeof(Vector3))
                    return (Vector3)v + (Vector3)Value;
                else if (v.GetType()== typeof(Vector4))
                    return (Vector4)v + (Vector4)Value;
                else
                    return v;
            case Modifier.Subtract:
                if (v.GetType() == typeof(int))
                    return (int)v - (int)Value;
                else if (v.GetType()== typeof(float))
                    return (float)v - (float)Value;
                else if (v.GetType()== typeof(double))
                    return (double)v - (double)Value;
                else if (v.GetType()== typeof(Vector2))
                    return (Vector2)v - (Vector2)Value;
                else if (v.GetType()== typeof(Vector3))
                    return (Vector3)v - (Vector3)Value;
                else if (v.GetType()== typeof(Vector4))
                    return (Vector4)v - (Vector4)Value;
                else
                    return v;
            case Modifier.Multiply:
                if (v.GetType()== typeof(int))
                    return (int)v * (int)Value;
                else if (v.GetType()== typeof(float))
                    return (float)v * (float)Value;
                else if (v.GetType()== typeof(double))
                    return (double)v * (double)Value;
                else if (v.GetType()== typeof(Vector2))
                    return (Vector2)v * (float)Value;
                else if (v.GetType()== typeof(Vector3))
                    return (Vector3)v * (float)Value;
                else if (v.GetType()== typeof(Vector4))
                    return (Vector4)v * (float)Value;
                else
                    return v;
            case Modifier.Divide:
                if (v.GetType()== typeof(int))
                    return (int)v / (int)Value;
                else if (v.GetType()== typeof(float))
                    return (float)v / (float)Value;
                else if (v.GetType()== typeof(double))
                    return (double)v / (double)Value;
                else if (v.GetType()== typeof(Vector2))
                    return (Vector2)v / (float)Value;
                else if (v.GetType()== typeof(Vector3))
                    return (Vector3)v / (float)Value;
                else if (v.GetType()== typeof(Vector4))
                    return (Vector4)v / (float)Value;
                else
                    return v;
        }

        return v;
    }

    public override object Value
    {
        get
        {
            return base.Value;
        }
        set
        {
            object v = value;

            if (v.GetType().IsValueType)
                base.Value = v;
            else
                throw new NotSupportedException("A StatisticModifier cannot be given a class as a value type\nA value of: " + v + " attempted to assign itself even though it is a: " + v.GetType() + "\nvalue: " + v + "\nvalue type: " + v.GetType());
        }
    }
}
