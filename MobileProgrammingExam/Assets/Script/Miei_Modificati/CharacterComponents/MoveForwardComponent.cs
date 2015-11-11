using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class MoveForwardComponent : MonoBehaviour 
{

	void Start () 
	{
		Assert.IsNotNull (m_tTarget, "Missing target transform in MoveForwardComponent");
	}

	void FixedUpdate () 
	{
		m_tTarget.Translate (m_tTarget.forward * m_fSpeed * Time.fixedDeltaTime, Space.World);
	}

	[Header("Setup")]
	[SerializeField] private Transform m_tTarget;
	[Header("Tuning")]
	[SerializeField] private float m_fSpeed;
}
