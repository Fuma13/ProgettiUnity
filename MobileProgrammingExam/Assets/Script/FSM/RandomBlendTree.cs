using UnityEngine;
using System.Collections;

//Select an blend tree for the state
public class RandomBlendTree : StateMachineBehaviour {

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int fRandomIndex = Random.Range(0, m_afPossibleBranch.Length-1);
        animator.SetFloat(m_sRandomVariable, m_afPossibleBranch[fRandomIndex]);
    }

    [SerializeField] private float[] m_afPossibleBranch;
    [SerializeField] private string m_sRandomVariable;

}
