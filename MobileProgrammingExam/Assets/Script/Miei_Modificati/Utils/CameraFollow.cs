using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour 
{
	void Awake () 
	{
		m_tCamera.position = (m_tTarget.position + m_v3TargetOffset) + m_v3Distance;
        m_tCamera.rotation = Quaternion.LookRotation((m_tTarget.position + m_v3TargetOffset) - m_tCamera.position);
		m_tCamera.Rotate(m_fZAngle * Vector3.forward);
	}

	void Update () 
	{
        m_tCamera.position = (m_tTarget.position + m_v3TargetOffset) + m_v3Distance;
	}

	[Header("Setup")]
	[SerializeField] private Transform m_tCamera;
	[SerializeField] private Transform m_tTarget;
	[Header("Tuning")]
    [SerializeField] private Vector3 m_v3TargetOffset;
	[SerializeField] private Vector3 m_v3Distance;
	[SerializeField] private float m_fZAngle;
}
