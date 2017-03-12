using UnityEngine;
using System.Collections;

public class ProjectileBehaviour : MonoBehaviour
{
    [SerializeField]
    private float lifetime = 4;
    [SerializeField]
    private AnimationCurve velocityCurve;
    [SerializeField]
    private float velocityMult = 500;
    //public float velocitySafe = 50;
    [SerializeField]
    private AnimationCurve lateralDrift;
    [SerializeField]
    private float lateralMult = 1;
    [SerializeField]
    private AnimationCurve verticleDrift;
    [SerializeField]
    private float verticleMult = 1;
    private Vector3 currentVelocity;
    [SerializeField]
    private GameObject hostileEffect;
    [SerializeField]
    private GameObject friendlyEffect;
    [SerializeField]
    private GameObject onCreate;
    [SerializeField]
    private GameObject onInterval;
    [SerializeField]
    private Synchronism.Synchronisations interval = Synchronism.Synchronisations.WHOLE_NOTE;
    private bool interval_dropped;
    [SerializeField]
    private GameObject onImpact;
    private float impactCD;
    [SerializeField]
    private GameObject onDeath;
    [SerializeField]
    private bool isHoming;
    [SerializeField]
    private bool dieOnImpact;

    public Controller parent;

    private Vector3 lastPosition;
    private float timestep = 1;

    private Rigidbody rigid;
    private float elapsed;

    private float velocityFlip = 1;
    // Use this for initialization
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        impactCD = 1;
        Physics.IgnoreLayerCollision(9, 9);
        lastPosition = transform.position;

        currentVelocity = new Vector3(lateralDrift.Evaluate(Random.Range(0f, 1f)) * lateralMult, verticleDrift.Evaluate(Random.Range(0f, 1f)) * verticleMult, velocityCurve.Evaluate(0) * velocityMult);

        if (onCreate != null)
        {
            GameObject inst = Instantiate(onCreate, transform.position, transform.rotation) as GameObject;
            inst.BroadcastMessage("SetParent", parent);
        }

        ((Synchronism)Blackboard.Global[Literals.StringLiterals.Blackboard.Synchroniser].Value).synchronisers[interval].RegisterCallback(this, Delegate_OnTime);
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime * timestep;
        impactCD += Time.deltaTime;

        transform.position = Vector3.Lerp(lastPosition, transform.position, timestep);
        timestep = Mathf.Lerp(timestep, 1, 0.01f);
        lastPosition = transform.position;

        float percent = elapsed / lifetime;

        currentVelocity.z = velocityCurve.Evaluate(percent) * velocityMult;
        rigid.velocity = transform.TransformVector(currentVelocity * velocityFlip);//transform.forward * /*velocitySafe;//*/velocity.Evaluate(percent);

        if (percent >= 1)
            Destroy();
    }

    void Delegate_OnTime()
    {
        if (onInterval != null)
        {
            GameObject inst = Instantiate(onInterval, transform.position, transform.rotation) as GameObject;
            inst.BroadcastMessage("SetParent", parent);
        }
    }

    void Destroy()
    {
        if (onDeath != null)
        {
            GameObject inst = Instantiate(onDeath, transform.position, transform.rotation) as GameObject;
            inst.BroadcastMessage("SetParent", parent);
        }

        ((Synchronism)Blackboard.Global[Literals.StringLiterals.Blackboard.Synchroniser].Value).synchronisers[interval].UnregisterCallback(this);

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (impactCD >= 0.1f)
        {
            GameObject inst = Instantiate(onImpact, transform.position, transform.rotation) as GameObject;
            inst.BroadcastMessage("SetParent", parent, SendMessageOptions.DontRequireReceiver);
            impactCD = 0;
        }

        Controller target = collision.gameObject.GetComponent<Controller>();

        if (target != null)
        {
            if (hostileEffect != null)
            {
                if (target.Faction != parent.Faction)
                {
                    GameObject effect = Instantiate(hostileEffect, Vector3.zero, new Quaternion()) as GameObject;
                    effect.transform.parent = collision.gameObject.transform;
                    effect.BroadcastMessage("SetParent", target);
                }
            }

            if (friendlyEffect != null)
            {
                if (target.Faction == parent.Faction)
                {
                    GameObject effect = Instantiate(friendlyEffect, Vector3.zero, new Quaternion()) as GameObject;
                    effect.transform.parent = collision.gameObject.transform;
                    effect.BroadcastMessage("SetParent", target);
                }
            }
        }

        if (dieOnImpact)
            Destroy();
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

    public void Flip()
    {
        velocityFlip *= -1;
    }
}
