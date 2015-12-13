using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

//Move Component: manage the forward axe, move the target in line with different speed
public class MoveForwardComponent : BaseComponent 
{
	void Awake () 
	{
        //Setup initial speed
        m_fSpeed = m_fWalkSpeed;
	}

    void OnEnable()
    {
        //Register to game events
        m_oGameManager.OnStartEvent += OnStart;
        m_oGameManager.OnUnpauseEvent += OnStart;
        m_oGameManager.OnRestartEvent += OnRestart;
        m_oGameManager.OnPauseEvent += Stop;
        m_oGameManager.OnMainMenuEvent += OnMainMenu;
        m_oGameManager.OnDeadEvent += Stop;
    }

    void OnDisable()
    {
        m_oGameManager.OnStartEvent -= OnStart;
        m_oGameManager.OnUnpauseEvent -= OnStart;
        m_oGameManager.OnRestartEvent -= OnRestart;
        m_oGameManager.OnPauseEvent -= Stop;
        m_oGameManager.OnMainMenuEvent -= OnMainMenu;
        m_oGameManager.OnDeadEvent -= Stop;
    }

	void FixedUpdate () 
	{
        if (m_bActive)
        {
            //Move the target
            m_tTarget.Translate(m_tTarget.forward * m_fSpeed * Time.fixedDeltaTime, Space.World);
        }
	}

    public void Stop()
    {
        //Stop move
        m_bActive = false;
    }

    public void Run()
    {
        //Change speed of move to Sprint run
        m_fSpeed = m_fRunSpeed;
    }

    public void Walk()
    {
        //Change speed of move to walk
        m_fSpeed = m_fWalkSpeed;
    }

    private void OnStart()
    {
        //Start move
        m_bActive = true;
    }

    //Reset initial posizion and start move
    private void OnRestart()
    {
        m_tTarget.position = m_tBegin.position;
        OnStart();
    }

    //Stop move and reset initial position
    private void OnMainMenu()
    {
        Stop();
        m_tTarget.position = m_tBegin.position;
    }

	[Header("Setup")]
    [SerializeField] private Transform m_tBegin;
	[Header("Tuning")]
	[SerializeField] private float m_fWalkSpeed;
	[SerializeField] private float m_fRunSpeed;

    private bool m_bActive = false;
    private float m_fSpeed;
}
