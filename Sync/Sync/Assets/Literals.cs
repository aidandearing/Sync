using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Literals
{
    public static class Integers
    {
        public static class Physics
        {
            public static class Layers
            {
                public const int Floors = 8;
                public const int walls = 9;
                public const int obstacles = 10;
                public static int entities = 11;
                public const int players = 12;
                public const int projectiles = 13;
                public const int particles = 14;
                public const int graffiti = 15;
            }
        }
    }

    public static class Strings
    {
        public static class Tags
        {
            public const string Floor = "floor";
            public const string Wall = "wall";
            public const string Player = "player";
            public const string NeroGraffitiController = "nero";
            public const string NeroGraffiti = "nero graffiti";
        }

        public static class Blackboard
        {
            public const string Synchroniser = "synchroniser";
            public const string Controller = "controller";
            public const string Player = "playerController";
        }

        public static class Movement
        {
            public const string CanWalkBackward = "canWalkBackward";
            public const string SpeedForward = "speedForward";
            public const string SpeedBackward = "speedBackward";
            public const string SpeedSidestep = "speedSidestep";
            public const string Height = "movementHeight";
            public const string Vectoring = "movementVectoring";
            public const string Count = "movementCount";
            public const string InheritVelocity = "movementInheritVelocity";
            public const string TeleportDistance = "movementTeleportDistance";
            public const string TeleportThroughWalls = "movementTeleportThroughWalls";
            public const string TeleportToTarget = "movementTeleportToTarget";
            public const string TeleportTarget = "movementTeleportTarget";
            public const string GlideDownToForward = "movementGlideDownToForward";
            public const string ThrustSpeed = "movementThrustSpeed";
            public const string ThrustSequencer = "movementThrustSequencer";
            public const string ThrustCurve = "movementThrustCurve";
        }

        public static class Parameters
        {
            public static class Animation
            {
                public const string IsMoving = "isMoving";
                public const string WantsToMove = "wantsToMove";
                public const string PlaySplash = "playSplash";
                public const string SpeedMove = "speedMove";
                public const string CycleMove = "cycleMove";
            }
        }
    }
}
