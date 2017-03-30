using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CubeController : Controller
{
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!isLocalPlayer)
            return;

        base.Update();
    }

    // Fixed Update is called once per physics step
    protected override void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        base.FixedUpdate();
    }

    protected override Vector3 HandleMovementInput()
    {
        Vector3 input = UnityEngine.Random.insideUnitCircle.normalized;
        input = new Vector3(input.x, 0, input.y);
        return input;
    }
}