using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // TODO REMOVE THIS WHEN BACK TO NETWORKING, and make this class a NetworkBehaviour
    public bool isLocalPlayer = true;

    public int Player = 1;

    [Header("Controller")]
    public Controller controller;

    [Header("Camera")]
    new public Camera camera;
    public RayQuery cameraRayQuery;
    public bool cameraOriented = true;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            string key = Literals.Strings.Tags.Player + i;
            if (!Blackboard.Global.ContainsKey(key))
                Blackboard.Global.Add(key, new BlackboardValue() { Value = this });
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;

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

    // Fixed Update is called once per physics step
    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;
    }

    Vector3 HandleMovementInput()
    {
        Vector3 v = new Vector3(Input.GetAxis(Literals.Strings.Input.Standard.MoveHorizontal+Player), 
                (Input.GetButton(Literals.Strings.Input.Standard.MoveSpecial+Player) == true) ? 1 : 0, 
                -Input.GetAxis(Literals.Strings.Input.Standard.MoveVertical+Player));

        if (v.x != 0 || v.z != 0)
        {
            // When camera orientated the input vector needs to be rotated so that its forward is pointing in the same way as the camera's forward (on the xz plane)
            if (cameraOriented)
            {
                // This should be achievable by first generating a rotation from the input to the camera forward;
                float eulerY = camera.transform.rotation.eulerAngles.y;

                Quaternion rotation = Quaternion.Euler(0, eulerY, 0);
                //if (Vector3.Dot(-camera.transform.forward, new Vector3(0, 0, -1)) < 0.86f)
                //    rotation = Quaternion.FromToRotation(new Vector3(0, 0, 1), -camera.transform.forward);
                //else
                //{
                //    v.Set(-v.x, v.y, v.z);

                //    rotation = Quaternion.FromToRotation(new Vector3(0, 0, -1), camera.transform.forward);
                //}

                float ty = v.y;

                v = rotation * v;
                //v.Normalize();

                v.Set(v.x, ty, v.z);
            }

            controller.movement.actionPrimaryLastInputTime = Time.time;
            controller.movement.actionPrimaryLastInputVector = v;
        }

        if (v.y > 0)
        {
            controller.movement.actionSecondaryLastInputTime = Time.time;
            controller.movement.actionSecondaryLastInputVector = v;
        }

        return v;
    }

    void MovementActionPrimaryCallback()
    {
        Vector3 input = HandleMovementInput();

        // This checks to see if the time between an input was last recieved and if the time is shorter than a percentage of the synchronisers duration and this frame
        if (Time.time - controller.movement.actionPrimaryLastInputTime < controller.movement.actionPrimarySynchroniser.Duration * MovementStatistics.FACTOR_OF_DURATION_AS_PADDING_ON_INPUT && input.sqrMagnitude < 1)
        {
            input = controller.movement.actionPrimaryLastInputVector;
        }

        MovementActions.Action(controller.movement.actionPrimary, controller, input, MovementActions.Move.Move);
    }

    void MovementActionSecondaryCallback()
    {
        Vector3 input = HandleMovementInput();

        if (Time.time - controller.movement.actionSecondaryLastInputTime < controller.movement.actionSecondarySynchroniser.Duration * MovementStatistics.FACTOR_OF_DURATION_AS_PADDING_ON_INPUT && input.y < 1)
        {
            input = controller.movement.actionSecondaryLastInputVector;
        }

        MovementActions.Action(controller.movement.actionSecondary, controller, input, MovementActions.Move.Move2);
    }
}
