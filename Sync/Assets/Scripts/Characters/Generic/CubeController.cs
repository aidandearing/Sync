using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CubeController : Controller
{
    public static float CohesionDistanceDefault = 10.0f;
    public static float CohesionDistanceChild = 0.0f;
    public static float CohesionDistanceLoner = 25.0f;
    public static float SeperationDistanceDefault = 2.5f;
    public static float SeperationDistanceChild = 2.5f;
    public static float SeperationDistanceLoner = 10.0f;
    public static float AlignmentDistanceDefault = 25.0f;
    public static float AlignmentDistanceChild = 0.0f;
    public static float AlignmentDistanceLoner = 25.0f;

    public static float PlayerCheckRadius = 50.0f;
    public static float PlayerCheckTime = 5.0f;
    public static float PlayerMemoryTime = 120.0f;

    public static float FormationCheckRadius = 5.0f;
    public static float FormationCheckTime = 1.0f;
    public static float FormationCheckDelay = 10.0f;
    public static float FormationCheckDelayDelta = 2.5f;
    public static float FormationBreakChance = 0.01f;
    public static int   FormationLimit = 64;
    public static float FormationSpeed = 5.0f;
    public static float FormationJoinSpeed = 7.5f;

    public static float LonerSpeedMin = 2.0f;
    public static float LonerSpeedMax = 10.0f;

    public float formationCheckCurrent = 0.0f;
    public Vector3 formationPosition = Vector3.zero;
    public bool willJoinFormation = true;

    public List<CubeController> children = new List<CubeController>();
    public CubeController parent;

    public AIEye eye;

    public PlayerController target;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        FormationBreakCheck();
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

        //float cohesionDistance = 

        if (formationCheckCurrent > 0)
            formationCheckCurrent -= Time.fixedDeltaTime;

        // This means it is a single unit, or it is acting as a cluster, either way, only those that are parentless should be updating
        if (parent == null)
        {
            // This means it is a single unit
            // Single units are loners, they will fly with cohesion, but their desired seperation is high, and their alignment low
            if (children.Count < 1)
            {
                
            }
            // This is a cluster
            // Clusters have maximum cohesion, after the children make sure they reach their formation point, then become part of the hierarchy under their parent, as each child is a sub transform
            // Therefore only the cluster leader needs to update movement logic
            else
            {
                
            }
        }
        // This is a child, and it needs to make sure if it isn't transform parented to its parent that it moves to its formation point and becomes transform parented
        else
        {
            // This child is not parented yet, and needs to get to the formation point
            if (transform.parent != parent.transform)
            {
                MovementActions.Fly(this, ((Vector3)(parent.transform.localToWorldMatrix * formationPosition) - transform.position).normalized);
            }
        }

        base.FixedUpdate();
    }

    protected override Vector3 HandleMovementInput()
    {
        Vector3 input = UnityEngine.Random.insideUnitCircle.normalized;
        input = new Vector3(input.x, 0, input.y);
        return input;
    }

    public void SetParent(CubeController parent)
    {
        this.parent = parent;
    }

    public void AddChild(CubeController child)
    {
        this.children.Add(child);
        child.SetParent(this);

        if (GetFormationDimension() > GetFormationDimension(children.Count - 1))
        {
            foreach(CubeController c in children)
            {
                c.SetFormationPoint();
            }
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
    }

    public Vector3 GetLocalLocation(CubeController child)
    {
        int index = children.IndexOf(child);

        int dimension = GetFormationDimension();

        return new Vector3(((index / dimension) / dimension) % dimension, 
            (index / dimension) % dimension, 
            index % dimension);
    }

    public int GetFormationDimension()
    {
        return Mathf.CeilToInt(Mathf.Pow(children.Count, 1.0f / 3.0f));
    }

    public int GetFormationDimension(int count)
    {
        return Mathf.CeilToInt(Mathf.Pow(count, 1.0f / 3.0f));
    }

    public void SetFormationPoint()
    {
        formationPosition = parent.GetLocalLocation(this);
        transform.SetParent(null);
    }

    /// <summary>
    /// Checks to see if this cube should join a formation
    /// </summary>
    public void FormationCheck()
    {
        // Has the count down gotten to 0, this means it is time to do a new formation check
        if (formationCheckCurrent <= 0)
        {
            // If this cube wants to join formations
            if (willJoinFormation)
            {
                // If this cube isn't in a formation they should look for one
                if (parent == null)
                {
                    RaycastHit[] hits = Physics.SphereCastAll(transform.position, FormationCheckRadius, transform.forward);

                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.collider.gameObject.tag == Literals.Strings.Tags.Cube)
                        {
                            CubeController cube = hit.collider.gameObject.GetComponent<CubeController>();

                            // This cube has found a cube in a formation
                            if (cube.parent != null)
                            {
                                cube.parent.AddChild(this);

                            }
                        }
                    }
                }
                // Else this cube is already in a formation and should check if it wants to leave it (eventually all cubes will leave formation, at 1% loss every second, that is approximately 5 minutes or so)
                else
                {
                    FormationBreakCheck();
                }
            }

            formationCheckCurrent += UnityEngine.Random.Range(FormationCheckDelay - FormationCheckDelayDelta, FormationCheckDelay + FormationCheckDelayDelta);
        }
    }

    public void FormationBreakCheck()
    {
        if (UnityEngine.Random.value < FormationBreakChance)
        {
            if (willJoinFormation)
            {
                willJoinFormation = false;

                movement.speedForward = UnityEngine.Random.Range(LonerSpeedMin, LonerSpeedMax);
            }
        }
    }
}