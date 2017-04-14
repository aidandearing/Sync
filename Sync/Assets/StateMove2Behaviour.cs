using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMove2Behaviour : StateMachineBehaviour
{
    public Controller controller;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(Literals.Strings.Parameters.Animation.IsMoving2, true);

        //Vector3 jumpVector = new Vector3(animator.GetFloat(Literals.Strings.Parameters.Animation.JumpX), animator.GetFloat(Literals.Strings.Parameters.Animation.JumpY), animator.GetFloat(Literals.Strings.Parameters.Animation.JumpZ));

        if (controller == null)
            controller = animator.gameObject.GetComponent<Controller>();
        //animator.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(animator.GetFloat(Literals.Strings.Parameters.Animation.JumpX), animator.GetFloat(Literals.Strings.Parameters.Animation.JumpY), animator.GetFloat(Literals.Strings.Parameters.Animation.JumpZ)), ForceMode.Impulse);

        //MovementActions.Jump(controller, jumpVector);
        //controller.rigidbody.AddForce(new Vector3(animator.GetFloat(Literals.Strings.Parameters.Animation.JumpX), animator.GetFloat(Literals.Strings.Parameters.Animation.JumpY), animator.GetFloat(Literals.Strings.Parameters.Animation.JumpZ)), ForceMode.Impulse);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(Literals.Strings.Parameters.Animation.IsMoving2, false);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
