using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

//Gravity Component: manage the vertical axe, with gravity and jump
public class RunningGravityComponent : BaseComponent 
{
    //Jump adding force to rigidbody target
    public void Jump(float fJumpIntencity)
    {
        m_oRigidBodyTarget.AddForce(m_tTarget.up * m_oRigidBodyTarget.mass
                            * fJumpIntencity, ForceMode.Impulse);
    }

    private void Awake()
    {
        Assert.IsNotNull(m_oRigidBodyTarget, "Missing rigidbody target in GravityComponent");

        //Block every force until the start
        m_oRigidBodyTarget.isKinematic = true;
        m_oJumpInputDelayTimer = gameObject.AddComponent<Timer>();
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
	
    //Eable the charater to fall down on the plane
	private void OnStart ()
	{
		m_oRigidBodyTarget.isKinematic = false;
	}

    private void OnJump()
    {
        //Check if is ground and if it can jump
        if (m_bGround && m_oCharacterFSM.Jump())
        {
            Jump(m_fJumpIntencity);
        }
        else
        {
            //Set pending the jump input
            m_bPendingJumpInput = true;
            m_oJumpInputDelayTimer.StartTimer(m_fJumpInputDelayTime, EndJumpInputDelay);
        }
    }

    private void FixedUpdate()
    {
        m_bGround = Physics.Raycast(m_tTarget.position - (-m_tTarget.up * m_fRaycastMaxDistance), -m_tTarget.up, m_fRaycastMaxDistance * 2, m_oLayerMask);
        //If there is a pending jump input and the target touch ground
        //Consume the pending jump input and invoke the jump
        //(It's useful when the player launch the jump input little before the target touch ground, so this input is not ignored)
        if (m_bPendingJumpInput && m_bGround && m_oCharacterFSM.Jump())
        {
            m_bPendingJumpInput = false;
            m_oJumpInputDelayTimer.Discard();
            Jump(m_fJumpIntencity);
        }
    }

    //Cancel pending jump input
    private void EndJumpInputDelay()
    {
        m_bPendingJumpInput = false;
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
    private Timer m_oJumpInputDelayTimer;
    private float m_fJumpInputDelayTime = 0.3f;
    private bool m_bPendingJumpInput = false;

}