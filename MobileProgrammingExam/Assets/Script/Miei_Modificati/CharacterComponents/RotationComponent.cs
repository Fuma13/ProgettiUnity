using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class RotationComponent : MonoBehaviour 
{
	void Start () 
	{
		Assert.IsNotNull (m_tTarget, "Missing target transform in RotationComponent");
		Assert.IsNotNull (m_tBaseRotation, "Missing base rotation transform in RotationComponent");
		Assert.IsNotNull (m_oGravityComponent, "Missing GravitiComponent in RotationComponent");
		
		m_fMaxSafeCosangle = Mathf.Cos (m_fMaxSafeAngle * Mathf.Deg2Rad);
	}

	void FixedUpdate () 
	{
		GetInput ();

		m_tTarget.Rotate(new Vector3(m_fVerticalRotation,m_fHorizontalRotation,0.0f), Space.World);
		
		m_fCosangleZ = Vector3.Dot(m_tBaseRotation.forward, m_tTarget.forward);
		
		if (m_oGravityComponent.IsGrounded) 
		{
			//Check the current rotation, if angle > m_fMaxSafeAngle the character fall
			if(Mathf.Abs(m_fCosangleZ) > m_fMaxSafeCosangle)
			{
				//if there are inputs, it doesn't correct the rotation
				if(m_fHorizontalRotation != 0 || m_fVerticalRotation != 0)
				{
					m_bMustInitSelfCorrection = true;
				}
				//When I leave the input, it initialize the rotation correction
				else if(m_bMustInitSelfCorrection)
				{
					InitSelfCorrection();
				}
				//When there aren't input and the rotattion correction is init
				else if(m_fCosangleZ < 1.0f)
				{
					ComputeSelfCorrection();
				}
			}
			else
			{
				//Fall
				if(m_bDebug)
					Debug.Log("Fall - current: " + m_fCosangleZ + " Max: " + m_fMaxSafeCosangle);
			}
		}
	}

	private void GetInput()
	{
		m_fHorizontalRotation = Input.GetAxis("Vertical") * m_fHorizontalSpeedRotation * Time.fixedDeltaTime;
		m_fVerticalRotation = Input.GetAxis("Horizontal") * m_fVerticalSpeedRotation * Time.fixedDeltaTime;
	}

	private void InitSelfCorrection()
	{
		m_qInitialRotation = m_tTarget.rotation;
		m_fAngleToDo = Quaternion.Angle (m_qInitialRotation, m_tBaseRotation.rotation);
		m_fRotationStartTime = Time.time;
		//Invert the base direction rotation, so it can go it fakie
		m_tBaseRotation.forward = Mathf.Sign (m_fCosangleZ) * m_tBaseRotation.forward;
		m_bMustInitSelfCorrection = false;
	}

	private void ComputeSelfCorrection()
	{
		m_fCurrentFracRotation = (Time.time - m_fRotationStartTime) * m_fHorizontalSpeedRotation;
		m_fCurrentFracRotation /= m_fAngleToDo;
		m_tTarget.rotation = Quaternion.Lerp (m_qInitialRotation, m_tBaseRotation.rotation, m_fCurrentFracRotation);
	}

	[Header("Setup")]
	[SerializeField] private Transform m_tTarget;
	[SerializeField] private Transform m_tBaseRotation;
	[SerializeField] private GravityComponent m_oGravityComponent;
	[Header("Tuning")]
	[SerializeField] private float m_fHorizontalSpeedRotation;
	[SerializeField] private float m_fVerticalSpeedRotation;
	[Tooltip("Max rotation angle after that fall (in degree)")]
	[SerializeField] private float m_fMaxSafeAngle;
	[Header("Debug")]
	[SerializeField] private bool m_bDebug = false;
	private float m_fHorizontalRotation;
	private float m_fVerticalRotation;
	private float m_fMaxSafeCosangle;
	private float m_fCosangleZ;
	private bool m_bMustInitSelfCorrection = true;
	private Quaternion m_qInitialRotation;
	private float m_fRotationStartTime;
	private float m_fCurrentFracRotation;
	private float m_fAngleToDo;
	private float m_fRotationDirection;
}
