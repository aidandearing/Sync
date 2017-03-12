using UnityEngine;
using System.Collections;

public class SkillManager : MonoBehaviour
{
    [SerializeField]
    private GameObject parent;
    [SerializeField]
    private Transform aimNode;
    [SerializeField]
    private GameObject passive;
    [SerializeField]
    private GameObject skill1;
    private SkillScript skill1script;
    [SerializeField]
    private GameObject skill2;
    private SkillScript skill2script;
    [SerializeField]
    private GameObject skill3;
    private SkillScript skill3script;
    [SerializeField]
    private GameObject skill4;
    private SkillScript skill4script;
    [SerializeField]
    private GameObject grenade;
    private SkillScript grenadescript;

    // Use this for initialization
    void Start()
    {
        GameObject create = Instantiate(passive, transform.position, new Quaternion()) as GameObject;
        create.transform.parent = aimNode;
        create.SendMessage("SetParent", parent.GetComponent<Controller>());

        SetSkill1To(skill1);
        SetSkill2To(skill2);
        SetSkill3To(skill3);
        SetSkill4To(skill4);
        SetGrenadeTo(grenade);
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetButtonDown("Skill1"))
        {
            skill1script.Use();
        }

        if (Input.GetButtonDown("Skill2"))
        {
            skill2script.Use();
        }

        if (Input.GetButtonDown("Skill3"))
        {
            skill3script.Use();
        }

        if (Input.GetButtonDown("Skill4"))
        {
            skill4script.Use();
        }

        if (Input.GetButtonDown("Grenade"))
        {
            grenadescript.Use();
        }
    }

    public void SetSkill1To(GameObject skill)
    {
        skill1 = Instantiate(skill, transform.position, new Quaternion()) as GameObject;
        skill1.transform.parent = transform.parent;
        skill1script = skill1.GetComponent<SkillScript>();
        skill1script.SetAimTransform(aimNode);
        skill1script.SetParent(parent);
    }

    public void SetSkill2To(GameObject skill)
    {
        skill2 = Instantiate(skill, transform.position, new Quaternion()) as GameObject;
        skill2.transform.parent = transform.parent;
        skill2script = skill2.GetComponent<SkillScript>();
        skill2script.SetAimTransform(aimNode);
        skill2script.SetParent(parent);
    }

    public void SetSkill3To(GameObject skill)
    {
        skill3 = Instantiate(skill, transform.position, new Quaternion()) as GameObject;
        skill3.transform.parent = transform.parent;
        skill3script = skill3.GetComponent<SkillScript>();
        skill3script.SetAimTransform(aimNode);
        skill3script.SetParent(parent);
    }

    public void SetSkill4To(GameObject skill)
    {
        skill4 = Instantiate(skill, transform.position, new Quaternion()) as GameObject;
        skill4.transform.parent = transform.parent;
        skill4script = skill4.GetComponent<SkillScript>();
        skill4script.SetAimTransform(aimNode);
        skill4script.SetParent(parent);
    }

    public void SetGrenadeTo(GameObject skill)
    {
        grenade = Instantiate(skill, transform.position, new Quaternion()) as GameObject;
        grenade.transform.parent = transform.parent;
        grenadescript = grenade.GetComponent<SkillScript>();
        grenadescript.SetAimTransform(aimNode);
        grenadescript.SetParent(parent);
    }
}
