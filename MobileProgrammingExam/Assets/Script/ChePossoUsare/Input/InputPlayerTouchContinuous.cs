using UnityEngine;
using System.Collections;

public class InputPlayerTouchContinuous: InputBase
{
	protected override void InternalInitInput()
	{
		m_aoTouchInfos = new TouchInfo[mk_iMaxTouchNumber];

		InitTouch();
	}

	private void InitTouch()
	{
		for(int i = 0; i < mk_iMaxTouchNumber; ++i)
		{
			m_aoTouchInfos[i].m_oContInput = new ContinuousInput(mk_iTouchWindowSize);
			m_aoTouchInfos[i].m_bStarted = false;
		}

        //m_oGestureFSM = new GestureFSM (4);
        //m_oGestureFSM.AddGesture (new Vector3 (0.0f, 1.0f, 0.0f), 0.15f, 0.5f, 30);
        //m_oGestureFSM.AddGesture (new Vector3 (1.0f, 0.0f, 0.0f), 0.15f, 0.5f, 30);
        //m_oGestureFSM.AddGesture (new Vector3 (0.0f, -1.0f, 0.0f), 0.15f, 0.5f, 30);
        //m_oGestureFSM.AddGesture (new Vector3 (-1.0f, 0.0f, 0.0f), 0.15f, 0.5f, 30);
	}

	protected override void InternalInputUpdate()
	{
		int iTouches = 0;

		for(int i = 0; i < Input.touchCount; ++i)
		{
			if(Input.touches[i].fingerId >= mk_iMaxTouchNumber)
			{
				Debug.LogError("Finger ID excedes max touch numbers");
				return;
			}
			else
			{
				m_aoTouchInfos[Input.touches[i].fingerId].m_bStarted = true;

				//Update the touch position
				iTouches |= 1 << i;
				int iID = Input.touches[i].fingerId;
				
				Vector3 vPos = Input.touches[i].position;
				vPos.x /= Screen.width;
				vPos.y /= Screen.height;
				
				m_aoTouchInfos[iID].m_oContInput.AddPosition(vPos, Time.deltaTime);
			}
		}

		for(int i = 0; i < mk_iMaxTouchNumber; ++i)
		{
			if(m_aoTouchInfos[i].m_bStarted)
			{
				//Check Gestures..
				CheckGesture(i);

				//Touch finished..
				if((iTouches & 1 << i) == 0)
				{
					TouchFinished(i);
				}
			}
		}
	}


	private void TouchFinished(int iID)
	{
		m_aoTouchInfos[iID].m_oContInput.Clear();

		m_aoTouchInfos[iID].m_bStarted = false;

        //m_oGestureFSM.Reset ();
	}

	private void CheckGesture(int iID)
	{
		float fTime = 0.0f;
		float fDistance = 0.0f;
		Vector3 vDirection = Vector3.zero;
        GestureFSM.Gesture currentGesture = new GestureFSM.Gesture();//m_oGestureFSM.GetCurrentGesture ();

        //m_aoTouchInfos[iID].m_oContInput.GetGestureStatus(out fDistance, out fTime, out vDirection);

		float fSpeed = fDistance / fTime;
		if(fDistance >= currentGesture.m_fMinDistanceForValidate && fSpeed > currentGesture.m_fMinSpeedForValidate)
		{
//			float fAngle = VectorUtils.Angle(currentGesture.m_vReferenceDirection, mk_vUpVector, vDirection);
			bool bValidAngle = VectorUtils.IsAngleWithinThreshold(vDirection, mk_vUpVector, currentGesture.m_vReferenceDirection, currentGesture.m_iAngleTreshold);
			
//			Debug.Log("fDistance = " + fDistance + "fSpeed = " + fSpeed + " bValidAngle = " + bValidAngle + " fAngle = " + fAngle + " iID = " + iID);
			
			if(bValidAngle)
			{
				//Clear the gesture info..
				m_aoTouchInfos[iID].m_oContInput.Clear();
                //Debug.Log("Gesture: " + m_oGestureFSM.GetCurrentGestureID() + " done");
				//TODO: Go to the next gesture to check
                //if(!m_oGestureFSM.NextGesture())
                //{
                //    InternalJumpDetected();
                //    m_oGestureFSM.Reset();
                //}
			}
		}
	}

	//VARS
	private struct TouchInfo
	{
		public ContinuousInput 	m_oContInput;
		public bool 			m_bStarted;
	}

	private TouchInfo[] 		m_aoTouchInfos;

	private const int 			mk_iMaxTouchNumber = 10;
	private const int 			mk_iTouchWindowSize = 15;

	private GestureFSM			m_oGestureFSM;

//	private const int 			mk_iAngleTreshold = 30;
//	private const float 		mk_fMinSpeedForValidate = 0.5f;
//	private const float 		mk_fMinDistanceForValidate = 0.15f;
//	private readonly Vector3 	mk_vReferenceDirection = new Vector3(0.0f, 1.0f, 0.0f);
    private readonly Vector3 mk_vUpVector = new Vector3(0.0f, 0.0f, 1.0f);
}
