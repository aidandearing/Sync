using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[AddComponentMenu("Scripts/Utility/Fixed Rotation")]
public class FixedRotation : MonoBehaviour
{
    public enum Mode { Update, FixedUpdate };

    public Mode mode = Mode.Update;
    public Vector3 forward;
    public Vector3 up;

    void Update()
    {
        if (mode == Mode.Update)
            transform.rotation = Quaternion.LookRotation(forward, up);
    }

    void FixedUpdate()
    {
        if (mode == Mode.FixedUpdate)
            transform.rotation = Quaternion.LookRotation(forward, up);
    }
}