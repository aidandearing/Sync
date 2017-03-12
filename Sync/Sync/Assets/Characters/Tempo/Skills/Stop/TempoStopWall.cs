using UnityEngine;
using System.Collections;

public class TempoStopWall : MonoBehaviour
{
    [SerializeField]
    private float fieldStrength = 0;

    private Controller parent;

    // Use this for initialization
    void Start()
    {

    }

    public void SetParent(Controller par)
    {
        parent = par;
    }

    void OnTriggerStay(Collider other)
    {
        ProjectileBehaviour projectile = other.GetComponent<ProjectileBehaviour>();
        if (projectile != null)
        {
            if (projectile.parent.Faction != parent.Faction)
            {
                projectile.SetTimestep(fieldStrength);
            }
        }
        else
        {
            GrenadeBehaviour grenade = other.GetComponent<GrenadeBehaviour>();
            if (grenade != null)
            {
                if (grenade.parent.Faction != parent.Faction)
                {
                    grenade.SetTimestep(fieldStrength);
                }
            }
        }
    }
}
