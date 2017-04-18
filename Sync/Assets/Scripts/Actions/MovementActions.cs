using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MovementActions
{
    /// <summary>
    /// All possible actions any controller can perform within this action context
    /// </summary>
    public enum Actions
    {
        None,
        /// <summary>
        /// Limits all movement to the XZ plane, ignores all Y input
        /// </summary>
        MoveXZ360,
        /// <summary>
        /// Perform a jump
        /// </summary>
        Jump,
        /// <summary>
        /// Free motion in all directions
        /// </summary>
        Fly
    }

    public enum Move
    {
        None,
        Move,
        Move2
    }

    public static void Action(Actions action, Controller self, Vector3 input, Move move)
    {
        switch (action)
        {
            case Actions.MoveXZ360:
                MoveXZ360(self, input, move);
                break;
            case Actions.Jump:
                Jump(self, input, move);
                break;
            case Actions.Fly:
                Fly(self, input, move);
                break;
        }
    }

    /// <summary>
    /// This static function handles the logic of moving a controller, based on some input vector, along an xz plane with 360 degree forward motion
    /// </summary>
    /// <param name="self">The Controller doing the moving</param>
    /// <param name="input">The input vector, each axis is treated as 0 no desire to move, to 1 complete desire to move</param>
    public static void MoveXZ360(Controller self, Vector3 input, Move move)
    {
        float speed = new Vector2(input.x, input.z).magnitude;

        // MoveXZ360 only works while on the ground
        if (self.animator.GetBool(Literals.Strings.Parameters.Animation.IsOnGround))
        {
            string wantsToMove = (move == Move.Move) ? Literals.Strings.Parameters.Animation.WantsToMove : Literals.Strings.Parameters.Animation.WantsToMove2;
            string isMoving = (move == Move.Move) ? Literals.Strings.Parameters.Animation.IsMoving : Literals.Strings.Parameters.Animation.IsMoving2;
            // Determine if input is being recieved this turn, if so that means the controller actually desires movement
            bool actuallyWantsToMove = (input.x != 0 || input.z != 0);

            // Ensure the animator knows the controller's desired state
            self.animator.SetBool(wantsToMove, actuallyWantsToMove);

            // Only allow movement when the animator is moving
            if (self.animator.GetBool(isMoving))
            {
                // If the controller wants to move and is currently in the move animation then allow movement
                if (actuallyWantsToMove)
                {
                    // If not already moving at full speed begin moving at full speed
                    float speedForward = (float)self.statistics[Literals.Strings.Blackboard.Movement.SpeedForward].Value * (float)self.statistics[Literals.Strings.Blackboard.Movement.SpeedAll].Value;
                    if (self.rigidbody.velocity.sqrMagnitude < speedForward * speedForward)
                        self.rigidbody.AddForce(self.transform.forward * speedForward * speed * self.rigidbody.mass, ForceMode.Impulse);
                    // If moving at full speed continue to do so, but based on the forward
                    else
                        self.rigidbody.velocity = self.transform.forward * self.movement.speedForward;
                }

                // If any input is being recieved then the controller should be rotated towards the input
                float speedTurn = (float)self.statistics[Literals.Strings.Blackboard.Movement.SpeedTurn].Value;
                if (input.sqrMagnitude > 0)
                {
                    self.rigidbody.MoveRotation(Quaternion.RotateTowards(self.rigidbody.rotation, Quaternion.Euler(0, Mathf.Rad2Deg * Mathf.Atan2(input.x, input.z), 0), speedTurn * Time.fixedDeltaTime));
                }
                // Else the controller should be rotated towards their velocity's direction
                else if (self.rigidbody.velocity.sqrMagnitude > 1)
                {
                    self.rigidbody.MoveRotation(Quaternion.RotateTowards(self.rigidbody.rotation, Quaternion.Euler(0, Mathf.Rad2Deg * Mathf.Atan2(self.rigidbody.velocity.x, self.rigidbody.velocity.z), 0), speedTurn * Time.fixedDeltaTime));
                }
            }
        }
        // All non-ground MoveXZ360 logic
        else
        {
            // If the controller allows in air control
            if (self.movement.vectoring)
            {
                // If not already moving at full speed begin moving at full speed
                if (self.rigidbody.velocity.sqrMagnitude < self.movement.speedForward * self.movement.speedForward)
                    self.rigidbody.AddForce(self.transform.forward * self.movement.speedForward * speed * self.rigidbody.mass, ForceMode.Impulse);
                // If moving at full speed continue to do so, but based on the forward
                //else
                //    self.rigidbody.velocity = self.transform.forward * self.movement.speedForward;

                // If any input is being recieved then the controller should be rotated towards the input
                if (input.sqrMagnitude > 0)
                {
                    self.rigidbody.MoveRotation(Quaternion.RotateTowards(self.rigidbody.rotation, Quaternion.Euler(0, Mathf.Rad2Deg * Mathf.Atan2(input.x, input.z), 0), self.movement.speedTurn * Time.fixedDeltaTime));
                }
                // Else the controller should be rotated towards their velocity's direction
                else if (self.rigidbody.velocity.sqrMagnitude > 1)
                {
                    self.rigidbody.MoveRotation(Quaternion.RotateTowards(self.rigidbody.rotation, Quaternion.Euler(0, Mathf.Rad2Deg * Mathf.Atan2(self.rigidbody.velocity.x, self.rigidbody.velocity.z), 0), self.movement.speedTurn * Time.fixedDeltaTime));
                }
            }
        }

        self.animator.SetFloat(Literals.Strings.Parameters.Animation.SpeedMove, self.rigidbody.velocity.magnitude);
    }

    public static void Jump(Controller self, Vector3 input, Move move)
    {
        bool wantsToJump = false;

        string wantsToMove = (move == Move.Move) ? Literals.Strings.Parameters.Animation.WantsToMove : Literals.Strings.Parameters.Animation.WantsToMove2;
        string isMoving = (move == Move.Move) ? Literals.Strings.Parameters.Animation.IsMoving : Literals.Strings.Parameters.Animation.IsMoving2;

        if (self.animator.GetBool(Literals.Strings.Parameters.Animation.IsOnGround) || self.movement.countCurrent > 0)
        {
            self.movement.countCurrent--;

            Vector3 movement = input.normalized * self.movement.force;

            wantsToJump = input.y > 0;

            if (input.y > 0)
            {
                //self.rigidbody.AddForce(movement, ForceMode.Impulse);

                if (self.animator.GetBool(isMoving))
                {
                    self.rigidbody.AddForce(movement, ForceMode.Impulse);
                }
                else
                {
                    //self.animator.SetFloat(Literals.Strings.Parameters.Animation.JumpX, input.x);
                    //self.animator.SetFloat(Literals.Strings.Parameters.Animation.JumpY, input.y);
                    //self.animator.SetFloat(Literals.Strings.Parameters.Animation.JumpZ, input.z);
                }
            }
        }

        self.animator.SetBool(wantsToMove, wantsToJump);
    }

    public static void Fly(Controller self, Vector3 input, Move move)
    {
        if (input.sqrMagnitude > 0)
        {
            // If not already moving at full speed begin moving at full speed
            float speedForward = (float)self.statistics[Literals.Strings.Blackboard.Movement.SpeedForward].Value * (float)self.statistics[Literals.Strings.Blackboard.Movement.SpeedAll].Value;
            if (self.rigidbody.velocity.sqrMagnitude < speedForward * speedForward)
                self.rigidbody.AddForce(self.transform.forward * speedForward * self.rigidbody.mass, ForceMode.Force);
            // If moving at full speed continue to do so, but based on the forward
            //else
            //    self.rigidbody.velocity = self.transform.forward * self.movement.speedForward;

            // If any input is being recieved then the controller should be rotated towards the input
            float speedTurn = (float)self.statistics[Literals.Strings.Blackboard.Movement.SpeedTurn].Value;
            if (input.sqrMagnitude > 0)
            {
                self.rigidbody.MoveRotation(Quaternion.RotateTowards(self.rigidbody.rotation, Quaternion.LookRotation(input), speedTurn * Time.fixedDeltaTime));
            }
            // Else the controller should be rotated towards their velocity's direction
            else if (self.rigidbody.velocity.sqrMagnitude > 1)
            {
                self.rigidbody.MoveRotation(Quaternion.RotateTowards(self.rigidbody.rotation, Quaternion.LookRotation(self.rigidbody.velocity), speedTurn * Time.fixedDeltaTime));
            }
        }
    }
}
