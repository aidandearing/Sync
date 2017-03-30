using UnityEngine;
using System.Collections;

public class MoveForward : MonoBehaviour
{
    public float Velocity = 1000;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward * Velocity, Velocity * Time.deltaTime);
    }
}
