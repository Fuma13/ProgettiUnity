using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class RunningGravityComponent : BaseComponent 
{
    public void JumpSprint(float fJumpIntencity)
    {
        m_oRigidBodyTarget.AddForce(m_tTarget.up * m_oRigidBodyTarget.mass
                            * fJumpIntencity, ForceMode.Impulse);
    }

    private void Awake()
    {
        Assert.IsNotNull(m_oRigidBodyTarget, "Missing rigidbody target in GravityComponent");

        m_oRigidBodyTarget.isKinematic = true;
    }

    private void OnEnable()
    {
        m_oGameManager.OnStartEvent += OnStart;
        m_oInputManager.OnJump += OnJump;
    }

    private void OnDisable()
    {
        m_oGameManager.OnStartEvent -= OnStart;
        m_oInputManager.OnJump -= OnJump;
    }
	
	private void OnStart ()
	{
		m_oRigidBodyTarget.isKinematic = false;
	}

    private void OnJump()
    {
        if (m_bGround && m_oCharacterFSM.Jump())
        {
            m_oRigidBodyTarget.AddForce(m_tTarget.up * m_oRigidBodyTarget.mass
                                * m_fJumpIntencity, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        m_bGround = Physics.Raycast(m_tTarget.position - (-m_tTarget.up * m_fRaycastMaxDistance), -m_tTarget.up, m_fRaycastMaxDistance * 2, m_oLayerMask);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(m_tTarget.position - (-m_tTarget.up * m_fRaycastMaxDistance), m_tTarget.position + -m_tTarget.up * m_fRaycastMaxDistance);
        Gizmos.DrawSphere(m_tTarget.position - (-m_tTarget.up * m_fRaycastMaxDistance), 0.1f);
        Gizmos.DrawSphere(m_tTarget.position + (-m_tTarget.up * m_fRaycastMaxDistance), 0.1f);
    }

    public bool IsGrounded
    {
        get { return m_bGround; }
    }

	[Header("Setup")]
	[SerializeField] private Rigidbody m_oRigidBodyTarget;
    [SerializeField] private LayerMask m_oLayerMask;
	[Header("Tuning")]
	[SerializeField] private float m_fJumpIntencity = 5.0f;
    [SerializeField] private float m_fRaycastMaxDistance = 0.2f;

    private bool m_bGround;

}
