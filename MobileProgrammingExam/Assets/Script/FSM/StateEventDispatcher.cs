using UnityEngine;
using System.Collections;
using System;

//Dispatch an event when the state exit
public class StateEventDispatcher : StateMachineBehaviour 
{
	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_oCharacterFSM != null)
        {
            m_oCharacterFSM.OnStateExit(m_eAnimationState);
        }
    }

    [SerializeField] CharacterFSM.AnimationState m_eAnimationState;
    private CharacterFSM m_oCharacterFSM = null;

    public CharacterFSM CharacterFSMController
    {
        set { m_oCharacterFSM = value; }
    }
}
