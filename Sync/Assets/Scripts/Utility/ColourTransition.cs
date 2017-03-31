using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColourTransition
{
    public Gradient colours;
    private float colourGoal;
    private float colourStart;

    public Color Evaluate(float time)
    {
        return colours.Evaluate(Mathf.Lerp(colourStart, colourGoal, time));
    }

    public void SetColour()
    {
        float temp = Random.Range(Mathf.Clamp(colourGoal - 0.2f, 0, 1), Mathf.Clamp(colourGoal + 0.2f, 0, 1));

        while (colourGoal == temp)
        {
            temp = Random.Range(Mathf.Clamp(colourGoal - 0.2f, 0, 1), Mathf.Clamp(colourGoal + 0.2f, 0, 1));
        }

        colourStart = colourGoal;
        colourGoal = temp;
    }
}
