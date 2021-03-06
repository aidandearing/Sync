﻿using System;
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
                public const int entities = 11;
                public const int players = 12;
                public const int projectiles = 13;
                public const int particles = 14;
                public const int graffiti = 15;
            }
        }
    }

    public static class Strings
    {
        public static class Input
        {
            public static class Controller
            {
                public const string ButtonA = "A";
                public const string ButtonB = "B";
                public const string ButtonX = "X";
                public const string ButtonY = "Y";
                public const string BumperLeft = "Left Bumper";
                public const string BumperRight = "Right Bumper";
                public const string ButtonView = "View";
                public const string ButtonMenu = "Menu";
                public const string StickLeftButton = "Left Stick Button";
                public const string StickRightButton = "Right Stick Button";
                public const string StickLeftHorizontal = "Left Stick - Horizontal";
                public const string StickLeftVertical = "Left Stick - Vertical";
                public const string StickRightHorizontal = "Right Stick - Horizontal";
                public const string StickRightVertical = "Right Stick - Vertical";
                public const string PadHorizontal = "DPAD - Horizontal";
                public const string PadVertical = "DPAD - Vertical";
                public const string TriggerLeft = "Left Trigger";
                public const string TriggerRight = "Right Trigger";
                public const string TriggerBoth = "Left Trigger Shared Axis";
            }

            public static class Standard
            {
                public const string Interact = "Interact - Player ";
                public const string Menu = "Menu - Player ";
                public const string LookHorizontal = "Look Horizontal - Player ";
                public const string LookVertical = "Look Vertical - Player ";
                public const string MoveHorizontal = "Move Horizontal - Player ";
                public const string MoveVertical = "Move Vertical - Player ";
                public const string FlipSide = "Flip Side - Player ";
                public const string MoveSpecial = "Special Move - Player ";
                public const string Fire = "Fire - Player ";
                public const string Grenade = "Grenade - Player ";
                public const string Skill1 = "Skill 1 - Player ";
                public const string Skill2 = "Skill 2 - Player ";
                public const string Skill3 = "Skill 3 - Player ";
                public const string Skill4 = "Skill 4 - Player ";
                public const string LookMouseHorizontal = "Look Horizontal - Mouse";
                public const string LookMouseVertical = "Look Vertical - Mouse";
            }
        }

        public static class Tags
        {
            public const string Floor = "floor";
            public const string Wall = "wall";
            public const string Player = "player";
            public const string Cube = "cube";
            public const string NeroGraffitiController = "nero";
            public const string NeroGraffiti = "nero graffiti";
        }

        public static class Physics
        {
            public static class Layers
            {
                public const string Default = "Default";
                public const string Floors = "Floors";
                public const string Walls = "Walls";
                public const string Obstacles = "Obstacles";
                public const string Entities = "Entities";
                public const string Players = "Players";
                public const string Projectiles = "Projectiles";
                public const string Particles = "Particles";
                public const string Graffiti = "Graffiti";
            }
        }

        public static class Blackboard
        {
            public const string Type = "type";
            public const string Value = "value"; 

            public static class Synchronisation
            {
                public const string Synchroniser = "synchronisationSynchroniser";
            }

            public static class Controllers
            {
                public const string Controller = "controllersController";
                public const string Environment = "controllersEnvironmentController";
                public const string Player = "controllersPlayerController";
            }

            public static class Movement
            {
                public const string Input = "movementInputVector";
                public const string CanWalkBackward = "movementCanWalkBackward";
                public const string SpeedAll = "movementSpeedAll";
                public const string SpeedForward = "movementSpeedForward";
                public const string SpeedBackward = "movementSpeedBackward";
                public const string SpeedSidestep = "movementSpeedSidestep";
                public const string SpeedTurn = "movementSpeedTurn";
                public const string Force = "movementForce";
                public const string Vectoring = "movementVectoring";
                public const string Count = "movementCount";
                public const string InheritVelocity = "movementInheritVelocity";
                public const string TeleportDistance = "movementTeleportDistance";
                public const string TeleportThroughWalls = "movementTeleportThroughWalls";
                public const string TeleportToTarget = "movementTeleportToTarget";
                public const string TeleportTarget = "movementTeleportTarget";
                public const string GlideDownToForward = "movementGlideDownToForward";
                public const string ThrustSequencer = "movementThrustSequencer";
                public const string ThrustCurve = "movementThrustCurve";
            }

            public static class Locations
            {
                public const string CubeGatheringPointOne = "locationsCubeGatherPointOne";
            }
        }

        public static class Parameters
        {
            public static class Animation
            {
                public static class Vector
                {
                    public static string Forward(int axis)
                    {
                        string s = "x";

                        if (axis == 1)
                            s = "y";
                        else if (axis == 2)
                            s = "z";

                        return "vectorForward" + s;
                    }
                }

                public const string CycleMove = "cycleMove";
                public const string IsCasting = "isCasting";
                public const string IsCastLooping = "isCastLooping";
                public const string IsCastingSkill = "isCastingSkill";
                public const string IsAttacking = "isAttacking";
                public const string IsAttackLooping = "isAttackLooping";
                public const string IsFalling = "isFalling";
                public const string IsMoving = "isMoving";
                public const string IsMoving2 = "isMoving2";
                public const string IsOnGround = "isOnGround";
                public const string IsUsingGrenade = "isUsingGrenade";
                public const string IsUsingGrenadeLooping = "isUsingGrenadeLooping";
                public const string PlaySplash = "playSplash";
                public const string SpeedAttack = "speedAttack";
                public const string SpeedMove = "speedMove";
                public const string TouchedGround = "touchedGround";
                public const string WantsToFall = "wantsToFall";
                public const string WantsToAttack = "wantsToAttack";
                public const string WantsToCast = "wantsToCast";
                public const string WantsToCastSkill = "wantsToCastSkill";
                public const string WantsToMove = "wantsToMove";
                public const string WantsToMove2 = "wantsToMove2";
                public const string WantsToUseGrenade = "wantsToUseGrenade";
            }
        }
    }
}
