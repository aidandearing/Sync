using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : Controller
{
    public enum MovementAction
    {
        /// <summary>
        /// This character will perform a simple jump
        /// </summary>
        Jump,
        /// <summary>
        /// This character will teleport across the ground, at their current height in their forward direction
        /// </summary>
        Teleport,
        /// <summary>
        /// This character will fall far less slowly, turning most of the speed into forward speed
        /// </summary>
        Glide,
        /// <summary>
        /// This character will perform a jump and then transition into a glide
        /// </summary>
        JumpGlide,
        /// <summary>
        /// This character will hover
        /// </summary>
        Hover,
        /// <summary>
        /// This character will perform a jump and then transition into a hover
        /// </summary>
        JumpHover,
        /// <summary>
        /// This character will thrust themselves into the air
        /// </summary>
        Thrust,
        /// <summary>
        /// This character will jump and then thrust themselves into the air
        /// </summary>
        JumpThrust,
        /// <summary>
        /// This character will count walls as valid to jump off of, as well as the ground
        /// </summary>
        WallJump
    };

    [Header("References")]
    new public Rigidbody rigidbody;
    public Animator animator;

    [Header("Movement")]
    [Range(0, 15)]
    [Tooltip("The speed in m/s that this character will move forward")]
    public float speedForward;
    [Tooltip("By default the controller moves in a 360 direction, with independant camera control, with this true that changes to a camera controlled forward, strafing, and backstepping capable movement")]
    public bool canWalkBackward = false;
    [Range(0, 7)]
    [Tooltip("The speed in m/s that this character will move backwards")]
    public float speedBackward;
    [Range(0, 15)]
    [Tooltip("The speed in m/s that this character will move sideways")]
    public float speedSidestep;
    [Tooltip("The format for this characters movement")]
    public MovementAction movementAction = MovementAction.Jump;
    [Tooltip("Establishes at what timing this character is allowed to perform their movement action, whether it be jumping, teleporting, or whatever it may be")]
    public Synchronism.Synchronisations movementSync = Synchronism.Synchronisations.HALF_NOTE;
    public Synchroniser synchroniser;
    // TELEPORT
    [Range(-25,25)]
    public float movementTeleportDistance = 2.5f;
    public bool movementTeleportThroughWalls = false;
    public bool movementTeleportToTarget = true;
    public Vector3 movementTeleportTarget = Vector3.zero;
    // GLIDE
    [Range(0,10)]
    public float movementGlideDownToForward = 0.9f;
    // THRUST
    public float movementThrustSpeed = 10.0f;
    public SequencerGradient movementThrustSequencer;
    public AnimationCurve movementThrustCurve = new AnimationCurve();
    // JUMP and HOVER
    [Range(0, 10)]
    public float movementHeight = 2;
    public bool movementVectoring = false;
    // GENERAL
    [Range(-1, 100)]
    [Tooltip("The number of times this character is able to perform their movement action, -1 for infinite actions")]
    public float movementCount = 1;
    public bool movementInheritVelocity = true;

    // Use this for initialization
    void Start()
    {

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

        HandleInput();

        animator.SetFloat("speedMove", rigidbody.velocity.magnitude);
    }

    void HandleInput()
    {
        bool wantsToMove = animator.GetBool("wantsToMove");

        // Make a vector that has its left right axis set to the desired horizontal movement, its vertical axis set to 1 or 0 desired jump state, and its forward axis set to the desired forward movement
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal") * speedForward, Input.GetAxis("Jump"), Input.GetAxis("Vertical") * speedForward);

        bool actuallyWantsToMove = (MathHelper.Vector.XZ(movement).sqrMagnitude > 0);

        //animator.SetFloat("cycleMove", (Blackboard.Global["Synchroniser"].Value as Synchronism).synchronisers[Synchronism.Synchronisations.QUARTER_NOTE].Percent);

        if (animator.GetBool("isMoving"))
        {
            //if (movement.sqrMagnitude <= 0 && wantsToMove)
            //    movement = transform.forward;

            rigidbody.AddForce(movement - rigidbody.velocity, ForceMode.Impulse);

            if (movement.sqrMagnitude > 0)
                rigidbody.MoveRotation(Quaternion.Euler(0, Mathf.Rad2Deg * Mathf.Atan2(rigidbody.velocity.x, rigidbody.velocity.z), 0));
        }
        else
        {
            if (movement.sqrMagnitude > 0)
                animator.SetBool("wantsToMove", actuallyWantsToMove);
            else
                animator.SetBool("wantsToMove", actuallyWantsToMove);
        }

        // Shooter Mode
        if (canWalkBackward)
        {

        }
        // 360 Action Mode
        else
        {

        }
    }
}
