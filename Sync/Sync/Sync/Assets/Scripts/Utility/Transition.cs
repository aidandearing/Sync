using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class Transition
{
    public enum Function { Linear, Loop, PingPong };
    public Function function;

    private float start;
    [Range(0.0f, 360.0f)]
    public float duration = 1;

    public float Evaluate()
    {
        float percentage = 0;

        // First Evaluate
        if (start == 0)
        {
            start = Time.time;
        }

        float deltaTime = Time.time - start;

        // If it doesn't bounce back, it will never go over duration
        if (function != Function.PingPong)
        {
            if (function == Function.Loop)
            {
                percentage = (deltaTime % duration) / duration;
            }
            else if (function == Function.Linear)
            {
                percentage = deltaTime / duration;
                percentage = Mathf.Min(percentage, 1.0f);
            }
        }
        // If it does bounce back then deltaTime is allowed to reach duration doubled
        else
        {
            percentage = (deltaTime > duration) ? 1 - (deltaTime - duration) / duration : deltaTime / duration;
        }
        
        return percentage;
    }
}
