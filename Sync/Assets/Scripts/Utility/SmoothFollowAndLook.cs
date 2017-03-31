using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("Scripts/Utility/Smooth Follow and Look")]
public class SmoothFollowAndLook : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Vector3[] offset;
    [SerializeField]
    private int maxSteps;
    [SerializeField]
    private float speed;

    private Vector3 currentVelocity;
    private int currentStep;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float percent = (float)currentStep / (float)maxSteps;

        Vector3 goal = target.position + target.TransformDirection(Vector3.Lerp(offset[(int)Mathf.Clamp((offset.Length - 1) * percent, 0, offset.Length - 1)], offset[(int)Mathf.Clamp((offset.Length - 1) * percent + 1, 0, offset.Length - 1)], (offset.Length - 1) * percent - Mathf.Floor((offset.Length - 1) * percent)));

        transform.position = Vector3.SmoothDamp(transform.position, goal, ref currentVelocity, speed);

        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (currentStep < maxSteps)
                currentStep++;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (currentStep > 0)
                currentStep--;
        }
    }
}
