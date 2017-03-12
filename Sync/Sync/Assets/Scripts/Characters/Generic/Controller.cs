using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Controller : NetworkBehaviour
{
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
    public MovementAction movementAction;
    [Tooltip("Establishes at what timing this character is allowed to perform their movement action, whether it be jumping, teleporting, or whatever it may be")]
    public Synchronism.Synchronisations movementSync = Synchronism.Synchronisations.HALF_NOTE;
    public Synchroniser synchroniser;
    // TELEPORT
    [Range(-25, 25)]
    public float movementTeleportDistance = 2.5f;
    public bool movementTeleportThroughWalls = false;
    public bool movementTeleportToTarget = true;
    public Vector3 movementTeleportTarget = Vector3.zero;
    // GLIDE
    [Range(0, 10)]
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

    [SerializeField]
    private string _faction;
    public string Faction { get { return _faction; } set { _faction = value; } }

    // Use this for initialization
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {

    }
}
