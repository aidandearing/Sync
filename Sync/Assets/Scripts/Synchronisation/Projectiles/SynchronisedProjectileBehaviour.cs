using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SynchronisedProjectileBehaviour : MonoBehaviour
{
    public enum Format { linear, curve };

    [Header("References")]
    new public Rigidbody rigidbody;

    [Header("Controller")]
    public Controller parent;

    [Header("Synchronisation")]
    public Synchronism.Synchronisations synchronisation = Synchronism.Synchronisations.BAR;
    public Synchroniser synchroniser;
    public int duration = 4;
    public int durationCurrent = 0;

    [Header("Trajectory")]
    public Format format = Format.linear;
    public Vector3 direction;
    public Vector3 origin;
    public float speed = 50.0f;
    public float gravity = 9.81f;
    public bool collidesWithCreator = false;
    public bool collidesWithAllies = true;
    public string[] layerMask;

    [Header("Properties")]
    public Property[] alliedProperties;
    public Property[] neutralProperties;
    public Property[] hostileProperties;

    public Ray ray;
    public RaycastHit rayHit;

    void Start()
    {
        rigidbody.MovePosition(origin);
        ray = new Ray(origin, direction);
    }

    void FixedUpdate()
    {
        Physics.Raycast(ray, out rayHit, LayerMask.GetMask(layerMask));

        if (synchroniser == null)
        {
            if (Blackboard.Global.ContainsKey(Literals.Strings.Blackboard.Synchronisation.Synchroniser))
            {
                synchroniser = (Blackboard.Global[Literals.Strings.Blackboard.Synchronisation.Synchroniser].Value as Synchronism).synchronisers[synchronisation];
                synchroniser.RegisterCallback(this, CallbackEnd);
            }
        }

        float p = durationCurrent / (float)duration;

        if (rayHit.distance / (origin + transform.forward * speed * duration * (float)synchroniser.Duration).magnitude < p)
            End();

        rigidbody.MovePosition(origin + transform.forward * speed * (synchroniser.Percent + durationCurrent) * (float)synchroniser.Duration);
        rigidbody.MoveRotation(Quaternion.LookRotation(direction));
    }

    void CallbackEnd()
    {
        if (durationCurrent < duration)
        {
            durationCurrent++;
        }
        else
        {
            End();
        }
    }

    public void End()
    {
        synchroniser.UnregisterCallback(this);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != parent.gameObject)
        {
            Controller target = other.gameObject.GetComponent<Controller>() as Controller;

            if (target != null)
            {
                if (target.faction.isHostile(parent.faction))
                {
                    foreach (Property effect in hostileProperties)
                    {
                        effect.Apply(target);
                    }
                }
                else if (target.faction.isNeutrals(parent.faction))
                {
                    foreach (Property effect in neutralProperties)
                    {
                        effect.Apply(target);
                    }
                }
                else if (target.faction.isAllied(parent.faction))
                {
                    foreach (Property effect in alliedProperties)
                    {
                        effect.Apply(target);
                    }
                }
            }

            End();
        }
    }
}