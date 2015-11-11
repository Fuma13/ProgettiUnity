using UnityEngine;
using System.Collections;

public class GestureFSM 
{
	public GestureFSM(int maxGestures)
	{
		m_aoGestures = new Gesture[maxGestures];
		m_iMaxGestures = maxGestures;
		m_iLastGesture = 0;
		m_iCurrentGesture = 0;
	}
	
	public void SetGesture(int index, Vector3 direction, float minDistance, float minSpeed, int angleTreshold)
	{
		if (index < m_iMaxGestures) {
			m_aoGestures [index].m_vReferenceDirection = direction;
			m_aoGestures [index].m_fMinDistanceForValidate = minDistance;
			m_aoGestures [index].m_fMinSpeedForValidate = minSpeed;
			m_aoGestures [index].m_iAngleTreshold = angleTreshold;
		}
	}

	public void AddGesture(Vector3 direction, float minDistance, float minSpeed, int angleTreshold)
	{
		if(m_iLastGesture < m_iMaxGestures)
		{
			SetGesture(m_iLastGesture, direction, minDistance, minSpeed, angleTreshold);
			++m_iLastGesture;
		}
	}

	public Gesture GetCurrentGesture()
	{
		return m_aoGestures [m_iCurrentGesture];
	}

	public int GetCurrentGestureID()
	{
		return m_iCurrentGesture;
	}

	public bool NextGesture()
	{
		if (m_iCurrentGesture < m_iLastGesture-1) 
		{
			++m_iCurrentGesture;
			return true;
		}
		return false;
	}

	public void Clear()
	{
		m_iCurrentGesture = 0;
	}


	public struct Gesture
	{
		public int 		m_iAngleTreshold;
		public float 	m_fMinSpeedForValidate;
		public float 	m_fMinDistanceForValidate;
		public Vector3 	m_vReferenceDirection;
	}

	private Gesture[] m_aoGestures;
	private int m_iLastGesture;
	private int m_iCurrentGesture;
	private int m_iMaxGestures;
}
