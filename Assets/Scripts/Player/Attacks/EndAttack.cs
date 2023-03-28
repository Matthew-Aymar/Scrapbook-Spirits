using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAttack : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>().inAttackAnim = false;
    }
}
