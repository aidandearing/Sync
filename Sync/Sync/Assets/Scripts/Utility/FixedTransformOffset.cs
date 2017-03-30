using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[AddComponentMenu("Scripts/Utility/Fixed Transform Offset")]
public class FixedTransformOffset : MonoBehaviour
{
    public enum Mode { Update, Fixed };
    public Mode mode = Mode.Update;

    public Transform target;
    public Vector3 offset;

    public void Update()
    {
        if (mode == Mode.Update)
        {
            transform.position = target.position + offset;
        }
    }

    public void FixedUpdate()
    {
        if (mode == Mode.Fixed)
        {
            transform.position = target.position + offset;
        }
    }
}
