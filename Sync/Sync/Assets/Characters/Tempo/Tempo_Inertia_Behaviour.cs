using UnityEngine;
using System.Collections;

public class Tempo_Inertia_Behaviour : MonoBehaviour
{
    public GameObject Parent;
    public Transform Aim;

    public GameObject InertiaField;
    public GameObject Reverse;
    public GameObject Stop;
    public GameObject Blink;
    public float BlinkDisableInertiaFieldFor = 4;
    public GameObject Pause;
    public GameObject InertiaTrap;

    private SkillScript reverse;
    private SkillScript stop;
    private SkillScript blink;
    private SkillScript pause;
    private SkillScript inertiatrap;

    private bool isInertiaFieldDisabled = false;
    private float inertiaFieldDisabledTimer = 0;

    // Use this for initialization
    void Start()
    {
        InertiaField = Instantiate(InertiaField, transform.position, transform.rotation) as GameObject;
        InertiaField.BroadcastMessage("SetParent", Parent.GetComponent<Controller>());
        InertiaField.transform.parent = transform.parent;

        Reverse = Instantiate(Reverse, transform.position, transform.rotation) as GameObject;
        Reverse.transform.parent = transform.parent;
        reverse = Reverse.GetComponent<SkillScript>();
        reverse.SetParent(Parent);
        reverse.SetAimTransform(Aim);

        Stop = Instantiate(Stop, transform.position, transform.rotation) as GameObject;
        Stop.transform.parent = transform.parent;
        stop = Stop.GetComponent<SkillScript>();
        stop.SetParent(Parent);
        stop.SetAimTransform(Aim);

        Blink = Instantiate(Blink, transform.position, transform.rotation) as GameObject;
        Blink.transform.parent = transform.parent;
        blink = Blink.GetComponent<SkillScript>();
        blink.SetParent(Parent);
        blink.SetAimTransform(Aim);

        Pause = Instantiate(Pause, transform.position, transform.rotation) as GameObject;
        Pause.transform.parent = transform.parent;
        pause = Pause.GetComponent<SkillScript>();
        pause.SetParent(Parent);
        pause.SetAimTransform(Aim);

        InertiaTrap = Instantiate(InertiaTrap, transform.position, transform.rotation) as GameObject;
        InertiaTrap.transform.parent = transform.parent;
        inertiatrap = InertiaTrap.GetComponent<SkillScript>();
        inertiatrap.SetParent(Parent);
        inertiatrap.SetAimTransform(Aim);
    }

    // Update is called once per frame
    void Update()
    {
        InertiaField.SetActive(!isInertiaFieldDisabled);
        if (isInertiaFieldDisabled)
        {
            inertiaFieldDisabledTimer += Time.deltaTime;

            if (inertiaFieldDisabledTimer >= BlinkDisableInertiaFieldFor)
            {
                inertiaFieldDisabledTimer = 0;
                isInertiaFieldDisabled = false;
            }
        }

        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetButtonDown("Skill1"))
        {
            reverse.Use();
        }

        if (Input.GetButtonDown("Skill2"))
        {
            stop.Use();
        }

        if (Input.GetButtonDown("Skill3"))
        {
            blink.Use();
            isInertiaFieldDisabled = true;
        }

        if (Input.GetButtonDown("Skill4"))
        {
            pause.Use();
        }

        if (Input.GetButtonDown("Grenade"))
        {
            inertiatrap.Use();
        }
    }
}
