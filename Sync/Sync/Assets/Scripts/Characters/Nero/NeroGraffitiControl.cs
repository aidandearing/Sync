using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[AddComponentMenu("Scripts/Characters/Nero/NeroGraffitiControl")]
public class NeroGraffitiControl : MonoBehaviour
{
    [Header("Sequencer")]
    public SequencerGradient sequencer;

    [Header("References")]
    public Transform location;

    [Header("Distance")]
    public float nearDistance = 5.0f;
    public float farDistance = 25.0f;

    [Header("Velocity")]
    [Tooltip("This controls the speed graffiti growth is limited to.")]
    public float velocityLimit = 0.01f;

    void Update()
    {
        if (!sequencer.isInitialised)
        {
            sequencer.Initialise();
            sequencer.callback = Callback;
        }
    }

    public float Evaluate(Transform other)
    {
        float distance = Mathf.Clamp((other.position - location.position).magnitude, nearDistance, farDistance) / (farDistance - nearDistance);
        return Mathf.Lerp(0, 1, 1 - distance); 
    }

    void Callback()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit[] hits = Physics.SphereCastAll(ray, farDistance, farDistance);//, Literals.IntLiterals.Physics.Layers.graffiti);

        foreach(RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.tag == Literals.StringLiterals.Tags.NeroGraffiti)
            {
                NeroGraffitiBehaviour graffiti = hit.collider.gameObject.GetComponent<NeroGraffitiBehaviour>();
                graffiti.SetController(this);
            }
        }
    }
}
