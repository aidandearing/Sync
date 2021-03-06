﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Synchroniser
{
    public delegate void OnTime();

    private Dictionary<object, OnTime> delegates = new Dictionary<object, OnTime>();
    private List<object> delegatesToRemove = new List<object>();

    private double timer;
    private double goal;
    public double Duration
    {
        get
        {
            return goal;
        }
    }

    public float Percent
    {
        get
        {
            return (float)(timer / goal);
        }
    }

    public Synchroniser(double goal)
    {
        this.goal = goal;
        timer = 0;
    }

    public void Update()
    {
        timer += Time.deltaTime;

        if (timer >= goal)
        {
            timer -= goal;

            //if (goal > 0)
            //    Debug.Log("Synchroniser :" + goal + " has triggered " + delegates.Count + " delegates");

            List<object> nullCalls = new List<object>();

            foreach (KeyValuePair<object, OnTime> callback in delegates)
            {
                if (callback.Key == null)
                {
                    nullCalls.Add(callback.Key);
                }
            }

            foreach (object o in nullCalls)
            {
                delegates.Remove(o);
            }

            foreach (object obj in delegatesToRemove)
            {
                delegates.Remove(obj);
            }

            delegatesToRemove = new List<object>();

            foreach (KeyValuePair<object, OnTime> callback in delegates)
            {
                callback.Value.Invoke();
            }

            foreach(object obj in delegatesToRemove)
            {
                delegates.Remove(obj);
            }

            delegatesToRemove = new List<object>();
        }
    }

    public void RegisterCallback(object owner, OnTime callback)
    {
        delegates.Add(owner, callback);
    }

    public void UnregisterCallback(object owner)
    {
        delegatesToRemove.Add(owner);
    }

    public bool HasCallback(object owner)
    {
        return delegates.ContainsKey(owner);
    }

    public void ChangeGoal(double newGoal)
    {
        goal = newGoal;
    }
}
