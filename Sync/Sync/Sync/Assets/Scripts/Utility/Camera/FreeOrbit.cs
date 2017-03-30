using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[AddComponentMenu("Scripts/Utility/Camera/FreeOrbit")]
public class FreeOrbit : MonoBehaviour
{
    public static bool invertY = true;

    public enum UpdateMode { Update, Fixed };
    public UpdateMode update;

    public enum Mode { Mouse, Gamepad };
    public Mode mode = Mode.Gamepad;

    [Tooltip("Having soft on causes the object to move towards the desired orbit location, instead of immediately being there.")]
    public bool isSoft = false;
    [Tooltip("The speed of the soft transition, this is exponential, smaller numbers will have higher speed")]
    public float softSpeed = 1;
    private Vector3 softVelocity;

    public Transform target;
    [Tooltip("The focal point the orbiting object will orient its forward towards, as an offset from the target")]
    public Vector3 targetOffset = new Vector3(0, 2, 0);
    [Tooltip("The offset from the target transform this object will orbit")]
    public Vector3[] orbitOffsets;
    [Tooltip("The speed in degrees per second this object will be moved at around its target transform")]
    public float orbitSpeed = 180;
    [Tooltip("The number of percentage points per second the transition across orbit offsets will be performed (0 - 1)")]
    public float orbitTransitionSpeed = 0.25f;

    private float orbitAngle;
    private float orbitTransition;
    private Vector3 lastMousePosition;

    void Start()
    {

    }

    void Update()
    {
        if (update == UpdateMode.Update)
        {
            Orbit();
        }
    }

    void FixedUpdate()
    {
        if (update == UpdateMode.Fixed)
        {
            Orbit();
        }
    }

    void Orbit()
    {
        // Rotation is based on a plane, where the horizontal input data is used to rotate the object along its orbit
        // And verticle input data is used to rotate the pitch of the object
        Vector3 input = Vector2.zero;

        switch (mode)
        {
            case Mode.Gamepad:
                input = new Vector3(-Input.GetAxis(Literals.Strings.Input.Controller.StickRightHorizontal), Input.GetAxis(Literals.Strings.Input.Controller.StickRightVertical), 0);
                break;
            case Mode.Mouse:
                input = Input.mousePosition - lastMousePosition;

                lastMousePosition = Input.mousePosition;
                break;
        }

        float orbitDelta = orbitSpeed * input.x * Time.fixedDeltaTime;
        orbitAngle += orbitDelta;

        float orbitTransitionDelta = orbitTransitionSpeed * input.y * Time.fixedDeltaTime;

        if (invertY)
            orbitTransitionDelta *= -1;

        orbitTransition = Mathf.Clamp(orbitTransition + orbitTransitionDelta, 0.001f, 0.999f);

        float orbitOffsetCount = orbitOffsets.Length - 1;
        int lower = Mathf.FloorToInt(orbitTransition * orbitOffsetCount);
        int upper = Mathf.CeilToInt(orbitTransition * orbitOffsetCount);

        float percentageAtLower = lower / orbitOffsetCount;
        float percentageAtUpper = upper / orbitOffsetCount;
        float percentageBetween = (orbitTransition - percentageAtLower) / (percentageAtUpper - percentageAtLower);

        Vector3 orbitOffset = Vector3.Slerp(orbitOffsets[lower], orbitOffsets[upper], percentageBetween);

        float orbitSin = Mathf.Sin(Mathf.Deg2Rad * orbitAngle);
        float orbitCos = Mathf.Cos(Mathf.Deg2Rad * orbitAngle);
        float offsetX = orbitOffset.z * orbitCos;
        float offsetZ = orbitOffset.z * orbitSin;

        if (isSoft)
        {
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(target.position.x + offsetX, target.position.y + orbitOffset.y, target.position.z + offsetZ), ref softVelocity, softSpeed);
        }
        else
        {
            transform.position = new Vector3(target.position.x + offsetX, target.position.y + orbitOffset.y, target.position.z + offsetZ);
        }

        transform.LookAt(target.transform.position + targetOffset);
    }
}
