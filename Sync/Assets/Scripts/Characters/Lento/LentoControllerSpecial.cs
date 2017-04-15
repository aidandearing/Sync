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
    public PlayerController controller;

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
            }
        }
    }

    public void CallbackAttack()
    {
        if (Time.time - attackTimestamp < attackSynchroniser.Duration * 0.75f)
        {
            GameObject inst = attackPrefabs.Evaluate();

            if (inst)
            {
                inst = Instantiate(inst, transform.position, new Quaternion());

                CubeController target = null;
                if (cameraRayQuery.rayHitLast.collider != null)
                {
                    if (cameraRayQuery.rayHitLast.collider.gameObject.tag == "cube")
                    {
                        target = cameraRayQuery.rayHitLast.collider.gameObject.GetComponent<CubeController>();
                    }
                }

                inst.GetComponent<LentoLaserAttack>().Begin(target, attackNode, cameraRayQuery.rayHitLast, cameraRayQuery.ray.direction);
            }
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

        if (Input.GetAxis(Literals.Strings.Input.Controller.TriggerRight) > 0)
        {
            attackTimestamp = Time.time;
        }

        if ((float)controller.controller.statistics["health"].Value <= 0)
        {
            AsyncOperation load = SceneManager.LoadSceneAsync("gameover", LoadSceneMode.Single);
            load.allowSceneActivation = true;
        }
    }
}
