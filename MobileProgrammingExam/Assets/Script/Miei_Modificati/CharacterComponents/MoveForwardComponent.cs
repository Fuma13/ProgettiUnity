using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class MoveForwardComponent : BaseComponent 
{

	void Start () 
	{
        Assert.IsNotNull(m_oGameManager, "Missing GameManager in MoveForwardComponent");
        Assert.IsNotNull(m_tTarget, "Missing target transform in MoveForwardComponent");
        m_fSpeed = m_fWalkSpeed;
	}

    void OnEnable()
    {
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
            m_tTarget.Translate(m_tTarget.forward * m_fSpeed * Time.fixedDeltaTime, Space.World);
        }
	}

    public void Stop()
    {
        m_bActive = false;
    }

    public void Run()
    {
        m_fSpeed = m_fRunSpeed;
    }

    public void Walk()
    {
        m_fSpeed = m_fWalkSpeed;
    }

    private void OnStart()
    {
        m_bActive = true;
    }

    private void OnRestart()
    {
        m_tTarget.position = m_tBegin.position;
        OnStart();
    }

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
