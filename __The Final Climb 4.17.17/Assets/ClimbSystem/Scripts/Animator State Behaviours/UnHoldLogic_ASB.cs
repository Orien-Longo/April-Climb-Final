using UnityEngine;
using System.Collections;

public class UnHoldLogic_ASB : StateMachineBehaviour {

    Climbing.ClimbBehaviour cb;
   
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
        if(cb == null)
        {
            cb = animator.transform.GetComponent<Climbing.ClimbBehaviour>();
        }

        cb.UnHold();
	}

}
