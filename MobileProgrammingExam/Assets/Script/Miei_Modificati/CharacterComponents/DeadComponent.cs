using UnityEngine;
using System.Collections;

public class DeadComponent : BaseComponent 
{
	void Start () {
	
	}
	
	void FixedUpdate () 
    {
        if (!m_bDead)
        {
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
        }
	}

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

    private bool m_bDead = false;
    private RaycastHit m_oRaycastHit;
    private DestroyObstacle m_oDestoryObstacle;
}

