using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class GravityComponent : MonoBehaviour {

	void Awake()
	{
		Assert.IsNotNull (m_oRigidBodyTarget, "Missing rigidbody target in GravityComponent");

		m_oRigidBodyTarget.isKinematic = true;
	}

	void Start () 
	{
	}

	void OnEnable()
	{
		m_oGameManager.OnStartEvent += OnStart;
		for(int i=0; i < m_aoCollisionTrigger.Length; ++i)
		{
			m_aoCollisionTrigger[i].SimpleCollisionEnter += CharacterCollisionEnter;
			m_aoCollisionTrigger[i].SimpleCollisionExit += CharacterCollisionExit;
		}
	}

	void OnDisable()
	{
		m_oGameManager.OnStartEvent -= OnStart;
		for(int i=0; i < m_aoCollisionTrigger.Length; ++i)
		{
			m_aoCollisionTrigger[i].SimpleCollisionEnter -= CharacterCollisionEnter;
			m_aoCollisionTrigger[i].SimpleCollisionExit -= CharacterCollisionExit;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		GetInput ();
	}

	private void GetInput()
	{
		if (m_fJumpIntencity < 1.0f && Input.GetButton ("Jump")) 
		{
			m_fJumpIntencity += Time.fixedDeltaTime;
		}
		if(Input.GetButtonUp("Jump"))
		{
			m_oRigidBodyTarget.AddForce(m_tBaseJumpDirection.up * m_fJumpIntencity 
			                            * m_oJumpIntencityCorrectorCurve.Evaluate(m_fJumpIntencity) 
			                            * m_fJumpIntencityCorrector, ForceMode.Impulse);
			m_fJumpIntencity = 0.0f;
		}
	}

	public bool IsGrounded
	{
		get { return true;}
	}

	private void CharacterCollisionEnter()
	{
		m_bGrounded = true;
	}

	private void CharacterCollisionExit()
	{
		m_bGrounded = false;
	}

	private void OnStart ()
	{
		m_oRigidBodyTarget.isKinematic = false;
	}

	[Header("Setup")]
	[SerializeField] private GameManager m_oGameManager;
	[SerializeField] private Transform m_tBaseJumpDirection;
	[SerializeField] private Rigidbody m_oRigidBodyTarget;
	[SerializeField] private CollisionEvent[] m_aoCollisionTrigger;
	[Header("Tuning")]
	[SerializeField] private float m_fJumpIntencityCorrector;
	[SerializeField] private AnimationCurve m_oJumpIntencityCorrectorCurve;

	private float m_fJumpIntencity;
	private bool m_bGrounded = false;
}
