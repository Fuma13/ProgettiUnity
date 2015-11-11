using System.Collections;
using UnityEngine;

//Snalize and compare the simple gesture with a direction and an angle
public class GestureAnalizer : MonoBehaviour {

	//When init, if there is, discard all precedent parameters
	public void Init(float gestureLife, Vector2 searchedDirection, float validDeltaAngle, OnRightGestureDone onRightGestureDone)
	{
		SetSearchedDirection (searchedDirection);
		SetValidDeltaAngle (validDeltaAngle);
		if (m_oGestureIdentifier == null)
			m_oGestureIdentifier = gameObject.AddComponent<GestureIdentifier> ();
		m_oGestureIdentifier.Init (gestureLife, OnGestureDone);
		m_oOnRightGestureDone = onRightGestureDone;
	}

	//If the start and the end input are unnoticeable use this
	//E.g. GetMousePosition
	//Otherwise use StartGestureInput and EndGestureInput
	public void StartOrEndGestureInput(Vector2 inputPosition)
	{
		if (m_oOnRightGestureDone != null) {
			m_oGestureIdentifier.StartOrEndGestureInput(inputPosition);
		}
	}
	
	//If the start and the end input are noticeable(different) use this and then EndGestureInput
	//E.g. GetKeyDown
	//Otherwise use StartOrEndGestureInput
	public void StartGestureInput(Vector2 inputPosition)
	{
		if (m_oOnRightGestureDone != null) {
			m_oGestureIdentifier.StartGestureInput(inputPosition);
		}
	}
	
	//If the start and the end input are noticeable(different) type use this after StartGestureInput
	//E.g. GetKeyUp
	//Otherwise use StartOrEndGestureInput
	public void EndGestureInput(Vector2 inputPosition)
	{
		if (m_oOnRightGestureDone != null) {
			m_oGestureIdentifier.EndGestureInput(inputPosition);
		}
	}
	
	public void SetSearchedDirection(Vector2 searchedDirection)
	{
		m_vSearchedDirection = new Vector3 (searchedDirection.normalized.x, searchedDirection.normalized.y);
	}
	
	public void SetValidDeltaAngle(float validDeltaAngle)
	{
		m_fValidDeltaAngle = validDeltaAngle;
	}
	
	//If someone is interested to gesture with this specific gestureLife, direction and angle can register to it
	public void RegisterOnGestureDoneCallback(OnRightGestureDone onGestureRightDone)
	{
		m_oOnRightGestureDone += onGestureRightDone;
	}
	
	public void DeregisternGestureDoneCallback(OnRightGestureDone onGestureRightDone)
	{
		m_oOnRightGestureDone -= onGestureRightDone;
	}

	public void DiscardCurrentGesture()
	{
		m_oGestureIdentifier.DiscardCurrentGesture ();
	}

	public void Discard()
	{
		m_oGestureIdentifier.Discard();
		m_oOnRightGestureDone = null;
	}

	private void OnGestureDone(Vector2 gesture)
	{
		float angle = Vector2.Angle (m_vSearchedDirection, gesture);
		if (angle <= m_fValidDeltaAngle) {

			Vector3 crossProduct = Vector3.Cross(m_vSearchedDirection, new Vector3(gesture.x, gesture.y));
			bool right = crossProduct.z > 0 ? true : false;
			
			//Callback
			m_oOnRightGestureDone(angle,right);
		}
	}

	//Parametric variables
	private Vector3 m_vSearchedDirection;
	private float m_fValidDeltaAngle;
	//Internal variables
	private GestureIdentifier m_oGestureIdentifier;
	//Events
	public delegate void OnRightGestureDone(float deltaAngle, bool right);
	private event OnRightGestureDone m_oOnRightGestureDone = null;

}
