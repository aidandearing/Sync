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

    [Header("References")]
    public Material materialEye;

    [Header("Delay")]
    public Synchronism.Synchronisations delaySynchronisation = Synchronism.Synchronisations.BAR_8;
    public Synchroniser delaySynchroniser;
    public int delayBy = 1;
    public int delayCurrent = 0;

    [Header("Delay Specifics")]
    public AnimationCurve delaySpotlightIntensity;
    public Light delaySpotlight;
    public GameObject delaySequence;
    public GameObject delayEndSequence;

    [Header("Sequencing")]
    public Synchronism.Synchronisations sequencerSynchronisation = Synchronism.Synchronisations.BAR_8;
    public Synchroniser sequencerSynchroniser;
    public GameObject sequenceIntro;
    public GameObject sequenceIdle;
    public GameObject sequenceFormation;
    public GameObject sequenceAttack;
    public GameObject sequenceSpawn;
    public int sequenceIncrementation;

    public List<GameObject> sequenceQueue;

    void Start()
    {
        synchronism.Initialise();

        // This establishes the focus point for all cubes made by this monolith
        Blackboard.Global.Add(Literals.Strings.Blackboard.Locations.CubeGatheringPointOne, new BlackboardValue() { Value = new Vector3(0, 0, 0) });

        if (delaySynchronisation == sequencerSynchronisation)
        {
            sequencerSynchroniser = delaySynchroniser = synchronism.synchronisers[delaySynchronisation];
            delaySynchroniser.RegisterCallback(this, CallbackBoth);
        }
        else
        {
            delaySynchroniser = synchronism.synchronisers[delaySynchronisation];
            delaySynchroniser.RegisterCallback(this, CallbackDelay);

            sequencerSynchroniser = synchronism.synchronisers[sequencerSynchronisation];
            sequencerSynchroniser.RegisterCallback(this, CallbackBoth);
        }

        Instantiate(delaySequence, transform, false);
    }

    void Update()
    {
        synchronism.Update();

        if (delayCurrent < delayBy)
            delaySpotlight.intensity = delaySpotlightIntensity.Evaluate((delaySynchroniser.Percent + delayCurrent) / delayBy);

        materialEye.SetColor("_EmissionColor", delaySpotlight.color * delaySpotlight.intensity);
    }

    private void CallbackBoth()
    {
        CallbackDelay();
        CallbackSequencer();
    }

    private void CallbackDelay()
    {
        if (delayCurrent < delayBy)
        {
            delayCurrent++;
        }

        if (delayCurrent == delayBy)
        {
            delaySpotlight.intensity = delaySpotlightIntensity.Evaluate(1);
            Instantiate(delayEndSequence, transform, false);
            delayCurrent++;
        }
    }

    private void CallbackSequencer()
    {
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

                // This is the first wave, it is a spawn wave
                if (sequenceIncrementation == 1)
                {
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
                        GameObject seq = sequenceAttack;

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

            Instantiate(sequenceQueue[0], transform, false);
            sequenceQueue.RemoveAt(0);
        }
    }
}
