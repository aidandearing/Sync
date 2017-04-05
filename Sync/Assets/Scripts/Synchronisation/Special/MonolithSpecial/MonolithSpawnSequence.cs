using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MonolithSpawnSequence : MonoBehaviour
{
    [Header("Synchronisation")]
    public Synchronism.Synchronisations synchronisation = Synchronism.Synchronisations.BAR_8;
    public Synchroniser synchroniser;
    public bool overLife;
    private float lastPercent;

    [Header("Spawning")]
    public Synchronism.Synchronisations spawningSynchronisation = Synchronism.Synchronisations.QUARTER_NOTE;
    public float spawnDelayPercent = 0.49f;
    public Synchroniser spawningSynchroniser;
    public GameObject spawningPrefab;
    public GameObject spawningInstance;
    public int index = 0;

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

        Vector3 endPosition = new Vector3((index - 1) % 4 - 2, (index - 1) / 4, -6) * 5;

        if (spawningInstance != null)
        {
            spawningInstance.transform.position = Vector3.Lerp(transform.position, endPosition, spawningSynchroniser.Percent);
        }
    }

    void CallbackSynchronisation()
    {
        //Destroy(gameObject);
    }

    void CallbackSpawn()
    {
        spawningInstance = null;

        if (synchroniser.Percent > spawnDelayPercent)
        {
            spawningInstance = Instantiate(spawningPrefab, transform.position, new Quaternion());
            index++;
        }
    }
}
