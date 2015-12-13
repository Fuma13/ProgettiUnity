using UnityEngine;
using System.Collections;

//Follow the target with an offset and distace setted
public class CameraFollow : MonoBehaviour 
{
	void Awake () 
	{
		m_tCamera.position = (m_tTarget.position + m_v3TargetOffset) + m_v3Distance;
        m_tCamera.rotation = Quaternion.LookRotation((m_tTarget.position + m_v3TargetOffset) - m_tCamera.position);
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
}
