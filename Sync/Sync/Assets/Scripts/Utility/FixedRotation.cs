using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[AddComponentMenu("Scripts/Utility/Fixed Rotation")]
public class FixedRotation : MonoBehaviour
{
    public Vector3 forward;
    public Vector3 up;

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(forward, up);
    }
}