using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LentoLaserAttack : MonoBehaviour
{
    [Header("References")]
    public LineRenderer line;
    public GameObject startEffect;
    public GameObject endEffect;
    public CubeController target;

    [Header("Duration")]
    public float duration = 0.25f;
    public float durationCurrent = 0.0f;
    public Gradient startColour;
    public Gradient endColour;

    [Header("Misc")]
    public float damagePerSecond = 100.0f;
    public Transform start;
    public Vector3 end;
    public RaycastHit hit;

    private Vector3 direction;

    public void Begin(CubeController target, Transform start, RaycastHit hit, Vector3 dir)
    {
        if (target != null)
        {
            this.target = target;
        }

        this.start = start;
        this.hit = hit;
        this.direction = dir;
    }

    void Update()
    {
        durationCurrent += Time.deltaTime;

        float t = Mathf.Clamp(durationCurrent / duration, 0, 1);

        line.startColor = startColour.Evaluate(t);
        line.endColor = endColour.Evaluate(t);

        //Ray ray = new Ray(start, direction);

        //RaycastHit hit;

        //Physics.Raycast(ray, out hit, 1000.0f, LayerMask.GetMask("Entities", "Walls", "Floors"));

        if (hit.collider != null)
        {
            end = hit.point;

            if (target)
            {
                if (durationCurrent < duration)
                    target.statistics["health"].Value = (float)target.statistics["health"].Value - damagePerSecond * Time.deltaTime;
            }
        }
        else
        {
            end = direction * 1000.0f;
        }

        endEffect.transform.position = end;

        line.SetPosition(0, start.position);
        line.SetPosition(1, end);
    }
}
