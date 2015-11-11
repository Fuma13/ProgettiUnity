using UnityEngine;
using System.Collections;

public class TouchInfo{

	public int m_iID = -1;

	public Position[] m_aPositions = new Position[2];
//	int m_iMaxPositions;
//	float m_fMaxTime;
	public int m_iCurrentHead = 0;
	public int m_iCurrentTail = 1;
//	float m_fCurrentTime;
//	Phase m_pTouchPhase;
}

public struct Position
{
	public Vector2 m_vPosition;
	public float m_fTime;
}


public enum Phase
{
	START,
	MOVING,
	END
}
