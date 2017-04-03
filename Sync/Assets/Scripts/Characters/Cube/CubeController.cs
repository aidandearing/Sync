using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CubeController : Controller
{
    public static float AlignmentDistanceDefault = 25.0f;
    public static float AlignmentDistanceChild = 0.0f;
    public static float AlignmentDistanceLoner = 25.0f;
    public static float CohesionDistanceDefault = 100.0f;
    public static float CohesionDistanceChild = 0.0f;
    public static float CohesionDistanceLoner = 100.0f;
    public static float SeperationDistanceDefault = 2.5f;
    public static float SeperationDistanceChild = 2.5f;
    public static float SeperationDistanceLoner = 10.0f;

    public static float PlayerCheckRadius = 50.0f;
    public static float PlayerCheckTime = 5.0f;
    public static float PlayerMemoryTime = 120.0f;

    public static float FormationSnapAngle = 10.0f;
    public static float FormationGridSize = 3.0f;
    public static float FormationCheckRadius = 100.0f;
    public static float FormationCheckTime = 1.0f;
    public static float FormationCheckDelay = 10.0f;
    public static float FormationCheckDelayDelta = 2.5f;
    public static float FormationBreakChance = 0.01f;
    public static int FormationLimit = 26;
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
    public AIFlockBehaviour flocking;

    public Transform[] targets;
    public PlayerController target;

    public enum State { Loner, Child, ParentedChild, Cluster, Wanderer, Attacker, Skulker, Orchestra };
    public State state = State.Wanderer;

    new public BoxCollider collider;

    ///////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  MonoBehaviour
    //
    ///////////////////////////////////////////////////////////////////////////////////////////////////

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

        flocking.alignmentDistance = AlignmentDistanceDefault;
        flocking.cohesionDistance = CohesionDistanceDefault;
        flocking.seperationDistance = SeperationDistanceDefault;

        if (formationCheckCurrent > 0)
            formationCheckCurrent -= Time.fixedDeltaTime;

        // This means it is a single unit, or it is acting as a cluster, either way, only those that are parentless should be updating
        if (parent == null)
        {
            // This means it is a single unit
            // Single units are loners, they will fly with cohesion, but their desired seperation is high, and their alignment low
            if (children.Count < 1)
            {
                // Loner behaviour is done only for those that don't like to be in formation
                if (!willJoinFormation)
                {
                    state = State.Loner;
                }
                // Otherwise they are wanderers
                else
                {
                    state = State.Wanderer;
                }
            }
            // This is a cluster
            // Clusters have maximum cohesion, after the children make sure they reach their formation point, then become part of the hierarchy under their parent, as each child is a sub transform
            // Therefore only the cluster leader needs to update movement logic
            else
            {
                state = State.Cluster;
            }
        }
        // This is a child
        else
        {
            // This child is not parented yet
            if (transform.parent != parent.transform)
            {
                state = State.Child;
            }
            // This child is parented
            else
            {
                state = State.ParentedChild;
            }
        }

        switch (state)
        {
            case State.Loner: Loner(); break;
            case State.Child: Child(); break;
            case State.ParentedChild: ParentedChild(); break;
            case State.Cluster: Cluster(); break;
            case State.Skulker: Skulker(); break;
            case State.Wanderer: Wanderer(); break;
            case State.Attacker: Attacker(); break;
            case State.Orchestra: Orchestra(); break;
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

    ///////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  States
    //
    ///////////////////////////////////////////////////////////////////////////////////////////////////

    private void Loner()
    {
        // Loners can only transition to skulkers

        // Possible states from Loner
        // Skulker

        // Loner behaviour is done only for those that don't like to be in formation
        flocking.alignmentDistance = AlignmentDistanceLoner;
        flocking.cohesionDistance = CohesionDistanceLoner;
        flocking.seperationDistance = SeperationDistanceLoner;

        // Loners like to fly high and look for the player, then when they see them they like to come down to player level somewhere far away from the spotted player, if they see the player they move away, but do not use pathfinding

    }

    private void Child()
    {
        // Children attempt to get to formation
        
        // Possible states from Child
        // ParentedChild

        // This child is not parented yet, and needs to get to the formation point
        // Calculate the formation point
        Vector3 worldPosFormation = parent.transform.TransformPoint(formationPosition);

        // If the child is close enough to the formation point
        if ((transform.position - worldPosFormation).magnitude < 1)
        {
            // Snap them to it, and keep them here
            rigidbody.MovePosition(worldPosFormation);
            // Start rotating them so they look the same way their parent does
            rigidbody.MoveRotation(Quaternion.RotateTowards(rigidbody.rotation, Quaternion.LookRotation(parent.transform.forward), movement.speedTurn * Time.fixedDeltaTime));
            // If they look within a threshold angle of the same way then snap their rotation
            if (Vector3.Dot(transform.forward, parent.transform.forward) > Mathf.Sin(Mathf.Deg2Rad * FormationSnapAngle))
            {
                // Snap rotation
                rigidbody.MoveRotation(parent.transform.rotation);
                // Make this child transform a child of parent transform (make sure they move together without me further having to waste time on calculating that myself)
                transform.SetParent(parent.transform);
                // And finally disable any capacity for them to recieve force, which disables any other attempts at moving them out of formation
                //rigidbody.isKinematic = true;
                state = State.ParentedChild;
            }
        }
        // If they aren't move them towards it
        else
        {
            movement.speedForward = FormationJoinSpeed;

            MovementActions.Fly(this, (worldPosFormation - transform.position).normalized);

            Debug.DrawLine(worldPosFormation, transform.position);
            Debug.DrawRay(transform.position, (worldPosFormation - transform.position).normalized, new Color(1, 0, 1));

            flocking.alignmentDistance = AlignmentDistanceChild;
            flocking.cohesionDistance = CohesionDistanceChild;
            flocking.seperationDistance = SeperationDistanceChild;
        }
    }

    private void ParentedChild()
    {
        // This child is parented, and should have a chance of gaining independance
        // Formation check automatically does this, for parented cubes
        FormationCheck();
        // Snap them to their formation position, and keep them there
        rigidbody.MovePosition(parent.transform.TransformPoint(formationPosition));
        // Snap rotation
        rigidbody.MoveRotation(parent.transform.rotation);
    }

    private void Cluster()
    {
        // This is a cluster
        // Clusters have maximum cohesion, after the children make sure they reach their formation point, then become part of the hierarchy under their parent
        // As each child is a sub transform only the cluster leader needs to update movement logic

        // Possible states from Cluster
        // Orchestra

        movement.speedForward = FormationSpeed;

        MovementActions.Fly(this, UnityEngine.Random.onUnitSphere);

        // Clusters like to seek out players, and use orchestral pattern attacks to eliminate them
    }

    private void Orchestra()
    {
        // Orchestras are Clusters that are attacking
        // Orchestras tell children to fire, in specific sequences.
    }

    private void Wanderer()
    {
        // Wanderers can either find a formation to join
        // Becoming either a child of another, or a cluster
        // Or if they spot the player, become an Attacker

        // Possible states from Wanderer
        // Child
        // Cluster
        // Attacker

        // Child
        // This wanderer has been found by another and put in its formation
        if (parent != null)
        {
            state = State.Child;
        }
        // Cluster
        // This wanderer has found others and put them in its formation
        else if (children.Count > 0)
        {
            state = State.Cluster;
        }
        else
        {
            // Check to see if this wanderer can see players
            Transform[] spottedPlayers = eye.SenseAll();

            // Attacker
            // If it can make it go into attack mode, and keep the array of player transforms
            if (spottedPlayers != null)
            {
                if (spottedPlayers.Length > 0)
                {
                    targets = spottedPlayers;
                    target = spottedPlayers[(int)(UnityEngine.Random.value * (spottedPlayers.Length - 1))].gameObject.GetComponent<PlayerController>();

                    state = State.Attacker;
                }
            }
            // Wanderer
            // Otherwise this wanderer should check if their are formations to join
            // And move to a location within a specified distance of the 
            else
            {
                FormationCheck();
            }
        }
    }

    private void Attacker()
    {
        // Seeker
        // First an attacking cube should home in on the player
        // Then when it gets close start attacking

        // Attacker
        // 
    }

    private void Skulker()
    {

    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Formation Logic
    //
    ///////////////////////////////////////////////////////////////////////////////////////////////////

    public void SetFormationCollider()
    {
        collider.size = Vector3.one * GetFormationDimension() * FormationGridSize;
        collider.center = collider.size / 2.0f - (Vector3.one * FormationGridSize / 2);
    }

    public void SetParent(CubeController parent)
    {
        this.parent = parent;

        if (parent != null)
        {
            flocking.enabled = false;
        }
        else
        {
            flocking.enabled = true;
            rigidbody.isKinematic = false;
        }
    }

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
    }

    public Vector3 GetFormationLocation(CubeController child)
    {
        int index = children.IndexOf(child) + 1;

        int dimension = GetFormationDimension();

        return new Vector3(((index / dimension) / dimension) % dimension,
            (index / dimension) % dimension,
            index % dimension) * FormationGridSize;
    }

    public int GetFormationDimension()
    {
        return Mathf.CeilToInt(Mathf.Pow(children.Count + 1, 0.333f));
    }

    public int GetFormationDimension(int count)
    {
        return Mathf.CeilToInt(Mathf.Pow(count, 0.333f));
    }

    public void SetFormationPoint()
    {
        formationPosition = parent.GetFormationLocation(this);
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
                    RaycastHit[] hits = Physics.SphereCastAll(transform.position, FormationCheckRadius, transform.forward, LayerMask.GetMask(Literals.Strings.Physics.Layers.entities));

                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.collider.gameObject != gameObject)
                        {
                            if (hit.collider.gameObject.tag == Literals.Strings.Tags.Cube)
                            {
                                CubeController cube = hit.collider.gameObject.GetComponent<CubeController>();

                                // This cube has found a cube in a formation
                                if (cube.parent != null)
                                {
                                    cube.parent.AddChild(this);
                                }
                                // This cube has found a cube it can form a formation with
                                else
                                {
                                    if (cube.willJoinFormation)
                                    {
                                        if (cube.children.Count > 0)
                                        {
                                            cube.AddChild(this);
                                        }
                                        else
                                        {
                                            AddChild(cube);
                                        }
                                    }
                                }// cube.parent != null
                            }// hit.collider.gameObject.tag == Literals.Strings.Tags.Cube
                        }// hit.collider.gameObject != gameObject
                    }// foreach 
                }// parent == null
                // Else this cube is already in a formation and should check if it wants to leave it
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

                if (parent != null)
                {
                    parent.RemoveChild(this);
                    transform.parent = null;
                    rigidbody.isKinematic = false;
                }

                movement.speedForward = UnityEngine.Random.Range(LonerSpeedMin, LonerSpeedMax);
            }
        }
    }
}