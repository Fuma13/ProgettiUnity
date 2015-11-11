using UnityEngine;
using System.Collections;

public class GravityModifier : MonoBehaviour {

	void Start()
	{
		m_v3BaseGravity = Physics.gravity;
		UpdateGravityCameraRotation ();
		Debug.Log ("Gravity: " + Physics.gravity);
	}

	public void UpdateGravityCameraRotation()
	{
		UpdateGravity (Camera.main.transform.eulerAngles.z);
	}

	public void UpdateGravity (float fAngle) 
	{
		float cos = Mathf.Cos (fAngle * Mathf.Deg2Rad);
		float sin = Mathf.Sin (fAngle * Mathf.Deg2Rad);
		//m_v3NewGravity.x = m_v3NewGravity.x;
		m_v3NewGravity.y = m_v3BaseGravity.x * sin + m_v3BaseGravity.y * cos;
		m_v3NewGravity.z = m_v3BaseGravity.z * cos - m_v3BaseGravity.y * sin;
		Physics.gravity = m_v3NewGravity;
	}

	private Vector3 m_v3BaseGravity;
	private Vector3 m_v3NewGravity;
}
