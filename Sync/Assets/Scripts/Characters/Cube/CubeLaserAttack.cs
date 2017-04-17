using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CubeLaserAttack : MonoBehaviour
{
    [Header("References")]
    public LineRenderer line;
    public GameObject startEffect;
    public GameObject endEffect;
    public PlayerController target;

    [Header("Duration")]
    public float duration = 2.0f;
    public float durationCurrent = 0.0f;
    public float durationDelay = 1.0f;
    public Gradient startColour;
    public Gradient endColour;

    [Header("Tracking")]
    public float trackingSpeed = 5.0f;

    [Header("Misc")]
    public float damagePerSecond = 50.0f;
    public Vector3 start;
    public Vector3 end;

    private Vector3 direction;

    void Start()
    {
        start = transform.position;
        startEffect.transform.position = start;
    }

    void FixedUpdate()
    {
        if (target == null)
            target = Blackboard.Global[Literals.Strings.Tags.Player + 1].Value as PlayerController;

        durationCurrent += Time.deltaTime;

        float t = Mathf.Clamp((durationCurrent - durationDelay) / (duration - durationDelay), 0, 1);

        if (durationCurrent >= durationDelay)
        {
            line.startColor = startColour.Evaluate(t);
            line.endColor = endColour.Evaluate(t);

            start = transform.position;

            direction = target.transform.position - start;

            Ray ray = new Ray(start, direction);

            RaycastHit hit;

            Physics.Raycast(ray, out hit, 1000.0f, LayerMask.GetMask("Players", "Walls", "Floors"));

            if (hit.collider != null)
            {
                end = hit.point;
                if (hit.collider.gameObject == target.gameObject)
                {
                    if (durationCurrent < duration)
                        target.controller.statistics["health"].Value = (float)target.controller.statistics["health"].Value - damagePerSecond * Time.deltaTime;
                }
            }
            else
            {
                end = direction * 1000.0f;
            }

            endEffect.transform.position = end;

            line.SetPosition(0, start);
            line.SetPosition(1, end);
        }
        else
        {
            line.startColor = line.endColor = Color.clear;
        }
    }
}
