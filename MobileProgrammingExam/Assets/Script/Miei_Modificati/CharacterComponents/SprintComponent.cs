using UnityEngine;
using System.Collections;

public class SprintComponent : BaseComponent 
{
    void Start()
    {
        m_oSprintTimer = gameObject.AddComponent<Timer>();
    }


	void OnEnable () 
    {
        m_oInputManager.OnSprint += Sprint;
	}
	
	void OnDiable () 
    {
        m_oInputManager.OnSprint -= Sprint;
	}

    void FixedUpdate()
    {
        if(m_bSprint)
        {
            AttackCheck(m_tAttackDownCheckDirection);
            AttackCheck(m_tAttackUpCheckDirection);
            JumpCheck(m_tAutoJumpCheckDirection);
        }
    }

    private void AttackCheck(Transform tDirection)
    {
        if (Physics.Raycast(tDirection.position, tDirection.forward, out m_oRaycastHit, m_fRaycastMaxDistanceAttackCheck, m_oLayerMaskAttack))
        {
            if (m_oCharacterFSM.CurrentState == CharacterFSM.AnimationState.ATTACK_SPRINT || m_oCharacterFSM.AttackSprint())
            {
                m_oAttackComponent.DirectAttack(m_oRaycastHit);
            }
        }
    }

    private void JumpCheck(Transform tDirection)
    {
        if (Physics.Raycast(m_tAutoJumpCheckDirection.position, m_tAutoJumpCheckDirection.forward, out m_oRaycastHit, m_fRaycastMaxDistanceJumpCheck, m_oLayerMaskJump))
        {
            if (m_oGravityComponent.IsGrounded && m_oCharacterFSM.JumpSprint())
            {
                m_oGravityComponent.Jump(m_fJumpSprintIntesity);
            }
        }
    }

    private void Sprint(Vector2 v2Direction)
    {
        if (m_bCanSprint)
        {
            m_bCanSprint = false;
            m_oSprintTimer.StartTimer(m_fSprintTime, EndSprint);
            m_oMove.Run();
            m_oCharacterFSM.RunSprint();
            m_bSprint = true;
        }
    }

    private void EndSprint()
    {
        m_oMove.Walk();
        m_oCharacterFSM.Walk();
        m_bSprint = false;
        m_oSprintTimer.StartTimer(m_fRecoverSprintTime, EndRecover, true);
    }
    
    private void EndRecover()
    {
        m_bCanSprint = true;
    }

    public bool IsSprinting
    {
        get { return m_bSprint; }
    }

    private void OnDrawGizmos()
    {
        DrawLineAndPoints(m_tAutoJumpCheckDirection.position, m_tAutoJumpCheckDirection.position + m_tAutoJumpCheckDirection.forward * m_fRaycastMaxDistanceJumpCheck);
        DrawLineAndPoints(m_tAttackDownCheckDirection.position, m_tAttackDownCheckDirection.position + m_tAttackDownCheckDirection.forward * m_fRaycastMaxDistanceAttackCheck);
        DrawLineAndPoints(m_tAttackUpCheckDirection.position, m_tAttackUpCheckDirection.position + m_tAttackUpCheckDirection.forward * m_fRaycastMaxDistanceAttackCheck);
    }

    private void DrawLineAndPoints(Vector3 v3StartPoint, Vector3 v3EndPoint)
    {
        Gizmos.DrawLine(v3StartPoint, v3EndPoint);
        Gizmos.DrawSphere(v3StartPoint, 0.1f);
        Gizmos.DrawSphere(v3EndPoint, 0.1f);
    }


    [Header("Setup")]
    [SerializeField] private MoveForwardComponent m_oMove;
    [SerializeField] private RunningGravityComponent m_oGravityComponent;
    [SerializeField] private AttackComponent m_oAttackComponent;
    [SerializeField] private LayerMask m_oLayerMaskAttack;
    [SerializeField] private Transform m_tAutoJumpCheckDirection;
    [SerializeField] private Transform m_tAttackUpCheckDirection;
    [SerializeField] private Transform m_tAttackDownCheckDirection;
    [SerializeField] private LayerMask m_oLayerMaskJump;
    [Header("Tuning")]
    [SerializeField] private float m_fRaycastMaxDistanceAttackCheck = 1f;
    [SerializeField] private float m_fRaycastMaxDistanceJumpCheck = 1f;
    [SerializeField] private float m_fSprintTime;
    [SerializeField] private float m_fRecoverSprintTime;
    [SerializeField] private float m_fJumpSprintIntesity;

    private Timer m_oSprintTimer;
    private bool m_bSprint = false;
    private bool m_bCanSprint = true;
    private RaycastHit m_oRaycastHit;
    private DestroyObstacle m_oDestoryObstacle;
}
