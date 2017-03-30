//using UnityEngine;
//using System.Collections;

//public class ThirdPersonController : Controller
//{
//    [SerializeField]
//    private Rigidbody rigidself;
//    [SerializeField]
//    private Camera camera;
//    [SerializeField]
//    private Transform aimNode;

//    private Vector3 currentVelocity;

//    // Jump variables
//    public bool canVectorInAir;
//    public int jumps = 1;
//    private int jumps_current;
//    public float jump_cooldown = 0.5f;
//    private float jump_cooldown_current;

//    public bool isOnGround;
//    public bool isTouching;

//    // Use this for initialization
//    protected override void Start()
//    {
//        base.Start();
//        // DEBUGGING!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//        isOnGround = true;

//        jumps_current = 0;
//        jump_cooldown_current = jump_cooldown;
//    }

//    // Update is called once per frame
//    protected override void Update()
//    {
//        base.Update();

//        Ray cameraForward = new Ray(transform.position, camera.transform.forward);
//        RaycastHit cameraLookAt;
//        bool cameraIsLooking = Physics.Raycast(cameraForward, out cameraLookAt, 500f);

//        Vector3 forward = Vector3.zero;

//        if (cameraIsLooking)
//            forward = camera.transform.forward * cameraLookAt.distance;
//        else
//            forward = camera.transform.forward * 500;

//        aimNode.rotation = Quaternion.LookRotation(forward);

//        forward.y = 0;

//        transform.rotation = Quaternion.LookRotation(forward);

//        Ray down = new Ray(transform.position, Vector3.down);
//        RaycastHit ground;
//        isOnGround = Physics.Raycast(down, out ground, 1.0f);

//        // THIS NEEDS TO GO AT SOME POINT
//        // This is my hacky way of shutting the damn errors up for a bit so I can debug.
//        if (Movement.magnitude > 0)
//            HandleInput();

//        // Line renderer code
//        // has potential if ever I want laser beads
//        Vector3[] positions = new Vector3[2];
//        positions[0] = transform.position;

//        if (cameraIsLooking)
//            positions[1] = cameraLookAt.point;
//        else
//            positions[1] = transform.position + camera.transform.forward * 500;

//        GetComponentInChildren<LineRenderer>().SetPositions(positions);
//    }

//    void HandleInput()
//    {
//        if (isOnGround || canVectorInAir)
//        {
//            // Establish a temporary movement vector
//            Vector3 moveGoal = Vector3.zero;

//            // If moving forward or backward, add only the forward backward component (z)
//            if (Input.GetKey(KeyCode.W))
//                moveGoal.z = Movement.z;
//            else if (Input.GetKey(KeyCode.S))
//                moveGoal.z = -Movement.z;

//            // If moving sideways, add only the sideways component (x)
//            if (Input.GetKey(KeyCode.A))
//                moveGoal.x = -Movement.x;
//            else if (Input.GetKey(KeyCode.D))
//                moveGoal.x = Movement.x;

//            // Normalize it
//            moveGoal.Normalize();

//            // Here is where the magic happens
//            // Usually if you have a z forward of 20 and an x sideways of 5 if you just add them together the character moves more than the maximum of 20 on a diagonal, not how people move
//            // So if you normalize the whole mess, then restore the leading axis (z) take whatever percentage of the current move vector in comparison to the goal movement speed and use it as a
//            // multiplier against the secondary axis (x) magnitude, you establish a move vector that is limited to a maximum magnitude of the leading axis.
//            // Save I am ignoring y, because it is for jumping, which doesn't need any of this.
//            moveGoal.z *= Movement.z;
//            moveGoal.x *= (1 - (moveGoal.z / Movement.z)) * Movement.x;

//            // If the z is negative the player is moving backwards, they should be backstepping, therefore they need to use the backstepMult
//            if (moveGoal.z < 0)
//                moveGoal.z *= MovementBackstepMult;

//            // Sprinting, if moving forward
//            if (Input.GetButton("Sprint"))
//            {
//                if (moveGoal.z > 0)
//                    moveGoal.z *= MovementSprintMult;
//            }

//            // Then it is just a matter of using the beautiful SmoothDamp method (much like a spring, but unable to go past the goal) to figure out the best way of making the player move naturally
//            transform.position = Vector3.SmoothDamp(transform.position, transform.position + transform.TransformDirection(moveGoal), ref currentVelocity, 1);
//        }

//        if (jump_cooldown_current >= jump_cooldown)
//        {
//            jump_cooldown_current -= jump_cooldown;

//            if (Input.GetButton("Jump"))
//            {
//                if (jumps_current > 0)
//                {
//                    jumps_current--;
//                    rigidself.AddForce(0, Movement.y, 0, ForceMode.VelocityChange);
//                }
//            }
//        }
//        else
//        {
//            jump_cooldown_current += Time.deltaTime;
//        }

//        if (isOnGround)
//        {
//            jumps_current = jumps;
//        }

//        // Action logic
//        // Shoot
//        if (Input.GetButton("Shoot"))
//        {
//            BroadcastMessage("Shoot");//, SendMessageOptions.DontRequireReceiver);
//        }
//    }

//    void OnCollisionStay(Collision collision)
//    {
//        isTouching = true;
//    }
//}
