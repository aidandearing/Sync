using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Synchronisation/Beam To Orb")]
public class BeamToOrb : MonoBehaviour
{
    public Synchronism.Synchronisations ListenTo = Synchronism.Synchronisations.WHOLE_NOTE;
    public GameObject Beam;
    public static Transform Orb;

    public Color[] Colours;

    private Color goal;
    private Color start;
    private Color colour;
    private int current;

    private bool NoteWave_dropped;
    private float NoteWave_last;

    // Use this for initialization
    void Start()
    {
        SetColours();

        ((Synchronism)Blackboard.Global[Literals.StringLiterals.Blackboard.Synchroniser].Value).synchronisers[ListenTo].RegisterCallback(this, SetColours);
    }

    // Update is called once per frame
    void Update()
    {
        SetColours();

        // Lerp the colour of the light and the emitter layer of the shader
        Vector3 startHSV, endHSV, currentHSV = Vector3.zero;
        Color.RGBToHSV(start, out startHSV.x, out startHSV.y, out startHSV.z);
        Color.RGBToHSV(goal, out endHSV.x, out endHSV.y, out endHSV.z);

        currentHSV = Vector3.Lerp(startHSV, endHSV, ((Synchronism)Blackboard.Global[Literals.StringLiterals.Blackboard.Synchroniser].Value).synchronisers[ListenTo].Percent);
        colour = Color.HSVToRGB(currentHSV.x, currentHSV.y, currentHSV.z);

        Renderer rend = GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Standard");
        rend.material.SetColor("_EmissionColor", colour);

        GetComponent<Light>().color = colour;

        //Beam.SetColors(start, colour);
        //Renderer beam_rend = Beam.GetComponent<Renderer>();
        //beam_rend.material.shader = Shader.Find("Standard");
        //beam_rend.material.SetColor("_EmissionColor", colour);

        LineRenderer beam = Beam.GetComponent<LineRenderer>();
        beam.SetPosition(1, Orb.position);
        beam.endWidth = 10 * (1 + (Orb.position.y - transform.position.y) / 1000);
    }

    void SetColours()
    {
        int temp = Random.Range(0, Colours.Length - 1);
        while (current == temp)
        {
            temp = Random.Range(0, Colours.Length - 1);
        }
        current = temp;
        start = goal;
        goal = Colours[current];
    }
}