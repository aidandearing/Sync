using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CubeController : MonoBehaviour
{
    // TODO REMOVE THIS WHEN BACK TO NETWORKING, and make this class a NetworkBehaviour
    public bool isLocalPlayer = true;

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

    [Header("Controller")]
    public Controller controller;

    [Header("Synchronisation")]
    public Synchronism.Synchronisations synchronisation = Synchronism.Synchronisations.BAR_2;
    public Synchroniser synchroniser;
    public SequencerGameObjects prefabAttack;
    public GameObject prefabDie;
    public GameObject prefabFind;
    //public GameObject prefabLose;

    [Header("Formation")]
    public float formationCheckCurrent;
    public Vector3 formationPosition = Vector3.zero;
    public bool willJoinFormation = true;
    public bool willLooseFormation = true;
    public bool manuallyParented = false;

    public CubeFormation parentPrefab;
    public CubeFormation parent;

    [Header("AI")]
    public AIEye eye;
    public AISphereSensor sensor;
    public AIFlockBehaviour flocking;

    [Header("Targeting")]
    public Transform[] targets;
    public PlayerController target;

    public float lifetime = 60.0f;
    public float lifetimeCurrent = 0.0f;

    public enum State { Loner, Child, ParentedChild, Wanderer, Attacker, Skulker, Die };
    public State state = State.Wanderer;
    public State lastState = State.Wanderer;

    public bool isInitialised;

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

        if (controller.movement.actionPrimarySynchroniser == null)
        {
            if (Blackboard.Global.ContainsKey(Literals.Strings.Blackboard.Synchronisation.Synchroniser))
            {
                controller.movement.actionPrimarySynchroniser = (Blackboard.Global[Literals.Strings.Blackboard.Synchronisation.Synchroniser].Value as Synchronism).synchronisers[controller.movement.actionPrimarySynchronisation];
                controller.movement.actionPrimarySynchroniser.RegisterCallback(this, MovementActionPrimaryCallback);

                controller.movement.actionSecondarySynchroniser = (Blackboard.Global[Literals.Strings.Blackboard.Synchronisation.Synchroniser].Value as Synchronism).synchronisers[controller.movement.actionSecondarySynchronisation];
                controller.movement.actionSecondarySynchroniser.RegisterCallback(this, MovementActionSecondaryCallback);
            }
        }
    }

    public void Attack()
    {
        //Vector3 delta = target.transform.position - transform.position;

        //if (delta.sqrMagnitude < 100)
        //{
            GameObject inst = prefabAttack.Evaluate();

            if (inst)
                Instantiate(inst, transform, false);
        //}
    }

    public void Callback()
    {
        if (state != lastState)
        {
            switch (state)
            {
                case State.Attacker:
                    Instantiate(prefabFind, transform, false);
                    lastState = State.Attacker;
                    break;
                case State.Die:
                    if (parent)
                        parent.RemoveChild(this);

                    Instantiate(prefabDie, transform.position, new Quaternion());
                    break;
            }
        }
        else
        {
            if (state == State.Attacker)
            {
                if ((target.transform.position - transform.position).sqrMagnitude < 200)
                {
                    Attack();
                }
            }
            else if (state == State.Die)
            {
                synchroniser.UnregisterCallback(this);
                controller.movement.actionPrimarySynchroniser.UnregisterCallback(this);
                controller.movement.actionSecondarySynchroniser.UnregisterCallback(this);

                Destroy(this.gameObject);
            }
        }

        lastState = state;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  MonoBehaviour
    //
    ///////////////////////////////////////////////////////////////////////////////////////////////////

    // Use this for initialization
    void Start()
    {
        FormationBreakCheck();

        formationCheckCurrent += UnityEngine.Random.Range(FormationCheckDelay - FormationCheckDelayDelta, FormationCheckDelay + FormationCheckDelayDelta);

        flocking.layerMask = LayerMask.GetMask(Literals.Strings.Physics.Layers.Entities);
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
        lifetimeCurrent += Time.fixedDeltaTime;

        if (!isLocalPlayer)
            return;

        if (!isInitialised)
            Initialise();

        flocking.alignmentDistance = AlignmentDistanceDefault;
        flocking.cohesionDistance = CohesionDistanceDefault;
        flocking.seperationDistance = SeperationDistanceDefault;

        if (formationCheckCurrent > 0)
            formationCheckCurrent -= Time.fixedDeltaTime;

        if (manuallyParented)
        {
            formationPosition = parent.GetFormationLocation(this);
            manuallyParented = false;
        }

        switch (state)
        {
            case State.Loner: Loner(); break;
            case State.Child: Child(); break;
            case State.ParentedChild: ParentedChild(); break;
            case State.Skulker: Skulker(); break;
            case State.Wanderer: Wanderer(); break;
            case State.Attacker: Attacker(); break;
            default: state = State.Die; break;
        }

        if ((float)controller.statistics["health"].Value <= 0 || lifetimeCurrent >= lifetime)
        {
            state = State.Die;
        }
    }

    Vector3 HandleMovementInput()
    {
        //Vector3 input = UnityEngine.Random.insideUnitCircle.normalized;
        //input = new Vector3(input.x, 0, input.y);
        //return input;

        return transform.forward;
    }

    void MovementActionPrimaryCallback()
    {
        MovementActions.Action(controller.movement.actionPrimary, controller, HandleMovementInput(), MovementActions.Move.Move);
    }

    void MovementActionSecondaryCallback()
    {
        //MovementActions.Action(controller.movement.actionPrimary, controller, HandleMovementInput(), MovementActions.Move.Move);
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
            if (!willLooseFormation)
            {
                // Snap them to the formation point, and keep them there
                controller.rigidbody.MovePosition(worldPosFormation);
                // Start rotating them so they look the same way their parent does
                controller.rigidbody.MoveRotation(Quaternion.RotateTowards(controller.rigidbody.rotation, Quaternion.LookRotation(parent.transform.forward), controller.movement.speedTurn * Time.fixedDeltaTime));
                // If they look within a threshold angle of the same way then snap their rotation
                if (Vector3.Dot(transform.forward, parent.transform.forward) > Mathf.Sin(Mathf.Deg2Rad * FormationSnapAngle))
                {
                    // Snap rotation
                    controller.rigidbody.MoveRotation(parent.transform.rotation);
                    // Make this child transform a child of parent transform (make sure they move together without me further having to waste time on calculating that myself)
                    transform.SetParent(parent.transform);
                    // And finally disable any capacity for them to recieve force, which disables any other attempts at moving them out of formation
                    //rigidbody.isKinematic = true;
                    state = State.ParentedChild;
                }
            }
        }
        // If they aren't move them towards it
        else
        {
            controller.movement.speedForward = FormationJoinSpeed;

            MovementActions.Fly(controller, (worldPosFormation - transform.position).normalized, MovementActions.Move.Move);

            Debug.DrawLine(worldPosFormation, transform.position);
            Debug.DrawRay(transform.position, (worldPosFormation - transform.position).normalized, new Color(1, 0, 1));

            flocking.alignmentDistance = AlignmentDistanceChild;
            flocking.cohesionDistance = CohesionDistanceChild;
            flocking.seperationDistance = SeperationDistanceChild;
        }
    }

    private void ParentedChild()
    {
        if (!willLooseFormation)
        {
            // This child is parented, and should have a chance of gaining independance
            // Formation check automatically does this, for parented cubes
            FormationCheck();
            // Snap them to their formation position, and keep them there
            controller.rigidbody.MovePosition(parent.transform.TransformPoint(formationPosition));
            // Snap rotation
            controller.rigidbody.MoveRotation(parent.transform.rotation);
        }
        else
        {
            state = State.Child;
        }
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
        if (parent)
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

                Transform t = sensor.Sense();

                if (t != null)
                {
                    target = t.gameObject.GetComponent<PlayerController>();
                    state = State.Attacker;
                }
            }
        }
    }

    private void Attacker()
    {
        // Seeker
        // First an attacking cube should home in on the player
        // Then when it gets close start attacking
        if (target != null)
        {
            MovementActions.Fly(controller, transform.forward, MovementActions.Move.Move);
            Vector3 position = target.transform.position + new Vector3(0, 10.0f, 0);
            Quaternion look = Quaternion.LookRotation(position - transform.position);
            controller.rigidbody.MoveRotation(Quaternion.RotateTowards(controller.rigidbody.rotation, look, controller.movement.speedTurn * Time.fixedDeltaTime));
        }
        else
        {
            state = State.Wanderer;
        }
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
            //rigidbody.isKinematic = false;
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
                    RaycastHit[] hits = Physics.SphereCastAll(transform.position, FormationCheckRadius, transform.forward, LayerMask.GetMask(Literals.Strings.Physics.Layers.Entities));

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
                                    break;
                                }
                                // This cube has found a cube it can form a formation with
                                else
                                {
                                    if (cube.willJoinFormation)
                                    {
                                        if (cube.parent != null)
                                        {
                                            cube.parent.AddChild(this);
                                            break;
                                        }
                                        else
                                        {
                                            parent = Instantiate(parentPrefab, transform.position, transform.rotation);
                                            parent.AddChild(this);
                                            parent.AddChild(cube);
                                            break;
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
        return;

        if (UnityEngine.Random.value < FormationBreakChance)
        {
            if (willJoinFormation)
            {
                willJoinFormation = false;

                if (parent != null)
                {
                    parent.RemoveChild(this);
                    transform.parent = null;
                    controller.rigidbody.isKinematic = false;
                }

                state = State.Loner;

                controller.movement.speedForward = UnityEngine.Random.Range(LonerSpeedMin, LonerSpeedMax);
            }
        }
    }
}