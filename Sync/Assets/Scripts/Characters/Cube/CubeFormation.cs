using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CubeFormation : Controller
{
    public static float FormationGridSize = 3.0f;
    public static int FormationLimit = 26;
    public static float FormationSpeed = 5.0f;
    public static float FormationTurnAngleMin = 45.0f;
    public static float FormationTurnAngleMax = 90.0f;
    public static float FormationTurnAngleSnap = 15.0f;
    public static float FormationTurnDelay = 3;
    public static float FormationTurnDelayDelta = 2;
    public static float FormationGroundHeight = 10.0f;

    public static float EyeDuration = 10.0f;
    public static float EyeDurationDelta = 5.0f;

    new public BoxCollider collider;
    public List<CubeController> children = new List<CubeController>();

    private float formationTurnDelay;
    private Quaternion formationTurnDesired;

    public CubeController eye;
    private float eyeDuration;

    public PlayerController target;

    ///////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  MonoBehaviour
    //
    ///////////////////////////////////////////////////////////////////////////////////////////////////

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        movement.speedForward = FormationSpeed;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!isLocalPlayer)
            return;

        base.Update();
    }

    // Fixed Update is called once per physics step
    protected override void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        // Formations follow a seek and destroy principle
        // They wander around in straight lines broken by turns of random degrees
        // Until they spot a target
        // Then they move in as quickly as they can
        // And they position themselves directly above the target
        // Then they begin firing down upon the target

        RaycastHit hit;
        Physics.Raycast(transform.position, new Vector3(0, -1, 0), out hit, 100.0f, LayerMask.GetMask(Literals.Strings.Physics.Layers.Floors));

        if ((hit.point - transform.position).sqrMagnitude < FormationGroundHeight * FormationGroundHeight)
        {
            rigidbody.AddForce(new Vector3(0, 1, 0), ForceMode.Force);
            //input += new Vector3(0, 1, 0);
        }
        else
        {
            rigidbody.AddForce(new Vector3(0, -1, 0), ForceMode.Force);
        }

        if (target == null)
        {
            if (formationTurnDelay > 0)
                formationTurnDelay -= Time.fixedDeltaTime;
            else
            {
                formationTurnDelay += UnityEngine.Random.Range(FormationTurnDelay - FormationTurnDelayDelta, FormationTurnDelay + FormationTurnDelayDelta);

                float turnInverter = (UnityEngine.Random.value <= 0.5) ? -1 : 1;
                float turnRad = Mathf.Deg2Rad * Mathf.Floor(UnityEngine.Random.Range(FormationTurnAngleMin, FormationTurnAngleMax) / FormationTurnAngleSnap) * FormationTurnAngleSnap * turnInverter;
                turnRad += Mathf.Atan2(transform.forward.z, transform.forward.x);
                formationTurnDesired = Quaternion.LookRotation(new Vector3(Mathf.Cos(turnRad), 0, Mathf.Sin(turnRad)));
            }

            MovementActions.Fly(this, transform.forward);
            rigidbody.MoveRotation(Quaternion.RotateTowards(rigidbody.rotation, formationTurnDesired, movement.speedTurn * Time.fixedDeltaTime));

            // Until a player is spotted, the formation wanders
            // If eye sight is lost, the position the player was last scene at is stored, and the formation moves to that location
            // The position slowly expands to cover a large radius that the formation attempts to radially scan

            if (eye != null)
            {
                Transform spottedTarget = eye.eye.Sense();

                if (spottedTarget != null)
                    target = spottedTarget.gameObject.GetComponent<PlayerController>();
            }
            else
            {
                PickEye();
            }
        }
        else
        {

        }

        base.FixedUpdate();
    }

    protected override Vector3 HandleMovementInput()
    {
        //Vector3 input = UnityEngine.Random.insideUnitCircle.normalized;
        //input = new Vector3(input.x, 0, input.y);
        //return input;

        return transform.forward;
    }

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

            // It really isn't, as the formation is loose, and fills back to front, which means I have to determine which cubes are front most
            // There should be a mathematical relation between the dimensions and the current number of cubes, such that the front most layer of cubes indices can be calculated
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

        if (child == eye)
        {
            eyeDuration = 0;
            PickEye();
        }
    }

    public Vector3 GetFormationLocation(CubeController child)
    {
        int index = children.IndexOf(child) + 1;

        int dimension = GetFormationDimension();

        Vector3 location = new Vector3(((index / dimension) / dimension) % dimension,
            (index / dimension) % dimension,
            -index % dimension) * FormationGridSize;
        return location - Vector3.one * dimension / 2;
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
