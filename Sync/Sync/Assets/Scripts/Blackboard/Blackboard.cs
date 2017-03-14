﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Blackboard
{
    public static Dictionary<string, BlackboardValue> Global = new Dictionary<string, BlackboardValue>();

    private Dictionary<string, BlackboardValue> values = new Dictionary<string, BlackboardValue>();

    public bool ContainsKey(string key)
    {
        return values.ContainsKey(key);
    }

    public void Add(string key, BlackboardValue value)
    {
        values.Add(key, value);
    }

    public bool Remove(string key)
    {
        return values.Remove(key);
    }

    public BlackboardValue this[string key]
    {
        get
        {
            if (values.ContainsKey(key))
            {
                return values[key];
            }
            else
            {
                return null;
            }
        }
        set
        {
            if (values.ContainsKey(key))
            {
                values[key].Value = value;
            }
            else
            {
                values.Add(key, value);
            }
        }
    }
}