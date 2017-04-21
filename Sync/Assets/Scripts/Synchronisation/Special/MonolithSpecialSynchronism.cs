using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[AddComponentMenu("Scripts/Synchronisation/Special/Monolith Synchronism")]
public class MonolithSpecialSynchronism : MonoBehaviour
{
    public Synchronism synchronism = new Synchronism();
    public MusicPlayer musicPlayer;
    public MenuSelectionController menu;

    [Header("References")]
    public Material materialEye;
    public LineRendererManager lineManager;

    [Header("Delay")]
    public Synchronism.Synchronisations delaySynchronisation = Synchronism.Synchronisations.BAR_8;
    public Synchroniser delaySynchroniser;
    public int delayBy = 1;
    public int delayCurrent = 0;

    [Header("Delay Specifics")]
    public AnimationCurve delaySpotlightIntensity;
    public Light delaySpotlight;

    [Header("Sequencing")]
    public Synchronism.Synchronisations sequencerSynchronisation = Synchronism.Synchronisations.BAR_8;
    public Synchroniser sequencerSynchroniser;
    public MonolithSequence sequenceIntro;
    public MonolithSequence sequenceIdle;
    public MonolithSequence sequenceFormation;
    public MonolithSequence sequenceAttack;
    public MonolithSequence sequenceSpawn;
    public int sequenceIncrementation;
    public MonolithSequence sequenceCurrent;

    public List<MonolithSequence> sequenceQueue;

    private Vector3[] lineEndOriginals;

    void Start()
    {
        synchronism.Initialise();

        // This establishes the focus point for all cubes made by this monolith
        Blackboard.Global.Add(Literals.Strings.Blackboard.Locations.CubeGatheringPointOne, new BlackboardValue() { Value = new Vector3(0, 0, 0) });
        Blackboard.Global.Add("GROSS@MonolithSpecialSynchronism.Start:lineManager", new BlackboardValue() { Value = lineManager });

        lineEndOriginals = new Vector3[lineManager.lines.Length];
        for (int i = 0; i < lineManager.lines.Length; i++)
        {
            lineEndOriginals[i] = lineManager.lines[i].endPoint;
        }

        Blackboard.Global.Add("GROSS@MonolithSpecialSynchronism.Start:lineEndOriginals", new BlackboardValue() { Value = lineEndOriginals });

        if (delaySynchronisation == sequencerSynchronisation)
        {
            sequencerSynchroniser = delaySynchroniser = synchronism.synchronisers[delaySynchronisation];
            delaySynchroniser.RegisterCallback(this, CallbackBoth);
            CallbackBoth();
        }
        else
        {
            delaySynchroniser = synchronism.synchronisers[delaySynchronisation];
            delaySynchroniser.RegisterCallback(this, CallbackDelay);

            sequencerSynchroniser = synchronism.synchronisers[sequencerSynchronisation];
            sequencerSynchroniser.RegisterCallback(this, CallbackSequencer);
        }
    }

    void Update()
    {
        synchronism.Update();

        if (!menu.isHidden)
            return;

        if (delayCurrent < delayBy)
            delaySpotlight.intensity = delaySpotlightIntensity.Evaluate((delaySynchroniser.Percent + delayCurrent) / delayBy);

        materialEye.SetColor("_EmissionColor", delaySpotlight.color * delaySpotlight.intensity);
    }

    private void CallbackBoth()
    {
        if (!menu.isHidden)
            return;

        CallbackDelay();
        CallbackSequencer();
    }

    private void CallbackDelay()
    {
        if (!menu.isHidden)
            return;

        if (delayCurrent < delayBy)
        {
            delayCurrent++;
        }

        if (delayCurrent == delayBy)
        {
            delaySpotlight.intensity = delaySpotlightIntensity.Evaluate(1);
            delayCurrent++;
        }
    }

    private void CallbackSequencer()
    {
        if (!menu.isHidden)
            return;

        if (delayCurrent >= delayBy)
        {
            if (sequenceQueue.Count == 0)
            {
                sequenceIncrementation++;

                // This is where the survival magic happens
                // I want to start off simple, give them a wave of spawns, then an idle
                // Then an attack then an idle, then a spawn and an attack, but always leave an idle between waves
                // So how do I do this with the least possible effort?

                // Lets try this:
                // Difficulty defines pattern, at first the patterns are predictable, hard coded, but later, they become randomized parameterized sequences

                // This is the first wave, it is the intro, and then a spawn wave
                if (sequenceIncrementation == 1)
                {
                    sequenceQueue.Add(sequenceIntro);
                    sequenceQueue.Add(sequenceSpawn);
                }
                // This is the second wave, it is an attack wave
                else if (sequenceIncrementation == 2)
                {
                    sequenceQueue.Add(sequenceAttack);
                }
                // This is the third wave, it is a spawn wave, followed by an attack wave
                else if (sequenceIncrementation == 3)
                {
                    sequenceQueue.Add(sequenceSpawn);
                    sequenceQueue.Add(sequenceAttack);
                }
                // This is the fourth wave, it is a formation wave, followed by an attack wave
                else if (sequenceIncrementation == 4)
                {
                    sequenceQueue.Add(sequenceFormation);
                    sequenceQueue.Add(sequenceAttack);
                }
                // This is the fifth and further wave, these follow a simple scaling pattern, with an amount of randomness
                else
                {
                    // The number of sequences before an idle is defined as the ceiling of the square root of the sequence incrementation
                    int sequences = Mathf.CeilToInt(Mathf.Sqrt(sequenceIncrementation));
                    
                    for (int i = 0; i < sequences; i++)
                    {
                        MonolithSequence seq = sequenceAttack;

                        float r = UnityEngine.Random.value;

                        if (r > 0.666)
                        {
                            seq = sequenceFormation;
                        }
                        else if (r > 0.333)
                        {
                            seq = sequenceSpawn;
                        }

                        sequenceQueue.Add(seq);
                    }
                }

                sequenceQueue.Add(sequenceIdle);
            }

            if (sequenceCurrent != null)
                sequenceCurrent.End();

            sequenceCurrent = Instantiate(sequenceQueue[0], transform, false);
            sequenceQueue.RemoveAt(0);
        }
    }
}
