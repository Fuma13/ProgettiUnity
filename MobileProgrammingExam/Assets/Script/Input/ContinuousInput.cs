using UnityEngine;
using System.Collections;

public class ContinuousInput
{
	public ContinuousInput(int iArraySize)
	{
		m_afDeltaTime = new float[iArraySize];
		m_avPositions = new Vector2[iArraySize];

		Clear();
	}

	public void Clear()
	{
		for(int i = 0; i < m_afDeltaTime.Length; ++i)
		{
			m_afDeltaTime[i] = 0.0f;
			m_avPositions[i] = Vector2.zero;
		}

		m_iNextElement = 0;

		m_fElapsedTime = 0.0f;
		m_vStartPosition = Vector2.zero;
		m_vEndPosition = Vector2.zero;
	}

	public void AddPosition(Vector2 vPosition, float fDT)
	{
		m_fElapsedTime -= m_afDeltaTime[m_iNextElement];

		if(m_iNextElement == 0 && m_vStartPosition == Vector2.zero)
		{
			m_vStartPosition = vPosition;
		}

		m_afDeltaTime[m_iNextElement] = fDT;
		m_fElapsedTime += m_afDeltaTime[m_iNextElement];

		m_avPositions[m_iNextElement] = vPosition;
		m_vEndPosition = vPosition;

		m_iNextElement = (m_iNextElement + 1) % m_afDeltaTime.Length;

		if(m_avPositions[m_iNextElement] != Vector2.zero)
		{
			m_vStartPosition = m_avPositions[m_iNextElement];
		}
	}

	public void GetGestureStatus(out float fDistance, out float fTime, out Vector2 vDirection)
	{
		fTime = m_fElapsedTime;

		vDirection = m_vEndPosition - m_vStartPosition;

		fDistance = vDirection.magnitude;

		vDirection.Normalize();
	}

	//VARS
	private float[] 	m_afDeltaTime;
	private float 		m_fElapsedTime;
	private Vector2[] 	m_avPositions;
	private Vector2		m_vStartPosition;
	private Vector2		m_vEndPosition;
	private int 		m_iNextElement;
}
