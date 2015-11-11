using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour 
{
	public Action OnPauseEvent;
	public Action OnUnpauseEvent;
	public Action OnMainMenuEvent;
	public Action OnStartEvent;
	public Action OnRestartEvent;

	void Start () 
	{
		m_eGameState = GameState.MAIN;
		OnMainmenu ();
	}

	public void ChangeState(GameState eNextState)
	{
		switch (eNextState) 
		{
		case GameState.GAME:
			if(m_eGameState == GameState.PAUSE)
			{
				m_eGameState = GameState.GAME;
				OnUnpause();
			}
			else if(m_eGameState == GameState.MAIN)
			{
				m_eGameState = GameState.GAME;
				OnStart();
			}
			break;
		case GameState.MAIN:
			if(m_eGameState == GameState.PAUSE)
			{
				m_eGameState = GameState.MAIN;
				OnMainmenu();
			}
			break;
		case GameState.PAUSE:
			if(m_eGameState == GameState.GAME)
			{
				m_eGameState = GameState.PAUSE;
				OnPause();
			}
			break;
		case GameState.RESTART:
			if(m_eGameState == GameState.PAUSE)
			{
				m_eGameState = GameState.RESTART;
				OnRestart();
				m_eGameState = GameState.GAME;
			}
			break;
		default:
			break;
		}
	}

	private void OnPause()
	{
		if (OnPauseEvent != null) 
		{
			OnPauseEvent();
		}
	}

	private void OnUnpause()
	{
		if (OnUnpauseEvent != null) 
		{
			OnUnpauseEvent();
		}
	}

	private void OnStart()
	{
		if (OnStartEvent != null) 
		{
			OnStartEvent();
		}
	}

	private void OnMainmenu()
	{
		if (OnMainMenuEvent != null) 
		{
			OnMainMenuEvent();
		}
	}

	private void OnRestart()
	{
		Debug.Log ("RESTART");
		
		if (OnRestartEvent != null) 
		{
			OnRestartEvent();
		}
	}


	private GameState m_eGameState;

	public enum GameState
	{
		MAIN = 0,
		PAUSE = 1,
		GAME = 2,
		RESTART = 3
	}
}
