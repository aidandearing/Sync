using UnityEngine;
using System.Collections;

public class CameraStartScript : MonoBehaviour
{
    public float StartDelay;
    public float EndDelay;
    public float TotalTime;
    public Transform LookTransform;
    public Vector3 Offset;
    public float Velocity;

    public GameObject secondary;

    private Vector3 CurrentVelocity;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(LookTransform.position - transform.position), 0.01f);

        if (Time.time >= StartDelay && Time.time <= EndDelay)
        {
            Vector3 goal = LookTransform.position + Offset;

            Mathf.SmoothDamp(transform.position.x, goal.x, ref CurrentVelocity.x, TotalTime, Mathf.Infinity, Time.deltaTime);
            Mathf.SmoothDamp(transform.position.y, goal.y, ref CurrentVelocity.y, TotalTime, Mathf.Infinity, Time.deltaTime);
            Mathf.SmoothDamp(transform.position.z, goal.z, ref CurrentVelocity.z, TotalTime, Mathf.Infinity, Time.deltaTime);

            transform.position += CurrentVelocity * Time.deltaTime;
        }
        else if (Time.time > EndDelay)
        {
            secondary.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
