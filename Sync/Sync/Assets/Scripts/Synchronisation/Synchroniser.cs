using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Synchronisation/Synchroniser")]
public class Synchroniser
{
    public delegate void OnTime();

    private Dictionary<object, OnTime> delegates = new Dictionary<object, OnTime>();

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

            foreach (KeyValuePair<object, OnTime> callback in delegates)
            {
                callback.Value.Invoke();
            }
        }
    }

    public void RegisterCallback(object owner, OnTime callback)
    {
        delegates.Add(owner, callback);
    }

    public void UnregisterCallback(object owner)
    {
        delegates.Remove(owner);
    }

    public void ChangeGoal(double newGoal)
    {
        goal = newGoal;
    }
}
