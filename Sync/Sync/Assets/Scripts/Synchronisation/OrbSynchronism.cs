using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Scripts/Synchronisation/Orb Synchronism")]
public class OrbSynchronism : Synchronism
{
    public AudioSource Metronome;
    public Light GlobalLight;
    public ParticleSystem CameraFlashes;
    public GameObject Shockwave;
    public GameObject StartChargeEffect;
    public GameObject StartEffect;

    //public GameObject[] Effects;
    public SequencerGameObjects[] effects;

    public Vector3 PositionStart;
    public Vector3 PositionEnd;

    public float startDelay = 10;

    public ColourTransition colours;
    private Color colour;

    public static float startPercent;

    /// <summary>
    /// Intensity is the static value that is changed by Synchronism and players as the game progresses and actions are taken, that dictates how intense both the synchronism effects are
    /// and in special cases how often certain other actions occur (as can be attached to the closest 'intensity' beat by using the nearest delegates)
    /// </summary>
    public static float Intensity;
    public static float Intensity_Magnitude = 0;
    public float intensity_min = 0;
    public float intensity_max = 1000000;
    public float intensity_time = 60;
    public float intensity_exponent = 10;
    public float intensity_goal_threshold = 0.001f;
    public float intensity_goal_lerp = 0.01f;

    public static bool isStarting = true;

    private ParticleSystem.EmissionModule cameraFlashesEmitter;

    // Use this for initialization
    protected override void Start()
    {
        Initialise();

        colours.SetColour();
        Intensity = 0;

        GetComponent<Rigidbody>().useGravity = false;

        transform.position = PositionStart;
        BeamToOrb.Orb = transform;

        cameraFlashesEmitter = CameraFlashes.emission;

        Instantiate(StartChargeEffect, transform.position, new Quaternion());

        synchronisers[Synchronisations.BAR_8].RegisterCallback(this, OnTimeBar8);
        synchronisers[Synchronisations.BAR_4].RegisterCallback(this, OnTimeBar4);
        synchronisers[Synchronisations.BAR_2].RegisterCallback(this, OnTimeBar2);
        synchronisers[Synchronisations.BAR].RegisterCallback(this, OnTimeBar);
        synchronisers[Synchronisations.WHOLE_NOTE].RegisterCallback(this, OnTimeWhole);
        synchronisers[Synchronisations.HALF_NOTE].RegisterCallback(this, OnTimeHalf);
        synchronisers[Synchronisations.QUARTER_NOTE].RegisterCallback(this, OnTimeQuarter);
        synchronisers[Synchronisations.EIGHTH_NOTE].RegisterCallback(this, OnTimeEighth);
        synchronisers[Synchronisations.SIXTEENTH_NOTE].RegisterCallback(this, OnTimeSixteenth);
        synchronisers[Synchronisations.THIRTYSECOND_NOTE].RegisterCallback(this, OnTimeThirtySecond);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        // Start vs Game behaviour
        if (Time.time >= startDelay)
        {
            // The game has started and the orb should be dropped
            GetComponent<Rigidbody>().useGravity = true;

            // The orb has reached its goal position
            if (transform.position.y <= PositionEnd.y)
            {
                // One time trigger on starting being over
                if (isStarting)
                {
                    // Fixing all the values
                    isStarting = false;
                    GetComponent<Rigidbody>().useGravity = false;
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    GetComponent<Rigidbody>().isKinematic = true;
                    transform.position = PositionEnd;
                    // Displaying the start effect
                    Instantiate(StartEffect, transform.position, new Quaternion());

                    cameraFlashesEmitter.rateOverTime = new ParticleSystem.MinMaxCurve(1000, 1000);
                }
            }

            // Calculating the baseline intensity that the intensity wants to be at, a function of time elapsed since start, clamped to a maximum value
            float intensity_baseLine = Mathf.Clamp(Mathf.Pow(((Time.time - startDelay) / intensity_time), intensity_exponent), intensity_min, intensity_max);

            // Ensures the current intensity actually achieves the baseline intensity, the problem with interpolation
            if (Mathf.Abs(Intensity - intensity_baseLine) <= intensity_baseLine * intensity_goal_threshold)
            {
                Intensity = intensity_baseLine;
                Intensity_Magnitude = Mathf.Log10(Intensity);
            }
            else
            {
                Intensity = Mathf.Lerp(Intensity, intensity_baseLine, intensity_goal_lerp);
                Intensity_Magnitude = Mathf.Log10(Intensity);
            }
        }
        else
        {
            // The orb is starting, and it should grow in brightness
            float percent = Time.time / startDelay;
            percent *= percent * percent * percent * percent;
            GetComponent<Light>().intensity = percent;
        }

        // Lerp the colour of the light and the emitter layer of the shader
        colour = colours.Evaluate(synchronisers[Synchronisations.WHOLE_NOTE].Percent);

        Renderer rend = GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Standard");
        rend.material.SetColor("_EmissionColor", colour);

        GetComponent<Light>().color = colour;

        GlobalLight.color = colour;

        float cameraRate = Mathf.Clamp(100 * Mathf.Log10(Intensity), 0, 1000);

        if (Mathf.Abs(cameraFlashesEmitter.rateOverTime.constantMin - cameraRate) <= cameraRate * 0.001f)
        {

            cameraFlashesEmitter.rateOverTime = new ParticleSystem.MinMaxCurve(cameraRate, cameraRate);
        }
        else
        {
            float cfer = Mathf.Lerp(cameraFlashesEmitter.rateOverTime.constantMin, cameraRate, 0.01f);
            cameraFlashesEmitter.rateOverTime = new ParticleSystem.MinMaxCurve(cfer, cfer);
        }
    }

    protected override void OnTimeBar8()
    {

    }

    protected override void OnTimeBar4()
    {

    }

    protected override void OnTimeBar2()
    {

    }

    protected override void OnTimeBar()
    {
        //Metronome.Stop();
        Metronome.Play();
    }

    protected override void OnTimeWhole()
    {
        colours.SetColour();

        if (!isStarting)
        {
            int step = 0;

            foreach (SequencerGameObjects sequencer in effects)
            {
                GameObject obj = sequencer.Evaluate();

                if (obj != null && Intensity_Magnitude >= step)
                {
                    Instantiate(obj, transform.position, new Quaternion());
                }

                step++;
            }
        }
    }

    protected override void OnTimeHalf()
    {

    }

    protected override void OnTimeQuarter()
    {
        
    }

    protected override void OnTimeEighth()
    {

    }

    protected override void OnTimeSixteenth()
    {

    }

    protected override void OnTimeThirtySecond()
    {

    }
}
