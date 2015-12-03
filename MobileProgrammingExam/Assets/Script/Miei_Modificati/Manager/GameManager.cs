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
    public Action OnDeadEvent;

	void Start () 
	{
		m_eGameState = GameState.MAIN;
		OnMainmenu ();
	}

	public bool ChangeState(GameState eNextState)
	{
		switch (eNextState) 
		{
		case GameState.GAME:
			if(m_eGameState == GameState.PAUSE)
			{
				m_eGameState = GameState.GAME;
				OnUnpause();
                return true;
			}
			else if(m_eGameState == GameState.MAIN)
			{
				m_eGameState = GameState.GAME;
				OnStart();
                return true;
            }
			break;
		case GameState.MAIN:
            if (m_eGameState == GameState.PAUSE || m_eGameState == GameState.DEAD)
			{
				m_eGameState = GameState.MAIN;
				OnMainmenu();
                return true;
            }
			break;
		case GameState.PAUSE:
			if(m_eGameState == GameState.GAME)
			{
				m_eGameState = GameState.PAUSE;
				OnPause();
                return true;
            }
			break;
		case GameState.RESTART:
			if(m_eGameState == GameState.PAUSE || m_eGameState == GameState.DEAD)
			{
				m_eGameState = GameState.RESTART;
				OnRestart();
				m_eGameState = GameState.GAME;
                return true;
            }
			break;
        case GameState.DEAD:
            if(m_eGameState == GameState.GAME)
            {
                m_eGameState = GameState.DEAD;
                OnDead();
                return true;
            }
            break;
		default:
            break;
		}
        return false;
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
		if (OnRestartEvent != null) 
		{
			OnRestartEvent();
		}
	}

    private void OnDead()
    {
        if(OnDeadEvent != null)
        {
            OnDeadEvent();
        }
    }


	private GameState m_eGameState;

	public enum GameState
	{
		MAIN = 0,
		PAUSE,
		GAME,
		RESTART,
        DEAD
	}
}
