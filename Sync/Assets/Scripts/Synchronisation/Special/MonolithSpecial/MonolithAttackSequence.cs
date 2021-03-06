﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MonolithAttackSequence : MonolithSequence
{
    [Header("Synchronisation")]
    public Synchronism.Synchronisations synchronisation = Synchronism.Synchronisations.BAR_8;
    public Synchroniser synchroniser;

    [Header("Spawning")]
    public Synchronism.Synchronisations attackSynchronisation = Synchronism.Synchronisations.QUARTER_NOTE;
    public Synchroniser attackSynchroniser;
    public SequencerGameObjects attackPrefabs;

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
            attackPrefabs = new SequencerGameObjects() { objs = attackPrefabs.objs };

            lineManager = Blackboard.Global["GROSS@MonolithSpecialSynchronism.Start:lineManager"].Value as LineRendererManager;
            lineEndOriginals = Blackboard.Global["GROSS@MonolithSpecialSynchronism.Start:lineEndOriginals"].Value as Vector3[];

            Synchronism synch = ((Synchronism)Blackboard.Global[Literals.Strings.Blackboard.Synchronisation.Synchroniser].Value);
            if (synch != null)
            {
                synchroniser = synch.synchronisers[synchronisation];
                //synchroniser.RegisterCallback(this, CallbackSynchronisation);

                attackSynchroniser = synch.synchronisers[attackSynchronisation];
                attackSynchroniser.RegisterCallback(this, CallbackSpawn);
                CallbackSpawn();

                isInitialised = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialised)
            Initialise();

        Vector3 lineEndMult = new Vector3(lineEndXMultiplier.Evaluate(synchroniser.Percent),
            lineEndYMultiplier.Evaluate(synchroniser.Percent),
            lineEndZMultiplier.Evaluate(synchroniser.Percent));

        for (int i = 0; i < lineManager.lines.Length; i++)
        {
            Vector3 point = new Vector3(lineEndOriginals[i].x * lineEndMult.x, lineEndOriginals[i].y * lineEndMult.y, lineEndOriginals[i].z * lineEndMult.z);
            lineManager.lines[i].endPoint = point;
        }
    }

    void CallbackSynchronisation()
    {
        //Destroy(gameObject);
    }

    void CallbackSpawn()
    {
        GameObject inst = attackPrefabs.Evaluate(synchroniser.Percent);

        if (inst != null)
        {
            Instantiate(inst, transform, false);
        }
    }

    public override void End()
    {
        attackSynchroniser.UnregisterCallback(this);
    }
}

