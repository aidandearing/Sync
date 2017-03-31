using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Synchronisation/Light Beam Behaviour")]
public class LightBeamBehaviour : MonoBehaviour
{
    public float velocity = 100;
    //public static Transform Goal;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * velocity * Time.deltaTime;
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Goal.position - transform.position), 0.1f);
    }
}
