using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballermotherAnimationLayer : StateMachineBehaviour {


	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.transform.parent.GetComponent<EnemyHeadhogMother>().Breath_DisableVisuals();
	}

}
