using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishWeaponSwitch : StateMachineBehaviour
{
    [SerializeField] private CogschanAnimationController _controller;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Do stuff
    }
}
