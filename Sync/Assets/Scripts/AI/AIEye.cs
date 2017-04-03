using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AIEye : AISensor
{
    public float sightDistance = 100.0f;
    public float sightAngle = 180;
    public string sightCriteriaTag = Literals.Strings.Tags.Player;

    public override Transform Sense()
    {
        if (CanSense())
        {
            // Quickest way to do this is just to shoot a sphere cast, get all the hits, and then check their dot product against view angle, which makes a spherical sector of possible hits
            // then fire a ray at each one until one of them returns true for having the sight criteria tag
            // https://en.wikipedia.org/wiki/Spherical_sector
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, sightDistance, transform.forward);

            float angle = Mathf.Sin(Mathf.Deg2Rad * sightAngle / 2);

            foreach (RaycastHit hit in hits)
            {
                // If they pass the tag criteria
                if (hit.collider.gameObject.tag == sightCriteriaTag)
                {
                    // If they pass the dot check
                    if (Vector3.Dot(hit.collider.transform.position, transform.position) > angle)
                    {
                        // Now we shoot a ray at them, and see if it hits them
                        RaycastHit sightHit;

                        Physics.Raycast(new Ray(transform.position, hit.collider.transform.position), out sightHit);

                        if (sightHit.collider.gameObject.tag == sightCriteriaTag)
                        {
                            return sightHit.collider.gameObject.transform;
                        }
                    }
                }
            }
        }

        return null;
    }
}
