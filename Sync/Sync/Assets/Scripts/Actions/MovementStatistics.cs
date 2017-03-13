using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class MovementStatistics : Statistics
{
    /// <summary>
    /// This constant defines how much of the synchronisers duration will be used as the amount of time after an input was entered that it will still be considered valid when the action can actually occur
    /// </summary>
    public const float FACTOR_OF_DURATION_AS_PADDING_ON_INPUT = 0.75f;

    [Tooltip("The format for this characters movement")]
    public MovementActions.Actions actionPrimary = MovementActions.Actions.MoveXZ360;
    [NonSerialized] public float actionPrimaryLastInputTime = 0;
    [NonSerialized] public Vector3 actionPrimaryLastInputVector = Vector3.zero;
    [Tooltip("Establishes at what timing this character is allowed to perform their primary action, whether it be jumping, teleporting, or whatever it may be")]
    public Synchronism.Synchronisations actionPrimarySynchronisation = Synchronism.Synchronisations.THIRTYSECOND_NOTE;
    [NonSerialized] public Synchroniser actionPrimarySynchroniser;
    [Tooltip("The format for this characters secondary movement")]
    public MovementActions.Actions actionSecondary = MovementActions.Actions.Jump;
    [NonSerialized] public float actionSecondaryLastInputTime = 0;
    [NonSerialized] public Vector3 actionSecondaryLastInputVector = Vector3.zero;
    [Tooltip("Establishes at what timing this character is allowed to perform their secondary action, whether it be jumping, teleporting, or whatever it may be")]
    public Synchronism.Synchronisations actionSecondarySynchronisation = Synchronism.Synchronisations.HALF_NOTE;
    public Synchroniser actionSecondarySynchroniser;
    [Range(0, 15)]
    [Tooltip("The speed in m/s that this character will move forward")]
    public float speedForward = 5;
    [Range(0, 7)]
    [Tooltip("The speed in m/s that this character will move backwards")]
    public float speedBackward = 2;
    [Range(0, 15)]
    [Tooltip("The speed in m/s that this character will move sideways")]
    public float speedSidestep = 3;
    [Range(0, 3600)]
    public float speedTurn = 90;
    // TELEPORT
    [Range(-25, 25)]
    [Tooltip("The distance in metres that this character will teleport")]
    public float teleportDistance = 2.5f;
    public bool teleportThroughWalls = false;
    public bool teleportToTarget = true;
    public Vector3 teleportTarget = Vector3.zero;
    // GLIDE
    [Range(0, 10)]
    public float glideDownToForward = 0.9f;
    // THRUST
    public SequencerGradient thrustSequencer;
    public AnimationCurve thrustCurve = new AnimationCurve();
    // JUMP HOVER and THRUST
    [Range(0, 10000)]
    public float force = 2000;
    public bool vectoring = false;
    // GENERAL
    [Range(-1, 100)]
    [Tooltip("The number of times this character is able to perform their movement action, -1 for infinite actions")]
    public int count = 1;
    [NonSerialized] public int countCurrent = 1;
    public bool inheritVelocity = true;
}
