using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MonolithFormationSequence : MonoBehaviour
{
    [Header("Synchronisation")]
    public Synchronism.Synchronisations synchronisation = Synchronism.Synchronisations.BAR_8;
    public Synchroniser synchroniser;
    public bool overLife;
    private float lastPercent;

    [Header("Spawning")]
    public Synchronism.Synchronisations spawningSynchronisation = Synchronism.Synchronisations.QUARTER_NOTE;
    public int spawnDelay = 63;
    public int spawnDelayCurrent = 0;
    public Synchroniser spawningSynchroniser;
    public GameObject spawningPrefab;
    public Transform formationPoint;

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
            Synchronism synch = ((Synchronism)Blackboard.Global[Literals.Strings.Blackboard.Synchronisation.Synchroniser].Value);
            if (synch != null)
            {
                synchroniser = synch.synchronisers[synchronisation];
                synchroniser.RegisterCallback(this, CallbackSynchronisation);

                spawningSynchroniser = synch.synchronisers[spawningSynchronisation];
                spawningSynchroniser.RegisterCallback(this, CallbackSpawn);

                lastPercent = synchroniser.Percent;

                isInitialised = true;
            }
        }

        lineEndOriginals = new Vector3[lineManager.lines.Length];
        for (int i = 0; i < lineManager.lines.Length; i++)
        {
            lineEndOriginals[i] = lineManager.lines[i].endPoint;
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

    void CallbackSynchronisation()
    {
        //Destroy(gameObject);
    }

    void CallbackSpawn()
    {
        if (spawnDelayCurrent < spawnDelay)
            spawnDelayCurrent++;

        if (spawnDelayCurrent == spawnDelay)
        {
            Instantiate(spawningPrefab, formationPoint.position, new Quaternion());
            spawnDelayCurrent++;
        }
    }
}

