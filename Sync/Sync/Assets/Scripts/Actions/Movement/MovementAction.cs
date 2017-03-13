using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class MovementAction : BaseAction
{
    public enum Action { Move, Jump, Teleport, Hover, Glide, JumpHover, JumpGlide, Thrust, JumpThrust };

    public static MovementAction Factory(Action type)
    {
        switch(type)
        {
            case Action.Jump:
                return new MovementActionJump();
            case Action.Teleport:
                return new MovementActionTeleport();
        }

        return null;
    }
}
