using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AnimationLookAt : MonoBehaviour
{
    public Animator animator;

    public Vector3 lookAt;
    public float lookSpeed = 90.0f;

    public float weight = 0.5f;
    public float weightBody = 0.5f;
    public float weightHead = 1.0f;
    public float weightClamp = 0.5f;

    void OnAnimatorIK(int layerIndex)
    {
        animator.SetLookAtPosition(lookAt);
        animator.SetLookAtWeight(weight, weightBody, weightHead, 1, weightClamp);
    }
}