using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HedgehogAnimationLayer : StateMachineBehaviour
{

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.parent.GetComponent<EnemyHedgehog>().AnimationEvent_SetStateBackToIdle();
    }
}
