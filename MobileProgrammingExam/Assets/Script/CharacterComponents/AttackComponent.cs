using UnityEngine;
using System.Collections;

//Attack Component: manage the attack fase invoked from player or from sprint
public class AttackComponent : BaseComponent 
{
    //Chech if the raycast has hit an obstacle, if so destroy it
    public void DirectAttack(RaycastHit oRaycastHit)
    {
        m_oDestoryObstacle = oRaycastHit.transform.GetComponent<DestroyObstacle>();
        if (m_oDestoryObstacle != null)
        {
            m_oDestoryObstacle.DestoryObstacle();
        }
    }

    private void OnEnable()
    {
        //Registration to attack input
        m_oInputManager.OnAttack += OnAttack;
        m_oCharacterFSM.OnStateExitEvent += OnExitAttack;
    }

    private void OnDisable()
    {
        //Deregistration to attack input
        m_oInputManager.OnAttack -= OnAttack;
        m_oCharacterFSM.OnStateExitEvent -= OnExitAttack;
    }

    private void OnAttack()
    {
        //Ask to CharacterFSM if it can attack
        m_bAttacking = m_oCharacterFSM.Attack();
    }

    //Animator FSM Callback
    private void OnExitAttack(CharacterFSM.AnimationState eAnimationState)
    {
        if(eAnimationState == CharacterFSM.AnimationState.ATTACK)
        {
            m_bAttacking = false;
        }
    }

    private void FixedUpdate()
    {
        if(m_bAttacking)
        {
            AttackCheck(m_tAttackDownCheckDirection);
            AttackCheck(m_tAttackUpCheckDirection);
        }
    }

    //Check if in tDirection there is an obstacle
    private void AttackCheck(Transform tDirection)
    {
        if (Physics.Raycast(tDirection.position, tDirection.forward, out m_oRaycastHit, m_fRaycastMaxDistanceAttackCheck, m_oLayerMaskAttack))
        {
            DirectAttack(m_oRaycastHit);
        }
    }

    [SerializeField] private float m_fRaycastMaxDistanceAttackCheck = 1f;
    [SerializeField] private LayerMask m_oLayerMaskAttack;
    [SerializeField] private Transform m_tAttackUpCheckDirection;
    [SerializeField] private Transform m_tAttackDownCheckDirection;
    private bool m_bAttacking = false;
    private RaycastHit m_oRaycastHit;
    private DestroyObstacle m_oDestoryObstacle;
}
