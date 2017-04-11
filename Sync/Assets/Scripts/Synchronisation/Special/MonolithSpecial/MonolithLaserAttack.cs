﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MonolithLaserAttack : MonoBehaviour
{
    [Header("References")]
    public LineRenderer line;
    public GameObject startEffect;
    public GameObject endEffect;
    public PlayerController target;

    [Header("Duration")]
    public float duration = 2.0f;
    public float durationCurrent = 0.0f;
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

    void Update()
    {
        if (target == null)
            target = Blackboard.Global[Literals.Strings.Tags.Player + 1].Value as PlayerController;

        durationCurrent += Time.deltaTime;

        float t = Mathf.Clamp(durationCurrent / duration, 0, 1);

        line.startColor = startColour.Evaluate(t);
        line.endColor = endColour.Evaluate(t);

        direction = target.transform.position - start;// Vector3.MoveTowards(direction, target.transform.position - start, trackingSpeed * Time.deltaTime);

        Ray ray = new Ray(start, direction);

        RaycastHit hit;

        Physics.Raycast(ray, out hit, 1000.0f, LayerMask.GetMask("Players", "Walls", "Floors", "Entities"));

        if (hit.collider != null)
        {
            end = hit.point;// Vector3.MoveTowards(end, hit.point, trackingSpeed * Time.fixedDeltaTime);
            if (hit.collider.gameObject == target.gameObject)
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

        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }
}