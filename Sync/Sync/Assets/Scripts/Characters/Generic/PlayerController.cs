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
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), Input.GetAxis("Vertical"));

        Vector3 movement = new Vector3(input.x * speedForward, 0, input.z * speedForward);

        bool actuallyWantsToMove = (MathHelper.Vector.XZ(input).sqrMagnitude > 0);

        if (animator.GetBool("isMoving"))
        {
            if (rigidbody.velocity.sqrMagnitude < speedForward * speedForward)
                rigidbody.AddForce(movement, ForceMode.Impulse);

            if (movement.sqrMagnitude > 0)
                rigidbody.MoveRotation(Quaternion.Euler(0, Mathf.Rad2Deg * Mathf.Atan2(rigidbody.velocity.x, rigidbody.velocity.z), 0));
        }
        else
        {
            if (input.sqrMagnitude > 0)
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

        if (input.y > 0)
        {
            Blackboard state = new Blackboard();
            state[Literals.Strings.Blackboard.Controller] = new BlackboardValue() { Value = this };
            state[Literals.Strings.Movement.Height] = new BlackboardValue() { Value = movementHeight };
            state[Literals.Strings.Movement.Vectoring] = new BlackboardValue() { Value = movementVectoring };
            state[Literals.Strings.Movement.Count] = new BlackboardValue() { Value = movementCount };
            state[Literals.Strings.Movement.InheritVelocity] = new BlackboardValue() { Value = movementInheritVelocity };

            movementActionInst = MovementAction.Factory(movementAction);
            movementActionInst.Do(state);
        }
    }
}
