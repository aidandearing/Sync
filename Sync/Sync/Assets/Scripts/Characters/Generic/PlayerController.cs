using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : Controller
{
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
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

        HandleInput();

        animator.SetFloat("speedMove", rigidbody.velocity.magnitude);

        base.FixedUpdate();
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
