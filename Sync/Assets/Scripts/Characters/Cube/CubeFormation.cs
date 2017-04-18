using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CubeFormation : MonoBehaviour
{
    // TODO REMOVE THIS WHEN BACK TO NETWORKING, and make this class a NetworkBehaviour
    public bool isLocalPlayer = true;

    public static float FormationGridSize = 3.0f;
    public static int FormationLimit = 27;
    public static float FormationSpeed = 5.0f;
    public static float FormationTurnAngleMin = 45.0f;
    public static float FormationTurnAngleMax = 90.0f;
    public static float FormationTurnAngleSnap = 15.0f;
    public static float FormationTurnDelay = 3;
    public static float FormationTurnDelayDelta = 2;
    public static float FormationGroundHeight = 10.0f;
    public static float FormationGatheringRange = 30.0f;
    public static float FormationGatheringHeight = 10.0f;

    public static float EyeDuration = 10.0f;
    public static float EyeDurationDelta = 5.0f;

    [Header("Controller")]
    public Controller controller;

    [Header("Synchronisation")]
    public Synchronism.Synchronisations synchronisation = Synchronism.Synchronisations.BAR_2;
    public Synchroniser synchroniser;

    [Header("Formation")]
    new public BoxCollider collider;
    public List<CubeController> children = new List<CubeController>();

    private float formationTurnDelay;
    private Quaternion formationTurnDesired;

    [Header("AI")]
    public AISphereSensor sensor;

    [Header("Targeting")]
    public PlayerController target;

    public bool isInitialised = false;

    public void Initialise()
    {
        if (!isInitialised)
        {
            isInitialised = true;

            Synchronism synch = ((Synchronism)Blackboard.Global[Literals.Strings.Blackboard.Synchronisation.Synchroniser].Value);
            if (synch != null)
            {
                synchroniser = synch.synchronisers[synchronisation];
                synchroniser.RegisterCallback(this, Callback);
            }
        }
    }

    public void Callback()
    {
        if (target != null)
        {
            if ((target.transform.position - transform.position).sqrMagnitude < 200 * 200)
            {
                int v = Mathf.RoundToInt(UnityEngine.Random.value * (children.Count - 1));
                children[v].target = target;
                children[v].Attack();
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  MonoBehaviour
    //
    ///////////////////////////////////////////////////////////////////////////////////////////////////

    // Use this for initialization
    void Start()
    {
        controller.movement.speedForward = FormationSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;
    }

    // Fixed Update is called once per physics step
    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        if (!isInitialised)
            Initialise();

        if (children.Count < 1)
            Destroy(gameObject);

        // Formations follow a seek and destroy principle
        // They wander around, a specified height above the ground, focusing on a gather point from the blackboard
        // Until they spot a target
        // Then they move in as quickly as they can
        // And they position themselves directly above the target
        // Then they begin firing down upon the target

        RaycastHit hit;
        Physics.Raycast(transform.position, new Vector3(0, -1, 0), out hit, 100.0f, LayerMask.GetMask(Literals.Strings.Physics.Layers.Floors));

        if ((hit.point - transform.position).sqrMagnitude < FormationGroundHeight * FormationGroundHeight)
        {
            controller.rigidbody.AddForce(new Vector3(0, 1, 0), ForceMode.Force);
            //input += new Vector3(0, 1, 0);
        }
        else
        {
            controller.rigidbody.AddForce(new Vector3(0, -1, 0), ForceMode.Force);
        }

        if (target == null)
        {
            if (formationTurnDelay > 0)
                formationTurnDelay -= Time.fixedDeltaTime;
            else
            {
                formationTurnDelay += UnityEngine.Random.Range(FormationTurnDelay - FormationTurnDelayDelta, FormationTurnDelay + FormationTurnDelayDelta);

                Vector3 position = UnityEngine.Random.insideUnitCircle.normalized * FormationGatheringRange;
                position = new Vector3(position.x, FormationGatheringHeight, position.y) + (Vector3)Blackboard.Global[Literals.Strings.Blackboard.Locations.CubeGatheringPointOne].Value;

                formationTurnDesired = Quaternion.LookRotation(position - transform.position);
            }

            MovementActions.Fly(controller, transform.forward, MovementActions.Move.Move);
            controller.rigidbody.MoveRotation(Quaternion.RotateTowards(controller.rigidbody.rotation, formationTurnDesired, (float)controller.statistics[Literals.Strings.Blackboard.Movement.SpeedTurn].Value * Time.fixedDeltaTime));

            Transform t = sensor.Sense();

            if (t != null)
                target = t.gameObject.GetComponent<PlayerController>();
        }
        else
        {
            MovementActions.Fly(controller, transform.forward, MovementActions.Move.Move);
            Vector3 position = target.transform.position + new Vector3(0, FormationGatheringHeight, 0);
            formationTurnDesired = Quaternion.LookRotation(position - transform.position);
            controller.rigidbody.MoveRotation(Quaternion.RotateTowards(controller.rigidbody.rotation, formationTurnDesired, controller.movement.speedTurn * Time.fixedDeltaTime));
        }
    }

    Vector3 HandleMovementInput()
    {
        //Vector3 input = UnityEngine.Random.insideUnitCircle.normalized;
        //input = new Vector3(input.x, 0, input.y);
        //return input;

        return transform.forward;
    }

    /*
    private void PickEye()
    {
        // One cube from the forward layer is picked as the 'eye' of the formation
        // Every couple of seconds this 'eye' is switched out for a different front layer cube

        if (eyeDuration > 0)
        {
            eyeDuration -= Time.fixedDeltaTime;
        }
        else
        {
            eyeDuration += UnityEngine.Random.Range(EyeDuration - EyeDurationDelta, EyeDuration + EyeDurationDelta);

            // The forward layer is +z
            // Given how the formation is constructed it is easy to get the indices of all the cubes in front

            // It really isn't, as the formation is mathematically derived, which means I have to determine which cubes are front most
            // There should be a relation between the dimensions and the current number of cubes, such that the front most layer of cubes indices can be calculated
            // Let me say all my knowns
            // I know the dimension, it is the ceiling of the cubed root of the number of cubes in formation
            // I know how many cubes are in formation
            // I can easily calculate how many cubes are needed to fill the formation
            // The criteria then is that the valid forward indices are all indices whos remander after division with the dimension is dimension - 1
            // This puts them at the front
            // As any index that would put them at the back will return 0
            // And any other position will return a decreasing number from 0 to dimension - 1

            // So ultimately I have decided to just rip through the formation list and check all their indices
            List<int> indices = new List<int>();

            // Calculate the dimension
            int dimension = GetFormationDimension();

            // Go through the children
            for (int i = 0; i < children.Count; i++)
            {
                // Check if their are a valid forward facing cube
                if (i % dimension == dimension - 1)
                {
                    // Add them to the list of possible eyes if they are
                    indices.Add(i);
                }
            }

            // Then pick one at random and go.
            eye = children[indices[(int)(UnityEngine.Random.value * (indices.Count - 1.0f))]];
        }
    }
    */

    ///////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Formation Logic
    //
    ///////////////////////////////////////////////////////////////////////////////////////////////////

    public void AddChild(CubeController child)
    {
        this.children.Add(child);
        child.SetParent(this);

        if (GetFormationDimension() > GetFormationDimension(children.Count - 1))
        {
            foreach (CubeController c in children)
            {
                c.SetFormationPoint();
            }

            SetFormationCollider();
        }
        else
        {
            child.SetFormationPoint();
        }
    }

    public void RemoveChild(CubeController child)
    {
        this.children.Remove(child);
        child.parent = null;

        if (child.transform.parent = transform.parent)
        {
            child.transform.SetParent(null);
        }

        if (GetFormationDimension() < GetFormationDimension(children.Count + 1))
        {
            foreach (CubeController c in children)
            {
                c.SetFormationPoint();
            }

            SetFormationCollider();
        }

        //if (child == eye)
        //{
        //    eyeDuration = 0;
        //    PickEye();
        //}

        if (children.Count == 0)
        {
            synchroniser.UnregisterCallback(this);
            Destroy(gameObject);
        }
    }

    public Vector3 GetFormationLocation(CubeController child)
    {
        int index = children.IndexOf(child) + 1;

        int dimension = GetFormationDimension();

        Vector3 location = new Vector3(((index / dimension) / dimension) % dimension,
            (index / dimension) % dimension,
            -index % dimension) * FormationGridSize;
        return location - new Vector3(1, 1, -1) * dimension;
    }

    public int GetFormationDimension()
    {
        return (int)(Mathf.Ceil(Mathf.Pow(children.Count + 1, 0.333f)));
    }

    public int GetFormationDimension(int count)
    {
        return (int)(Mathf.Ceil(Mathf.Pow(count, 0.333f)));
    }

    public void SetFormationCollider()
    {
        collider.size = Vector3.one * GetFormationDimension() * FormationGridSize;
        //collider.center = collider.size / 2.0f - (Vector3.one * FormationGridSize / 2);
    }
}
