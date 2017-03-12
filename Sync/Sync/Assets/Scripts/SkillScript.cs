using UnityEngine;
using System.Collections;

public class SkillScript : MonoBehaviour
{
    public string name = "SKILL_NAME_MISSING";
    public string description = "SKILL_DESCRIPTION_MISSING";
    [SerializeField]
    private GameObject create;
    [SerializeField]
    private bool isAimed;
    [SerializeField]
    private bool isCreatorCollidable;
    [SerializeField]
    private Synchronism.Synchronisations skillCooldown;
    [SerializeField]
    private int skillCDCounts;
    private int skillCDCurrent;
    private float current;
    [SerializeField]
    private bool stacking;
    [SerializeField]
    private int stackMax;
    private int stacks;
    [SerializeField]
    private string stackCooldown;
    [SerializeField]
    private int stackCDCounts;
    private int stackCDCurrent;
    private float stackCurrent;

    private bool canCast;
    private bool wantsToCast;

    private bool canRegen;
    private bool wantsToRegen;

    private float timeWantingToCast;

    private GameObject parent;
    private Transform aimTransform;

    // Use this for initialization
    void Start()
    {
        skillCDCurrent = skillCDCounts / 2;
        stackCDCurrent = stackCDCounts / 2;

        ((Synchronism)Blackboard.Global[Literals.StringLiterals.Blackboard.Synchroniser].Value).synchronisers[skillCooldown].RegisterCallback(this, Delegate_OnTime);
    }

    // Update is called once per frame
    void Update()
    {
        CanFireLogic();

        if (stacking)
            CanReloadLogic();

        if (wantsToCast)
        {
            timeWantingToCast += Time.deltaTime;

            if (timeWantingToCast >= 0.5f)
            {
                timeWantingToCast = 0;
                wantsToCast = false;
            }
        }

        if (skillCDCurrent >= skillCDCounts && (!stacking || (stacking && stacks > 0)))
        {
            //if (name != "SKILL_NAME_MISSING")
            //    Debug.Log(name + " is ready");
        }
    }

    public void Use()
    {
        wantsToCast = true;
    }

    void Cast()
    {
        if (!stacking || (stacking && stacks > 0))
        {
            GameObject spawn;
            if (isAimed)
                spawn = Instantiate(create, transform.position, aimTransform.rotation) as GameObject;
            else
                spawn = Instantiate(create, transform.position, transform.rotation) as GameObject;

            spawn.SendMessage("SetParent", parent.GetComponent<Controller>());
            if (!isCreatorCollidable)
                Physics.IgnoreCollision(spawn.GetComponent<Collider>(), parent.GetComponent<Collider>());

            wantsToCast = false;

            if (stacking)
                stacks--;
        }
    }

    void Reload()
    {
        if (stacks < stackMax)
        {
            stacks++;
        }
    }

    void Delegate_OnTime()
    {
        CanFireLogic();
        CanReloadLogic();
    }

    void CanFireLogic()
    {
        if (!canCast)
        {
            canCast = true;

            skillCDCurrent++;

            if (skillCDCurrent >= skillCDCounts)
            {
                if (wantsToCast)
                {
                    skillCDCurrent = 0;
                    Cast();
                }
            }

            wantsToCast = false;
        }
    }

    void CanReloadLogic()
    {
        if (!canRegen)
        {
            canRegen = true;

            stackCDCurrent++;

            if (stackCDCurrent >= stackCDCounts)
            {
                stackCDCurrent = 0;
                Reload();
            }
        }
    }

    public void SetAimTransform(Transform transform)
    {
        aimTransform = transform;
    }

    public void SetParent(GameObject par)
    {
        parent = par;
    }
}
