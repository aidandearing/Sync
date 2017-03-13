using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MovementActionJump : MovementAction
{
    public override void Do(Blackboard state)
    {
        // Jumping uses several variables from the blackboard
        // These are
        // Literals.Strings.Blackboard.Controller
        // Literals.Strings.Movement.Height
        // Literals.Strings.Movement.Vectoring
        // Literals.Strings.Movement.Count
        // Literals.Strings.Movement.InheritVelocity

        Controller self = state[Literals.Strings.Blackboard.Controller].Value as Controller;
        float height = (float)state[Literals.Strings.Movement.Height].Value;
        bool vectoring = (bool)state[Literals.Strings.Movement.Vectoring].Value;
        int count = (int)state[Literals.Strings.Movement.Count].Value;
        bool inherit = (bool)state[Literals.Strings.Movement.InheritVelocity].Value;

        // Now based on self.rigidbody I need to make sure that the controller will be launched Height into the air

        // Calculate how much impulse the controller must be given in order to achieve the desired height
        // This is not correct
        float impulse = height * self.rigidbody.mass;

        self.rigidbody.AddForce(new Vector3(0, impulse, 0), ForceMode.Impulse);
    }
}