using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour 
{
	private void OnEnable()
	{
		m_oGameManager.OnPauseEvent += OnPause;
		m_oGameManager.OnMainMenuEvent += OnMainMenu;
		m_oGameManager.OnRestartEvent += OnGame;
		m_oGameManager.OnStartEvent += OnGame;
		m_oGameManager.OnUnpauseEvent += OnGame;
        m_oGameManager.OnDeadEvent += OnDead;
	}

	private void OnDisable()
	{
		m_oGameManager.OnPauseEvent -= OnPause;
		m_oGameManager.OnMainMenuEvent -= OnMainMenu;
		m_oGameManager.OnRestartEvent -= OnGame;
		m_oGameManager.OnStartEvent -= OnGame;
		m_oGameManager.OnUnpauseEvent -= OnGame;
        m_oGameManager.OnDeadEvent -= OnDead;
    }

	#region MAIN_MENU
	public void OnStartClick()
	{
		m_oGameManager.ChangeState (GameManager.GameState.GAME);
	}

	public void OnExitClick()
	{
		Application.Quit ();
	}
	#endregion
	#region PAUSE_MENU DEAD_MENU
	public void OnContinueClick()
	{
		m_oGameManager.ChangeState (GameManager.GameState.GAME);
	}

	public void OnRestartClick()
	{
		m_oGameManager.ChangeState (GameManager.GameState.RESTART);
	}

	public void OnMainMenuClick()
	{
		m_oGameManager.ChangeState (GameManager.GameState.MAIN);
	}
	#endregion
    #region GAME
    public void OnPauseClick()
	{
		m_oGameManager.ChangeState (GameManager.GameState.PAUSE);
	}
	#endregion

	#region EVENTS
	private void OnPause()
	{
		m_oMainMenu.SetActive (false);
		m_oInGameMenu.SetActive (false);
        m_oDeadMenu.SetActive(false);
        m_oPauseMenu.SetActive(true);
	}
	private void OnMainMenu()
	{			
		m_oInGameMenu.SetActive (false);
		m_oPauseMenu.SetActive (false);
        m_oDeadMenu.SetActive(false);
        m_oMainMenu.SetActive(true);
	}

	private void OnGame()
	{
		m_oMainMenu.SetActive (false);
		m_oPauseMenu.SetActive (false);
        m_oDeadMenu.SetActive(false);
        m_oInGameMenu.SetActive(true);
	}

    private void OnDead()
    {
        m_oMainMenu.SetActive(false);
        m_oPauseMenu.SetActive(false);
        m_oInGameMenu.SetActive(false);
        m_oDeadMenu.SetActive(true);
    }
	#endregion


	[SerializeField]private GameManager m_oGameManager; 
	[SerializeField]private GameObject m_oMainMenu;
	[SerializeField]private GameObject m_oPauseMenu;
	[SerializeField]private GameObject m_oInGameMenu;
    [SerializeField]private GameObject m_oDeadMenu;
}
