using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SynchronisedProjectileBehaviour : MonoBehaviour
{
    public enum Format { linear, curve };

    [Header("Controller")]
    public Controller parent;
    public Faction faction;

    [Header("Synchronisation")]
    public Synchronism.Synchronisations synchronisation = Synchronism.Synchronisations.BAR;
    public Synchroniser synchroniser;
    public int duration = 4;
    public int durationCurrent = 0;

    [Header("Trajectory")]
    public Format format = Format.linear;
    public Vector3 direction;
    public Vector3 origin;
    public float radius = 0.1f;
    public float speed = 50.0f;
    public float gravity = 9.81f;

    [Flags]
    public enum CollidesWith { None = 0, Creator = 1, Allies = 2, Neutrals = 4, Enemies = 8, CreatorAllies = 3, CreatorNeutrals = 5, CreatorEnemies = 9, AlliesNeutrals = 6, AlliesEnemies = 10, NeutralsEnemies = 12, CreatorAlliesNeutrals = 7, CreatorAlliesEnemies = 11, CreatorNeutralsEnemies = 13, AlliesNeutralsEnemies = 14, CreatorAlliesNeutralsEnemies = 15 };
    [Header("Collisions")]
    public CollidesWith collidesWith = CollidesWith.AlliesNeutralsEnemies;
    public bool diesOnCollision = true;
    public int diesAfterCollisions = 1;
    public int diesAfterCollisionsCurrent = 0;
    public CollidesWith diesWith = CollidesWith.AlliesNeutralsEnemies;
    public string[] layerMask;

    [Header("Death")]
    public bool isDead = false;
    public float deathDelay = 2.0f;
    public GameObject[] disableOnDeath;

    [Header("Properties")]
    public Property[] alliedProperties;
    public Property[] neutralProperties;
    public Property[] hostileProperties;

    public Ray ray;
    public float lastDistance = 0.0f;
    public RaycastHit rayHit;

    public bool isInitialised = false;

    public void Initialise()
    {
        if (!isInitialised)
        {
            if ((Synchronism)Blackboard.Global[Literals.Strings.Blackboard.Synchronisation.Synchroniser].Value != null)
            {
                isInitialised = true;

                synchroniser = ((Synchronism)Blackboard.Global[Literals.Strings.Blackboard.Synchronisation.Synchroniser].Value).synchronisers[synchronisation];
                synchroniser.RegisterCallback(this, CallbackEnd);
            }
        }
    }

    void Start()
    {
        transform.position = origin;
        transform.rotation = Quaternion.LookRotation(direction);
        ray = new Ray(origin, direction);
    }

    void FixedUpdate()
    {
        if (isDead)
        {

            return;
        }

        if (!isInitialised)
            Initialise();

        float p = (synchroniser.Percent + durationCurrent) * (float)synchroniser.Duration;
        float maxDistance = p * speed;
        // Get all ray cast hits.
        RaycastHit[] hits = Physics.SphereCastAll(ray.origin + ray.direction * lastDistance, radius, ray.direction, maxDistance - lastDistance, LayerMask.GetMask(layerMask));

        Debug.DrawLine(ray.origin + ray.direction * lastDistance, ray.origin + ray.direction * maxDistance);

        transform.position = origin + transform.forward * speed * p;
        transform.rotation = Quaternion.LookRotation(direction);

        faction = parent.faction;

        //if (rayHit != null)
        //    Debug.DrawLine(rayHit.Value.collider.gameObject.transform.position, rayHit.Value.collider.gameObject.transform.position - new Vector3(0, 200, 0), new Color(1,0,0));

        if (hits.Count() > 0)
        {
            // Sort them by distance
            SortedList<float, RaycastHit> sorted = new SortedList<float, RaycastHit>();
            foreach (RaycastHit hit in hits)
            {
                if (!sorted.Keys.Contains(hit.distance))
                    sorted.Add(hit.distance, hit);
            }

            // Now proceed through them
            foreach (KeyValuePair<float, RaycastHit> hit in sorted)
            {
                Debug.Log(hit.Value.collider.gameObject.name);

                //if (hit.Value.distance >= lastDistance && hit.Value.distance < maxDistance)
                //{
                // Make sure they meet the collision criteria
                GameObject parentObject = (parent != null) ? parent.gameObject : null;

                if (hit.Value.collider.gameObject != parentObject || (collidesWith & CollidesWith.Creator) != CollidesWith.None)
                {
                    Controller target = hit.Value.collider.gameObject.GetComponent<Controller>() as Controller;

                    if (target != null)
                    {
                        if (target.faction.isHostile((parent != null) ? parent.faction : faction) && (collidesWith & CollidesWith.Enemies) != CollidesWith.None)
                        {
                            rayHit = hit.Value;
                            Trigger(target);
                        }
                        else if (target.faction.isNeutrals((parent != null) ? parent.faction : faction) && (collidesWith & CollidesWith.Neutrals) != CollidesWith.None)
                        {
                            rayHit = hit.Value;
                            Trigger(target);
                        }
                        else if (target.faction.isAllied((parent != null) ? parent.faction : faction) && (collidesWith & CollidesWith.Allies) != CollidesWith.None)
                        {
                            rayHit = hit.Value;
                            Trigger(target);
                        }
                    }
                    else
                    {
                        rayHit = hit.Value;
                        Trigger(null);
                    }
                }
                //}
            }
        }

        lastDistance = maxDistance;
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
        if (!isDead)
        {
            synchroniser.UnregisterCallback(this);

            isDead = true;

            transform.position = rayHit.point;

            foreach (GameObject disable in disableOnDeath)
            {
                disable.SetActive(false);
            }

            Destroy(gameObject, deathDelay);
        }
    }

    void Trigger(Controller target)
    {
        //transform.position = rayHit.point;

        if (target != null)
        {
            if (target.faction.isHostile(parent.faction) && (collidesWith & CollidesWith.Enemies) != CollidesWith.None)
            {
                foreach (Property effect in hostileProperties)
                {
                    effect.Apply(target);
                }

                if (diesOnCollision && (diesWith & CollidesWith.Enemies) != CollidesWith.None)
                {
                    diesAfterCollisionsCurrent++;

                    if (diesAfterCollisionsCurrent > diesAfterCollisions)
                        End();
                }
            }
            else if (target.faction.isNeutrals(parent.faction) && (collidesWith & CollidesWith.Neutrals) != CollidesWith.None)
            {
                foreach (Property effect in neutralProperties)
                {
                    effect.Apply(target);
                }

                if (diesOnCollision && (diesWith & CollidesWith.Neutrals) != CollidesWith.None)
                {
                    diesAfterCollisionsCurrent++;

                    if (diesAfterCollisionsCurrent > diesAfterCollisions)
                        End();
                }
            }
            else if (target.faction.isAllied(parent.faction) && (collidesWith & CollidesWith.Allies) != CollidesWith.None)
            {
                foreach (Property effect in alliedProperties)
                {
                    effect.Apply(target);
                }

                if (diesOnCollision && (diesWith & CollidesWith.Allies) != CollidesWith.None)
                {
                    diesAfterCollisionsCurrent++;

                    if (diesAfterCollisionsCurrent > diesAfterCollisions)
                        End();
                }
            }

            if (diesOnCollision && (diesWith & CollidesWith.Creator) != CollidesWith.None)
            {
                diesAfterCollisionsCurrent++;

                if (diesAfterCollisionsCurrent > diesAfterCollisions)
                    End();
            }
        }
        else if (diesOnCollision)
        {
            diesAfterCollisionsCurrent++;

            if (diesAfterCollisionsCurrent > diesAfterCollisions)
                End();
        }
    }
}