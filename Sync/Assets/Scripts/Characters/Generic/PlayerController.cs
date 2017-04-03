﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : Controller
{
    [Header("Camera")]
    new public Camera camera;
    public bool cameraOriented = true;

    // Use this for initialization
    protected override void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            string key = Literals.Strings.Tags.Player + i;
            if (!Blackboard.Global.ContainsKey(key))
                Blackboard.Global.Add(key, new BlackboardValue() { Value = this });
        }

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
        Vector3 v = new Vector3(Input.GetAxis(Literals.Strings.Input.Controller.StickLeftHorizontal), 
                (Input.GetButton(Literals.Strings.Input.Controller.ButtonA) == true) ? 1 : 0, 
                -Input.GetAxis(Literals.Strings.Input.Controller.StickLeftVertical));

        if (v.x != 0 || v.z != 0)
        {
            // When camera orientated the input vector needs to be rotated so that its forward is pointing in the same way as the camera's forward (on the xz plane)
            if (cameraOriented)
            {
                // This should be achievable by first generating a rotation from the input to the camera forward;
                float eulerY = camera.transform.rotation.eulerAngles.y;

                Quaternion rotation = Quaternion.Euler(0, eulerY, 0);
                //if (Vector3.Dot(-camera.transform.forward, new Vector3(0, 0, -1)) < 0.86f)
                //    rotation = Quaternion.FromToRotation(new Vector3(0, 0, 1), -camera.transform.forward);
                //else
                //{
                //    v.Set(-v.x, v.y, v.z);

                //    rotation = Quaternion.FromToRotation(new Vector3(0, 0, -1), camera.transform.forward);
                //}

                float ty = v.y;

                v = rotation * v;
                v.Normalize();

                v.Set(v.x, ty, v.z);
            }

            movement.actionPrimaryLastInputTime = Time.time;
            movement.actionPrimaryLastInputVector = v;
        }

        if (v.y > 0)
        {
            movement.actionSecondaryLastInputTime = Time.time;
            movement.actionSecondaryLastInputVector = v;
        }

        return v;
    }

    protected override void MovementActionPrimaryCallback()
    {
        Vector3 input = HandleMovementInput();

        // This checks to see if the time between an input was last recieved and if the time is shorter than a percentage of the synchronisers duration and this frame
        if (Time.time - movement.actionPrimaryLastInputTime < movement.actionPrimarySynchroniser.Duration * MovementStatistics.FACTOR_OF_DURATION_AS_PADDING_ON_INPUT && input.sqrMagnitude < 1)
        {
            input = movement.actionPrimaryLastInputVector;
        }

        MovementActions.Action(movement.actionPrimary, this, input);
    }

    protected override void MovementActionSecondaryCallback()
    {
        Vector3 input = HandleMovementInput();

        if (Time.time - movement.actionSecondaryLastInputTime < movement.actionSecondarySynchroniser.Duration * MovementStatistics.FACTOR_OF_DURATION_AS_PADDING_ON_INPUT && input.y < 1)
        {
            input = movement.actionSecondaryLastInputVector;
        }

        MovementActions.Action(movement.actionSecondary, this, input);
    }
}