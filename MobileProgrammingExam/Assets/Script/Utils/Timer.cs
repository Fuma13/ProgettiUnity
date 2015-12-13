using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Timer : MonoBehaviour
{
	// Update is called once per frame
	private void Update()
	{
		if(m_bRunning && !m_bPaused)
		{
			ComputeTimer();
		}
	}
	
	private void ComputeTimer()
	{
		m_fElapsedTime += Time.deltaTime;
		m_fFrameTime += Time.deltaTime;

		if((m_actDeltaTimeUpdate != null) && (m_fFrameTime >= m_fDeltaTime))
		{
			m_actDeltaTimeUpdate();
			m_fFrameTime -= m_fDeltaTime;
		}
		
		if(m_fElapsedTime >= m_fDuration)
		{
			FinishTimer();
		}
	}
	
	public void FinishTimer()
	{
		m_bRunning = false;
			
		if(m_actTimeElapsed != null)
		{
			m_actTimeElapsed();
		}	
	}
	
	public void StartTimer(float fTime, Action actCallBack)
	{
		StartTimer(fTime,1.0f, actCallBack, false, null);
	}
	
	public void StartTimer(float fTime, Action actCallBack, Action actDeltaTimeUpdateCallBack)
	{
		StartTimer(fTime,1.0f, actCallBack, false, actDeltaTimeUpdateCallBack);
	}

	public void StartTimer(float fTime,float fDeltaTime, Action actCallBack, Action actDeltaTimeUpdateCallBack)
	{
		StartTimer(fTime,fDeltaTime, actCallBack, false, actDeltaTimeUpdateCallBack);
	}
	
	public void StartTimer(float fTime, Action actCallBack, bool bStop)
	{
		StartTimer(fTime,1.0f, actCallBack, bStop, null);
	}
	
	public void StartTimer(float fTime, float fDeltaTime, Action actCallBack, bool bStop, Action actDeltaTimeUpdateCallBack)
	{
		if(!m_bRunning || bStop)
		{
            if(fTime == 0)
            {
                actCallBack();
                return;
            }

			m_fElapsedTime = 0.0f;
			m_fFrameTime = 0.0f;
			m_fDuration = fTime;
			m_fDeltaTime = fDeltaTime;
			
			m_actTimeElapsed = actCallBack;
			m_actDeltaTimeUpdate = actDeltaTimeUpdateCallBack;
			
			m_bRunning = true;
			m_bPaused = false;
		}
	}
	
	public float GetActualTime()
	{
		return 	m_fElapsedTime;
	}
	
	public float GetRemainingTime()
	{
		return 	m_fDuration - m_fElapsedTime;
	}
	
	public void Discard()
	{
		m_bRunning = false;
		m_bPaused = false;
		m_actTimeElapsed = null;
		m_actDeltaTimeUpdate = null;
	}
	
	public void Pause()
	{
		m_bPaused = true;
	}
	
	public void UnPause()
	{
		m_bPaused = false;
	}
	
	private bool 	m_bRunning = false;
	public bool IsRunning
	{
		get { return m_bRunning; }
	}
	
	private bool 	m_bPaused = false;
	public bool IsPaused
	{
		get { return m_bPaused; }
	}
	
	private float 	m_fElapsedTime = 0.0f;
	private float	m_fFrameTime = 0.0f;
	private float 	m_fDeltaTime = 1.0f;
	private float 	m_fDuration = 0.0f;
	
	private event Action m_actTimeElapsed = null;
	private event Action m_actDeltaTimeUpdate = null;
}
