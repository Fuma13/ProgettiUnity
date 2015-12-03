using UnityEngine;
using System.Collections;

public class InputPlayerTouch : InputBase
{
    protected override void InternalInitInput()
    {
    }
	protected override void InternalInputUpdate()
	{
		base.InputUpdate();
		TouchesUpdate ();
	}

	void TouchesUpdate()
	{
		int touchesCount = Input.touchCount;
		Touch[] touches = Input.touches;
		Touch courrentNewTouch;
		TouchInfo currentOldTouch;
		bool find = false;
		for (int i=0; i<m_aTouches.Count; ++i) {
			currentOldTouch = (TouchInfo)m_aTouches [i];
			Debug.Log("CHECK OLD TOUCH: " + currentOldTouch.m_iID);
			if (currentOldTouch.m_iID != -1) {
				for (int touchesIndex = 0; touchesIndex < touchesCount && !find; ++touchesIndex) {
					Debug.Log("CHECK TOUCH: ");
					courrentNewTouch = touches [touchesIndex];

					if (currentOldTouch.m_iID == courrentNewTouch.fingerId) {
						Debug.Log("UPDATE TOUCH " + courrentNewTouch.fingerId);
						//Update an old touch
						currentOldTouch.m_aPositions [currentOldTouch.m_iCurrentTail].m_vPosition = courrentNewTouch.position;
						currentOldTouch.m_aPositions [currentOldTouch.m_iCurrentTail].m_vPosition.x /= Screen.width;
						currentOldTouch.m_aPositions [currentOldTouch.m_iCurrentTail].m_vPosition.y /= Screen.height;
						currentOldTouch.m_aPositions [currentOldTouch.m_iCurrentTail].m_fTime = Time.time;
						find = true;
					}
				}
			
				if (!find) {
					Debug.Log("REMOVE TOUCH " + currentOldTouch.m_iID);
					//This touch is finished
					StaticGestureIdentifier.IdentifyGesture(currentOldTouch.m_aPositions [currentOldTouch.m_iCurrentHead],
				                                                           currentOldTouch.m_aPositions [currentOldTouch.m_iCurrentTail],
					                                        				mk_fMinDistanceForValidate, OnDetailedGestureDone,
				                                                           mk_vSearchedDirection, mk_iAngleTreshold,
				                                                           mk_fMinSpeedForValidate);
//					StaticGestureIdentifier.IdentifyGesture(currentOldTouch.m_aPositions [currentOldTouch.m_iCurrentHead].m_vPosition,
//					                                        currentOldTouch.m_aPositions [currentOldTouch.m_iCurrentTail].m_vPosition,
//					                                        mk_fMinDistanceForValidate, OnGestureDone);
					currentOldTouch.m_iID = -1;
					m_aTouches.RemoveAt(i);
				}
			}
		}

		if(m_aTouches.Count <= mk_MAX_TOUCHES){
		//There is an empty space and can add a new touch
			for (; touchesCount > 0; --touchesCount) {

				find = false;
				for (int i=0; i<m_aTouches.Count; ++i) {
					if (((TouchInfo)m_aTouches [i]).m_iID == touches [touchesCount - 1].fingerId)
						find = true;
				}
				if (!find) {
					Debug.Log("NEW TOUCH: " + touches [touchesCount - 1].fingerId);
					TouchInfo newTouch = new TouchInfo();
					newTouch.m_iID = touches [touchesCount - 1].fingerId;
					newTouch.m_aPositions[newTouch.m_iCurrentHead].m_vPosition = touches [touchesCount - 1].position;
					newTouch.m_aPositions[newTouch.m_iCurrentHead].m_vPosition.x /= Screen.width;
					newTouch.m_aPositions[newTouch.m_iCurrentHead].m_vPosition.y /= Screen.height;
					newTouch.m_aPositions[newTouch.m_iCurrentHead].m_fTime = Time.time;
					m_aTouches.Add(newTouch);
				}
			}
		}
	}

	void OnDetailedGestureDone(float deltaAngle, bool right, float distance, float velocity)
	{
        int iAngleDirection = right ? 1 : -1;
        Vector2 v2Direction = new Vector2();
        v2Direction.x = Mathf.Cos(deltaAngle);
        v2Direction.y = iAngleDirection * Mathf.Sin(deltaAngle);
        v2Direction *= distance;
        //InternalGestureDetected(v2Direction);
	}

	void OnGestureDone(Vector2 direction)
	{
        //InternalShootDetected (direction);
	}

	const int mk_MAX_TOUCHES = 10;
	ArrayList m_aTouches = new ArrayList (mk_MAX_TOUCHES);
	private readonly Vector2 	mk_vSearchedDirection = Vector2.up;
	private const int 			mk_iAngleTreshold = 30;
	private const float 		mk_fMinSpeedForValidate = 0.5f;
	private const float 		mk_fMinDistanceForValidate = 0.15f;

}
