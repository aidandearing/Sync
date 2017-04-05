using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RayQuery : MonoBehaviour
{
    public float distance = 100.0f;
    public string[] layerCriteria;
    public Ray ray;
    public RaycastHit rayHitLast;

    void FixedUpdate()
    {
        ray.origin = transform.position;
        ray.direction = transform.forward;

        Physics.Raycast(ray, out rayHitLast, distance, LayerMask.GetMask(layerCriteria));
    }
}
