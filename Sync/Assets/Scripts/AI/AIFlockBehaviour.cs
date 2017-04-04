using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AIFlockBehaviour : MonoBehaviour
{
    [Header("Alignment")]
    public float alignmentDistance = 5.0f;
    public float alignmentFactor = 0.01f;

    [Header("Cohesion")]
    public float cohesionDistance = 5.0f;
    public float cohesionForce = 5.0f;
    public ForceMode cohesionForceMode = ForceMode.Force;

    [Header("Seperation")]
    public float seperationDistance = 2.5f;
    public float seperationForce = 10.0f;
    public ForceMode seperationForceMode = ForceMode.Force;

    [Header("Management")]
    public float checkCooldown = 1.0f;
    private float checkCooldownCurrent = 1.0f;
    public float checkCooldownDelta = 0.5f;
    public int layerMask;

    public List<Transform> flock = new List<Transform>();

    private List<Transform> flockA = new List<Transform>();
    private List<Transform> flockC = new List<Transform>();
    private List<Transform> flockS = new List<Transform>();

    public string tagCriteria;

    public Controller controller;

    public void Start()
    {
        gameObject.GetComponent<Controller>();
    }

    public void FixedUpdate()
    {
        if (checkCooldownCurrent > 0)
            checkCooldownCurrent -= Time.deltaTime;
        else
        {
            flock = new List<Transform>();
            flockA = new List<Transform>();
            flockC = new List<Transform>();
            flockS = new List<Transform>();

            checkCooldownCurrent += UnityEngine.Random.Range(checkCooldown - checkCooldownDelta, checkCooldown + checkCooldownDelta);

            float maxDistance = Mathf.Max(Mathf.Max(alignmentDistance, cohesionDistance), seperationDistance);

            RaycastHit[] hits = Physics.SphereCastAll(transform.position, maxDistance, transform.forward, layerMask);

            foreach(RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.tag == tagCriteria)
                {
                    Transform member = hit.collider.gameObject.transform;
                    flock.Add(member);

                    float sqrDist = (hit.collider.gameObject.transform.position - transform.position).sqrMagnitude;

                    if (sqrDist <= alignmentDistance * alignmentDistance)
                    {
                        flockA.Add(member);
                    }

                    if (sqrDist <= cohesionDistance * cohesionDistance)
                    {
                        flockC.Add(member);
                    }

                    if (sqrDist <= seperationDistance * seperationDistance)
                    {
                        flockS.Add(member);
                    }
                }
            }
        }

        if (flock.Count > 0)
        {
            // Alignment Logic
            // Go through the flockA list, which contains all the flock transforms that are within the alignment distance
            // And calculate the average alignment of all flock members
            // then apply some factor of the average alignment of all flock members alignments
            if (flockA.Count > 0)
            {
                Vector3 flockAlignment = transform.forward;

                foreach (Transform member in flockA)
                {
                    flockAlignment += member.forward;
                }
                flockAlignment /= flockA.Count + 1;

                controller.rigidbody.MoveRotation(Quaternion.LookRotation(Vector3.Lerp(transform.forward, flockAlignment, alignmentFactor)));

                Debug.DrawRay(transform.position, transform.forward, new Color(1, 1, 1));
                Debug.DrawRay(transform.position, flockAlignment, new Color(0, 0, 1));
            }

            // Cohesion Logic
            // Go through the flockC list, which contains all the flock transforms that are within the alignment distance
            // And calculate the center of mass of all flock members
            // then apply a force towards the center of mass of all flock members
            if (flockC.Count > 0)
            {
                Vector3 com = transform.position;

                foreach (Transform member in flockC)
                {
                    com += member.position;
                }
                com /= flockC.Count + 1;

                Vector3 cohesiveForce = (com - transform.position).normalized * cohesionForce;
                controller.rigidbody.AddForce(cohesiveForce, cohesionForceMode);
                Debug.DrawRay(transform.position, cohesiveForce, new Color(0, 1, 0));
            }

            // Seperation Logic
            // Go through the flockS list, which contains all the flock transforms that are within the seperation distance
            // And apply a force away from each flock member
            if (flockS.Count > 0)
            {
                foreach (Transform member in flockS)
                {
                    Vector3 seperativeForce = (transform.position - member.position).normalized * seperationForce;
                    controller.rigidbody.AddForce(seperativeForce, seperationForceMode);
                    Debug.DrawRay(transform.position, seperativeForce, new Color(0, 1, 0));
                }
            }
        }
    }
}
