using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AIEye : AISensor
{
    public float sightAngle = 180;

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Debug.DrawLine(transform.position, transform.position + transform.forward * sensorDistance);
    }

    public override bool CanSense(Transform t)
    {
        // If they pass the tag criteria
        if (transform.gameObject.tag == sensorCriteriaTag)
        {
            float angle = Mathf.Sin(Mathf.Deg2Rad * sightAngle / 2);

            // If they pass the dot check
            if (Vector3.Dot((t.position - transform.position).normalized, transform.forward) > angle)
            {
                // Now we shoot a ray at them, and see if it hits them
                RaycastHit sightHit;
                
                Physics.Raycast(new Ray(transform.position, t.transform.position - transform.position), out sightHit, sensorDistance, GetLayerMask());

                if (sightHit.collider.gameObject.tag == sensorCriteriaTag)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public override Transform Sense()
    {
        if (CanSense())
        {
            Sensed();

            // Quickest way to do this is just to shoot a sphere cast, get all the hits, and then check their dot product against view angle, which makes a spherical sector of possible hits
            // then fire a ray at each one until one of them returns true for having the sight criteria tag
            // https://en.wikipedia.org/wiki/Spherical_sector
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, sensorDistance, transform.forward, GetLayerMask());

            float angle = Mathf.Sin(Mathf.Deg2Rad * sightAngle / 2);

            foreach (RaycastHit hit in hits)
            {
                // If they pass the tag criteria
                if (hit.collider.gameObject.tag == sensorCriteriaTag)
                {
                    // If they pass the dot check
                    if (Vector3.Dot((hit.collider.transform.position - transform.position).normalized, transform.forward) > angle)
                    {
                        // Now we shoot a ray at them, and see if it hits them
                        RaycastHit sightHit;

                        Physics.Raycast(new Ray(transform.position, hit.collider.transform.position - transform.position), out sightHit);

                        if (sightHit.collider.gameObject.tag == sensorCriteriaTag)
                        {
                            Debug.DrawLine(transform.position, hit.point, new Color(0, 1, 0));
                            return sightHit.collider.gameObject.transform;
                        }

                        Debug.DrawLine(transform.position, hit.point, new Color(1, 0, 0));
                    }
                }
            }
        }

        return null;
    }

    public override Transform[] SenseAll()
    {
        if (CanSense())
        {
            Sensed();

            // Quickest way to do this is just to shoot a sphere cast, get all the hits, and then check their dot product against view angle, which makes a spherical sector of possible hits
            // then fire a ray at each one and add each that hits to a list
            // https://en.wikipedia.org/wiki/Spherical_sector
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, sensorDistance, transform.forward, GetLayerMask());

            float angle = Mathf.Sin(Mathf.Deg2Rad * sightAngle / 2);

            List<Transform> transforms = new List<Transform>();

            foreach (RaycastHit hit in hits)
            {
                // If they pass the tag criteria
                if (hit.collider.gameObject.tag == sensorCriteriaTag)
                {
                    // If they pass the dot check
                    if (Vector3.Dot((hit.collider.transform.position - transform.position).normalized, transform.forward) > angle)
                    {
                        // Now we shoot a ray at them, and see if it hits them
                        RaycastHit sightHit;

                        Physics.Raycast(new Ray(transform.position, hit.collider.transform.position - transform.position), out sightHit);

                        if (sightHit.collider.gameObject.tag == sensorCriteriaTag)
                        {
                            transforms.Add(sightHit.collider.gameObject.transform);
                        }
                    }
                }// if hit.collider.gameObject.tag == sightCriteriaTag
            }// foreach

            return transforms.ToArray();
        }

        return null;
    }
}
