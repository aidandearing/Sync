using UnityEngine;
using System.Collections;

public class TempoInertiaField : MonoBehaviour
{
    [SerializeField]
    private float fieldStrengthMax = 2;
    [SerializeField]
    private float fieldStrengthMin = 0.1f;
    [SerializeField]
    private float fieldDistortMax = 5;
    [SerializeField]
    private float fieldDistortMin = 1;
    [SerializeField]
    private float fieldSpeed = 1;
    [SerializeField]
    private Renderer distort;
    [SerializeField]
    private float velocityMin = 0;
    [SerializeField]
    private float velocityMax = 20;
    [SerializeField]
    private Controller parent;

    private float fieldStrength;
    private Vector3 lastPosition;
    private float fieldStrengthPercentage;
    private float fieldStrengthVel;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float delta = (transform.position - lastPosition).magnitude / Time.deltaTime;

        fieldStrengthPercentage = Mathf.SmoothDamp(fieldStrengthPercentage, (delta - velocityMin) / (velocityMax - velocityMin), ref fieldStrengthVel, fieldSpeed);

        fieldStrength = Mathf.Lerp(fieldStrengthMin, fieldStrengthMax, fieldStrengthPercentage);

        Collider[] insiders = Physics.OverlapSphere(transform.position, 5);
        foreach (Collider insider in insiders)
        {
            // Controller (player/entity) stuff
            //Controller slowable = insider.gameObject.GetComponent<Controller>();

            //if (slowable != null)
            //{
            //    if (slowable.Faction != parent.Faction)
            //    {
            //        slowable.Timestep = fieldStrength;
            //    }
            //}
            //else
            //{
                // Grenade stuff
                GrenadeBehaviour grenade = insider.gameObject.GetComponent<GrenadeBehaviour>();
                if (grenade != null)
                {
                    if (grenade.parent.Faction != parent.Faction)
                    {
                        grenade.SetTimestep(fieldStrength);
                    }
                }
                else
                {
                    // Projectile stuff
                    ProjectileBehaviour projectile = insider.gameObject.GetComponent<ProjectileBehaviour>();
                    if (projectile != null)
                    {
                        if (projectile.parent.Faction != parent.Faction)
                        {
                            projectile.SetTimestep(fieldStrength);
                        }
                    }
                }
            //}
        }

        lastPosition = transform.position;

        distort.material.shader = Shader.Find("FX/Glass/Stained BumpDistort");
        distort.material.SetFloat("_BumpAmt", Mathf.Lerp(fieldDistortMax, fieldDistortMin, fieldStrengthPercentage));
    }

    public void SetParent(Controller par)
    {
        parent = par;
    }
}
