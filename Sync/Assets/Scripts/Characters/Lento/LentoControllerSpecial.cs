using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LentoControllerSpecial : MonoBehaviour
{
    // TODO REMOVE THIS WHEN BACK TO NETWORKING, and make this class a NetworkBehaviour
    public bool isLocalPlayer = true;

    [Header("Player")]
    public Controller controller;
    public PlayerController player;

    [Header("Lento Specifics")]
    public RayQuery cameraRayQuery;

    public Synchronism.Synchronisations attackSynchronisation = Synchronism.Synchronisations.QUARTER_NOTE;
    public Synchroniser attackSynchroniser;
    public SequencerGameObjects attackPrefabs;
    public Transform attackNode;

    public static float timeAlive = 0;

    public float attackTimestamp = 0.0f;

    public bool isInitialised = false;

    public void Initialise()
    {
        if (!isInitialised)
        {
            isInitialised = true;

            Synchronism synch = ((Synchronism)Blackboard.Global[Literals.Strings.Blackboard.Synchronisation.Synchroniser].Value);
            if (synch != null)
            {
                attackSynchroniser = synch.synchronisers[attackSynchronisation];
                attackSynchroniser.RegisterCallback(this, CallbackAttack);
                controller.animator.SetBool(Literals.Strings.Parameters.Animation.IsAttackLooping, true);
            }
        }
    }

    public void CallbackAttack()
    {
        if (Time.time - attackTimestamp < attackSynchroniser.Duration * 0.75f)
        {
            GameObject inst = attackPrefabs.Evaluate();

            controller.animator.SetBool(Literals.Strings.Parameters.Animation.WantsToAttack, true);

            if (inst)
            {
                inst = Instantiate(inst, attackNode.position, Quaternion.LookRotation(cameraRayQuery.rayHitLast.point - attackNode.position));

                CubeController target = null;
                if (cameraRayQuery.rayHitLast.collider != null)
                {
                    if (cameraRayQuery.rayHitLast.collider.gameObject.tag == "cube")
                    {
                        target = cameraRayQuery.rayHitLast.collider.gameObject.GetComponent<CubeController>();
                    }
                }

                SynchronisedProjectileBehaviour proj = inst.GetComponent<SynchronisedProjectileBehaviour>();
                proj.parent = controller;
                proj.origin = attackNode.position;
                proj.direction = cameraRayQuery.rayHitLast.point - attackNode.position;
                //inst.GetComponent<LentoLaserAttack>().Begin(target, attackNode, cameraRayQuery.rayHitLast, cameraRayQuery.ray.direction);
            }
        }
        else
        {
            controller.animator.SetBool(Literals.Strings.Parameters.Animation.WantsToAttack, false);
        }
    }

    // Fixed Update is called once per physics step
    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        if (!isInitialised)
            Initialise();

        timeAlive += Time.fixedDeltaTime;

        if (Input.GetAxis(Literals.Strings.Input.Standard.Fire + player.Player) > 0)
        {
            attackTimestamp = Time.time;
        }

        //if ((float)controller.statistics["health"].Value <= 0)
        //{
        //    AsyncOperation load = SceneManager.LoadSceneAsync("gameover", LoadSceneMode.Single);
        //    load.allowSceneActivation = true;
        //}

        // Set the forward vector of the general controller to the proper x and z components
        // These are defined as the amount of camera forward as can be projected onto both the right and forward.
        controller.animator.SetFloat(Literals.Strings.Parameters.Animation.Vector.Forward(0), Vector3.Dot(cameraRayQuery.ray.direction, transform.right));
        controller.animator.SetFloat(Literals.Strings.Parameters.Animation.Vector.Forward(2), Vector3.Dot(cameraRayQuery.ray.direction, transform.forward));
    }
}
