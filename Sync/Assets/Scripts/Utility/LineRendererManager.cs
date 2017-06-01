using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LineRendererManager : MonoBehaviour
{
    [Serializable]
    public class LineItem
    {
        public string name = "Line";

        [Header("References")]
        public LineRenderer renderer;

        [Header("Variables")]
        public Gradient startGradient;
        public Gradient endGradient;
        public AnimationCurve width;
        public float widthMultiplier = 1;
        public Vector3 startPoint;
        public Vector3 endPoint;
        public int segments = 5;

        public void Update(float t)
        {
            renderer.startColor = startGradient.Evaluate(t);
            renderer.endColor = endGradient.Evaluate(t);

            renderer.widthCurve = width;
            renderer.widthMultiplier = widthMultiplier;

            renderer.positionCount = segments;

            for (int i = 0; i < segments; i++)
            {
                Vector3 position = Vector3.Lerp(startPoint, endPoint, i / (segments - 1.0f));
                renderer.SetPosition(i, position);
            }
            //renderer.SetPositions(points);
        }
    }

    [Header("Synchronisation")]
    public Synchronism.Synchronisations synchronisation = Synchronism.Synchronisations.BAR_8;
    public Synchroniser synchroniser;

    public LineItem[] lines;

    private bool isInitialised = false;

    void Initialise()
    {
        if (!isInitialised)
        {
            Synchronism synch = ((Synchronism)Blackboard.Global[Literals.Strings.Blackboard.Synchronisation.Synchroniser].Value);
            if (synch != null)
            {
                synchroniser = synch.synchronisers[synchronisation];
                isInitialised = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialised)
            Initialise();

        foreach (LineItem line in lines)
        {
            line.Update(synchroniser.Percent);
        }
    }
}