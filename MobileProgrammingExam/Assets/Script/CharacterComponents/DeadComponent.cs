using UnityEngine;
using System.Collections;

//Dead Component: check if the target hit some obstacle or fall in a hole
public class DeadComponent : BaseComponent 
{	
    private void OnEnable()
    {
        m_oGameManager.OnRestartEvent += Reset;
    }
    private void OnDisable()
    {
        m_oGameManager.OnRestartEvent -= Reset;
    }
	void FixedUpdate () 
    {
        if (!m_bDead)
        {
            //Check if there is an obstacle that is not destoryed
            if (Physics.Raycast(m_tTarget.position, m_tTarget.forward, out m_oRaycastHit, m_fRaycastMaxDistance, m_oLayerMask))
            {
                m_oDestoryObstacle = m_oRaycastHit.transform.GetComponent<DestroyObstacle>();
                if (m_oDestoryObstacle == null || !m_oDestoryObstacle.Destroyed)
                {
                    if (m_oGameManager.ChangeState(GameManager.GameState.DEAD))
                    {
                        m_bDead = true;
                    }
                }
            }

            //Antibug check, if the target is under a limit force the dead
            if(m_tTarget.position.y < m_fMinYToDead)
            {
                if (m_oGameManager.ChangeState(GameManager.GameState.DEAD))
                {
                    m_bDead = true;
                }
            }
        }
	}

    private void Reset()
    {
        m_bDead = false;
    }

    //Debug draw
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(m_tTarget.position, m_tTarget.position + m_tTarget.forward * m_fRaycastMaxDistance);
        Gizmos.DrawSphere(m_tTarget.position, 0.1f);
        Gizmos.DrawSphere(m_tTarget.position + (m_tTarget.forward * m_fRaycastMaxDistance), 0.1f);
    }

	[Header("Setup")]
    [SerializeField] private LayerMask m_oLayerMask;
    [SerializeField] private MoveForwardComponent m_oMoveComponent;
    [Header("Tuning")]
    [SerializeField] private float m_fRaycastMaxDistance = 0.2f;
    [SerializeField] private float m_fMinYToDead = -2.0f;

    private bool m_bDead = false;
    private RaycastHit m_oRaycastHit;
    private DestroyObstacle m_oDestoryObstacle;
}

