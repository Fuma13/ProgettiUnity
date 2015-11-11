using UnityEngine;
using System.Collections;

//Generate a gesture from two inputs
public class StaticGestureIdentifier {

	static public void IdentifyGesture(Vector2 startPos, Vector2 endPos, float minDistance, OnGestureDone onGestureDoneCallback)
	{
		if (onGestureDoneCallback != null)
			if((endPos - startPos).sqrMagnitude > minDistance*minDistance)
				onGestureDoneCallback (endPos - startPos);
	}

	static public void IdentifyGesture(Vector2 startPos, Vector2 endPos,float minDistance, OnRightGestureDone onGestureDoneCallback,
	                                   Vector2 searchedDirection, float validDeltaAnagle)
	{
		if (onGestureDoneCallback != null) {
			Vector2 gesture = endPos - startPos;
			if (gesture.sqrMagnitude > minDistance * minDistance) {
				float angle = Vector2.Angle (searchedDirection, gesture);
				if (angle <= validDeltaAnagle) {
			
					Vector3 crossProduct = Vector3.Cross (searchedDirection, new Vector3 (gesture.x, gesture.y));
					bool right = crossProduct.z > 0 ? true : false;
			
					//Callback
					onGestureDoneCallback (angle, right);
				}
			}
		}
	}

	static public void IdentifyGesture(Position startPos, Position endPos, float minDistance, OnDetailedGestureDone onGestureDoneCallback,
	                                   Vector2 searchedDirection, float validDeltaAnagle, float minVelocity)
	{
		if (onGestureDoneCallback != null) {
			Vector2 gesture = startPos.m_vPosition - endPos.m_vPosition;
			if (gesture.sqrMagnitude >= minDistance * minDistance) {
				float distance = gesture.magnitude;
				float velocity = distance / (endPos.m_fTime - startPos.m_fTime);
				if(velocity >= minVelocity){
					float angle = Vector2.Angle (searchedDirection, gesture);
					if (angle <= validDeltaAnagle) {
						Vector3 crossProduct = Vector3.Cross (searchedDirection, new Vector3 (gesture.x, gesture.y));
						bool right = crossProduct.z > 0 ? true : false;
					
						//Callback
						onGestureDoneCallback (angle, right, distance, velocity);
					}
				}
			}
		}
	}


	//Events
	public delegate void OnGestureDone(Vector2 gesture);
	public delegate void OnRightGestureDone(float deltaAngle, bool right);
	public delegate void OnDetailedGestureDone(float deltaAngle, bool right, float distance, float velocity);
}
