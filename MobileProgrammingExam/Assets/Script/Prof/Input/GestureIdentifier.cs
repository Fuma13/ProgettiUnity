using UnityEngine;
using System.Collections;

//Generate a gesture from two inputs into a delta time
//If the time between two inputs is expired, the first input is discarted
//and the second one is considerated as first input for a new gesture
public class GestureIdentifier : MonoBehaviour {

	//When init, if there is, discard all precedent parameters
	public void Init(float gestureLife, OnGestureDone onGestureDone)
	{
		SetGestureLife (gestureLife);
		m_oOnGestureDone = onGestureDone;
	}

	//If the start and the end input are unnoticeable use this
	//Otherwise use StartGestureInput and EndGestureInput
	public void StartOrEndGestureInput(Vector2 inputPosition)
	{
		if (m_oOnGestureDone != null) {
			//First input
			if (!m_bWaitingSecondInput) {
				StartGestureInput(inputPosition);
			}
			//Second input and return gesture
			else {
				EndGestureInput(inputPosition);
			}
		}
	}

	//If the start and the end input are noticeable(different) use this and then EndGestureInput
	//Otherwise use StartOrEndGestureInput
	public void StartGestureInput(Vector2 inputPosition)
	{
		m_vGestureDirection = inputPosition;
		m_oGestureLifeTimer.StartTimer (m_fGestureLife, InputExpired);
		m_bWaitingSecondInput = true;
	}

	//If the start and the end input are noticeable(different) type use this after StartGestureInput
	//Otherwise use StartOrEndGestureInput
	public void EndGestureInput(Vector2 inputPosition)
	{
		m_oGestureLifeTimer.Discard ();
		if (m_bWaitingSecondInput) {
			m_oOnGestureDone (inputPosition - m_vGestureDirection);
		}
		m_bWaitingSecondInput = false;
	}
	
	public void SetGestureLife(float gestureLife)
	{
		m_fGestureLife = gestureLife;
		
		if (m_oGestureLifeTimer == null)
			m_oGestureLifeTimer = gameObject.AddComponent<Timer> ();
		else
			m_oGestureLifeTimer.Discard ();
	}
	
	//If someone is interested to gesture with this specific gestureLife can register to it
	public void RegisterOnGestureDoneCallback(OnGestureDone onGestureDone)
	{
		m_oOnGestureDone += onGestureDone;
	}
	public void DeregisterGestureDoneCallback(OnGestureDone onGestureDone)
	{
		m_oOnGestureDone -= onGestureDone;
	}

	public void DiscardCurrentGesture()
	{
		m_oGestureLifeTimer.Discard ();
		m_bWaitingSecondInput = false;
	}

	public void Discard()
	{
		DiscardCurrentGesture ();
		m_oOnGestureDone = null;
	}

	private void InputExpired()
	{
		m_bWaitingSecondInput = false;
	}

	//Parametric variables
	private float 	m_fGestureLife;
	//Internal variables
	private Timer 	m_oGestureLifeTimer;
	private Vector2 m_vGestureDirection;
	private bool 	m_bWaitingSecondInput = false;
	//Events
	public delegate void OnGestureDone(Vector2 gesture);
	private event OnGestureDone m_oOnGestureDone = null;
}
