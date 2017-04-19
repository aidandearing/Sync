using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MonolithIdleSequence : MonolithSequence
{
    [Header("Synchronisation")]
    public Synchronism.Synchronisations synchronisation = Synchronism.Synchronisations.BAR_8;
    public Synchroniser synchroniser;

    [Header("Lines")]
    public LineRendererManager lineManager;
    public AnimationCurve lineEndXMultiplier;
    public AnimationCurve lineEndYMultiplier;
    public AnimationCurve lineEndZMultiplier;

    private Vector3[] lineEndOriginals;

    private bool isInitialised = false;

    void Initialise()
    {
        if (!isInitialised)
        {
            lineManager = Blackboard.Global["GROSS@MonolithSpecialSynchronism.Start:lineManager"].Value as LineRendererManager;
            lineEndOriginals = Blackboard.Global["GROSS@MonolithSpecialSynchronism.Start:lineEndOriginals"].Value as Vector3[];

            Synchronism synch = ((Synchronism)Blackboard.Global[Literals.Strings.Blackboard.Synchronisation.Synchroniser].Value);
            if (synch != null)
            {
                synchroniser = synch.synchronisers[synchronisation];
                //synchroniser.RegisterCallback(this, CallbackSynchronisation);

                isInitialised = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialised)
            Initialise();

        if (lastPercent > synchroniser.Percent)
            overLife = true;

        lastPercent = synchroniser.Percent;

        if (overLife)
            return;

        Vector3 lineEndMult = new Vector3(lineEndXMultiplier.Evaluate(synchroniser.Percent),
            lineEndYMultiplier.Evaluate(synchroniser.Percent),
            lineEndZMultiplier.Evaluate(synchroniser.Percent));

        for (int i = 0; i < lineManager.lines.Length; i++)
        {
            Vector3 point = new Vector3(lineEndOriginals[i].x * lineEndMult.x, lineEndOriginals[i].y * lineEndMult.y, lineEndOriginals[i].z * lineEndMult.z);
            lineManager.lines[i].endPoint = point;
        }
    }

    public override void End()
    {

    }
}

