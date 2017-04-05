using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AISphereSensor : AISensor
{
    public override bool CanSense(Transform t)
    {
        if ((t.position - transform.position).sqrMagnitude < sensorDistance * sensorDistance)
        {
            return true;
        }

        return false;
    }

    public override Transform Sense()
    {
        if (CanSense())
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, sensorDistance, transform.forward, 0, GetLayerMask());

            foreach (RaycastHit hit in hits)
            {
                // If they pass the tag criteria
                if (hit.collider.gameObject.tag == sensorCriteriaTag)
                {
                    return hit.collider.gameObject.transform;
                }
            }
        }

        return null;
    }

    public override Transform[] SenseAll()
    {
        if (CanSense())
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, sensorDistance, transform.forward, 0, GetLayerMask());

            Transform[] transforms = new Transform[hits.Length];

            int i = 0;
            foreach(RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.tag == sensorCriteriaTag)
                {
                    transforms[i] = hit.collider.transform;
                }
                i++;
            }
        }

        return null;
    }
}
