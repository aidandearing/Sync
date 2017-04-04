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
    public static float FormationSnapPosition = 1.0f;
    public static float FormationCheckRadius = 100.0f;
    public static float FormationCheckTime = 1.0f;
    public static float FormationCheckDelay = 10.0f;
    public static float FormationCheckDelayDelta = 2.5f;
    public static float FormationBreakChance = 0.01f;
    public static float FormationJoinSpeed = 7.5f;

    public static float LonerSpeedMin = 2.0f;
    public static float LonerSpeedMax = 10.0f;

    public float formationCheckCurrent;
    public Vector3 formationPosition = Vector3.zero;
    public bool willJoinFormation = true;

    public CubeFormation parentPrefab;
    public CubeFormation parent;

    public AIEye eye;
    public AIFlockBehaviour flocking;

    public Transform[] targets;
    public PlayerController target;

    public enum State { Loner, Child, ParentedChild, Cluster, Wanderer, Attacker, Skulker, Orchestra };
    public State state = State.Wanderer;

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

        formationCheckCurrent += UnityEngine.Random.Range(FormationCheckDelay - FormationCheckDelayDelta, FormationCheckDelay + FormationCheckDelayDelta);

        flocking.layerMask = LayerMask.GetMask(Literals.Strings.Physics.Layers.entities);
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

        switch (state)
        {
            case State.Loner: Loner(); break;
            case State.Child: Child(); break;
            case State.ParentedChild: ParentedChild(); break;
            case State.Skulker: Skulker(); break;
            case State.Wanderer: Wanderer(); break;
            case State.Attacker: Attacker(); break;
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
        if ((transform.position - worldPosFormation).magnitude < FormationSnapPosition)
        {
            // Snap them to the formation point, and keep them there
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

    private void Wanderer()
    {
        // Wanderers can either find a formation to join
        // Becoming a child
        // Or if they spot the player, become an Attacker

        // Possible states from Wanderer
        // Child
        // Attacker

        // Child
        // This wanderer has been found by another and put in its formation
        if (parent != null)
        {
            state = State.Child;
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
            // And wander around
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

    public void SetParent(CubeFormation parent)
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

    public void SetFormationPoint()
    {
        formationPosition = parent.GetFormationLocation(this);
        transform.SetParent(null);

        state = State.Child;
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
                                        if (cube.parent != null)
                                        {
                                            cube.parent.AddChild(this);
                                        }
                                        else
                                        {
                                            parent = Instantiate(parentPrefab, transform.position, transform.rotation);
                                            parent.AddChild(this);
                                            parent.AddChild(cube);
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

                state = State.Loner;

                movement.speedForward = UnityEngine.Random.Range(LonerSpeedMin, LonerSpeedMax);
            }
        }
    }
}