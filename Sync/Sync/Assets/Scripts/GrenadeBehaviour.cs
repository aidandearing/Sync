using UnityEngine;
using System.Collections;

public class GrenadeBehaviour : MonoBehaviour
{
    [SerializeField]
    private float velocity = 25;
    [SerializeField]
    private Synchronism.Synchronisations detonationTimer = Synchronism.Synchronisations.QUARTER_NOTE;
    [SerializeField]
    private int detonationCounter = 32;
    [SerializeField]
    private GameObject detonation;

    private int detonationCurrent;

    public Controller parent;

    private float timestep = 1;
    private Vector3 lastPosition;

    // Use this for initialization
    void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * velocity;
        lastPosition = transform.position;

        ((Synchronism)Blackboard.Global[Literals.StringLiterals.Blackboard.Synchroniser].Value).synchronisers[detonationTimer].RegisterCallback(this, new Synchroniser.OnTime(Delegate_OnTime));
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(lastPosition, transform.position, timestep);
        timestep = Mathf.Lerp(timestep, 1, 0.01f);
        lastPosition = transform.position;
    }

    void Detonate()
    {
        GameObject spawn = Instantiate(detonation, transform.position, new Quaternion()) as GameObject;

        spawn.SendMessage("SetParent", parent);

        ((Synchronism)Blackboard.Global[Literals.StringLiterals.Blackboard.Synchroniser].Value).synchronisers[detonationTimer].UnregisterCallback(this);

        Destroy(gameObject);
    }

    public void SetParent(Controller par)
    {
        parent = par;
    }

    public void SetTimestep(float timestep)
    {
        if (this.timestep > timestep || timestep > 1)
            this.timestep = timestep;
    }

    public void Delegate_OnTime()
    {
        detonationCurrent++;

        if (detonationCurrent >= detonationCounter)
        {
            Detonate();
        }
    }
}

