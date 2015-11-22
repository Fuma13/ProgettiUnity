using UnityEngine;
using System.Collections;
using System;

public class InputBase
{
	public virtual void InitInput()
	{
	}

	public virtual void InputUpdate()
	{
	}

	protected void InternalJumpDetected()
	{
		if(m_actJumpCallback != null)
		{
			m_actJumpCallback();
		}
	}

	protected void InternalShootDetected(Vector3 v3Position)
	{
		if(m_actShootCallback != null)
		{
            m_actShootCallback(v3Position);
		}
	}

	protected void InternalGestureDetected(Vector2 v2Direction)
	{
		if (m_actGestureCallback != null) {
            m_actGestureCallback(v2Direction);
		}
	}

    public void Activate(Action actJumpInput, DetailedVec3Callback actShootInput, DetailedVec2Callback actGestureInput)
	{
		m_actJumpCallback = actJumpInput;
		m_actShootCallback = actShootInput;
		m_actGestureCallback = actGestureInput;
	}
	
	public void Deactivate()
	{
		m_actJumpCallback = null;
		m_actShootCallback = null;
		m_actGestureCallback = null;
	}

	//VARS
	public delegate void DetailedVec3Callback(Vector3 v3Position);
    public delegate void DetailedVec2Callback(Vector2 v2Direction);
	protected event Action m_actJumpCallback = null;
	protected event DetailedVec3Callback m_actShootCallback = null;
    protected event DetailedVec2Callback m_actGestureCallback = null;
}
