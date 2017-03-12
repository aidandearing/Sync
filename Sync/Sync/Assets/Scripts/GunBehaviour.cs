using UnityEngine;
using System.Collections;

public class GunBehaviour : MonoBehaviour
{
    [SerializeField]
    private SequencerGameObjects projectile;
    private GameObject highPriority;
    [SerializeField]
    private Synchronism.Synchronisations fireRate;
    [SerializeField]
    private bool hasClip;
    [SerializeField]
    private int clipSize;
    [SerializeField]
    private Synchronism.Synchronisations reloadRate;
    [SerializeField]
    private bool reloadIsMagazine;

    private int currentClip;

    private bool wantsToFire;
    private bool reallywantsToFire;

    private bool wantsToReload;

    private float timeWantingToFire;

    // Use this for initialization
    void Start()
    {
        wantsToFire = false;

        currentClip = clipSize;
        wantsToReload  = false;

        if (fireRate != reloadRate)
        {
            ((Synchronism)Blackboard.Global[Literals.StringLiterals.Blackboard.Synchroniser].Value).synchronisers[fireRate].RegisterCallback(this, Delegate_OnTime_Fire);
            ((Synchronism)Blackboard.Global[Literals.StringLiterals.Blackboard.Synchroniser].Value).synchronisers[reloadRate].RegisterCallback(this, Delegate_OnTime_Reload);
        }
        else
        {
            ((Synchronism)Blackboard.Global[Literals.StringLiterals.Blackboard.Synchroniser].Value).synchronisers[fireRate].RegisterCallback(this, Delegate_OnTime_Both);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (wantsToFire)
        {
            if (!reallywantsToFire)
                timeWantingToFire += Time.deltaTime;

            if (timeWantingToFire >= 0.5f)
            {
                timeWantingToFire = 0;
                wantsToFire = false;
            }
        }

        reallywantsToFire = false;
    }

    void Shoot()
    {
        wantsToFire = true;
        reallywantsToFire = true;
    }

    void Shoot(GameObject obj)
    {
        highPriority = obj;
        wantsToFire = true;
    }

    void Fire()
    {
        if (!hasClip || (hasClip && currentClip > 0))
        {
            GameObject spawn;

            if (highPriority == null)
            {
                spawn = Instantiate(projectile.Evaluate(), transform.position, transform.rotation) as GameObject;
            }
            else
            {
                spawn = Instantiate(highPriority, transform.position, transform.rotation) as GameObject;
            }

            spawn.BroadcastMessage("SetParent", transform.parent.GetComponent<Controller>());
            Physics.IgnoreCollision(spawn.GetComponent<Collider>(), transform.parent.GetComponent<Collider>());

            wantsToFire = false;

            if (hasClip)
                currentClip--;
        }
        else
        {
            wantsToReload = true;
        }
    }

    void Reload()
    {
        if (reloadIsMagazine)
        {
            currentClip = clipSize;
        }
        else if (currentClip < clipSize)
        {
            currentClip++;
        }

        if (currentClip >= clipSize)
            wantsToReload = false;
    }

    void Delegate_OnTime_Fire()
    {
        CanFireLogic();
    }

    void Delegate_OnTime_Reload()
    {
        if (hasClip)
            CanReloadLogic();
    }

    void Delegate_OnTime_Both()
    {
        CanFireLogic();
        if (hasClip)
            CanReloadLogic();
    }

    void CanFireLogic()
    {
        if (wantsToFire)
            Fire();
    }

    void CanReloadLogic()
    {
        if (wantsToReload)
            Reload();
    }
}
