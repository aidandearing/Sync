using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Controller : MonoBehaviour
{
    // TODO REMOVE THIS WHEN BACK TO NETWORKING, and make this class a NetworkBehaviour
    public bool isLocalPlayer = true;

    [Header("References")]
    new public Rigidbody rigidbody;
    public Animator animator;

    public MovementStatistics movement;
    public Statistics statistics;

    public bool checksForGround = true;

    [SerializeField]
    private string _faction;
    public string Faction { get { return _faction; } set { _faction = value; } }

    private Ray rayGroundCheck = new Ray(Vector3.zero, new Vector3(0, -1, 0));

    public bool isInitialised = false;

    // Use this for initialization
    protected virtual void Start()
    {
        statistics[Literals.Strings.Blackboard.Movement.Count] = new Statistic() { Value = movement.count };

        if (Blackboard.Global.ContainsKey(Literals.Strings.Blackboard.Synchronisation.Synchroniser))
        {
            movement.actionPrimarySynchroniser = (Blackboard.Global[Literals.Strings.Blackboard.Synchronisation.Synchroniser].Value as Synchronism).synchronisers[movement.actionPrimarySynchronisation];
            movement.actionPrimarySynchroniser.RegisterCallback(this, MovementActionPrimaryCallback);

            movement.actionSecondarySynchroniser = (Blackboard.Global[Literals.Strings.Blackboard.Synchronisation.Synchroniser].Value as Synchronism).synchronisers[movement.actionSecondarySynchronisation];
            movement.actionSecondarySynchroniser.RegisterCallback(this, MovementActionSecondaryCallback);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (movement.actionPrimarySynchroniser == null)
        {
            if (Blackboard.Global.ContainsKey(Literals.Strings.Blackboard.Synchronisation.Synchroniser))
            {
                movement.actionPrimarySynchroniser = (Blackboard.Global[Literals.Strings.Blackboard.Synchronisation.Synchroniser].Value as Synchronism).synchronisers[movement.actionPrimarySynchronisation];
                movement.actionPrimarySynchroniser.RegisterCallback(this, MovementActionPrimaryCallback);

                movement.actionSecondarySynchroniser = (Blackboard.Global[Literals.Strings.Blackboard.Synchronisation.Synchroniser].Value as Synchronism).synchronisers[movement.actionSecondarySynchronisation];
                movement.actionSecondarySynchroniser.RegisterCallback(this, MovementActionSecondaryCallback);
            }
        }
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        if (rigidbody.velocity.y < 0 && !animator.GetBool(Literals.Strings.Parameters.Animation.IsOnGround))
            animator.SetBool(Literals.Strings.Parameters.Animation.WantsToFall, true);
        else
            animator.SetBool(Literals.Strings.Parameters.Animation.WantsToFall, false);

        if (checksForGround)
        {
            rayGroundCheck.origin = transform.position;
            //rayGroundCheck = new Ray(transform.position, new Vector3(0, -1, 0));

            //Physics.Raycast(rayGroundCheck, movement.height, Literals.Integers.Physics.Layers.Floors)
            bool onGround = Physics.Raycast(rayGroundCheck, movement.height);
            animator.SetBool(Literals.Strings.Parameters.Animation.IsOnGround, onGround);
        }

        HandleMovementInput();
    }

    protected virtual void MovementActionPrimaryCallback()
    {
        MovementActions.Action(movement.actionPrimary, this, HandleMovementInput());
    }

    protected virtual void MovementActionSecondaryCallback()
    {
        MovementActions.Action(movement.actionSecondary, this, HandleMovementInput());
    }

    protected virtual Vector3 HandleMovementInput()
    {
        return Vector3.zero;
    }

    //protected virtual void OnCollisionEnter(Collision collision)
    //{
    //    //if (collision.collider.gameObject.layer == Literals.Integers.Physics.Layers.Floors)
    //    //    animator.SetBool(Literals.Strings.Parameters.Animation.IsOnGround, true);
    //}

    //protected virtual void OnCollisionStay(Collision collision)
    //{
    //    if (collision.collider.gameObject.layer == Literals.Integers.Physics.Layers.Floors)
    //        animator.SetBool(Literals.Strings.Parameters.Animation.IsOnGround, true);
    //}

    //protected virtual void OnCollisionExit(Collision collision)
    //{
    //    if (collision.collider.gameObject.layer == Literals.Integers.Physics.Layers.Floors)
    //        animator.SetBool(Literals.Strings.Parameters.Animation.IsOnGround, false);
    //}
}
