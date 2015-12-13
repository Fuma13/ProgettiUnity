using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System;

//Comunicate, initialize and deactivate the InputBase
//Interface between input and game
public class InputManager : MonoBehaviour 
{
    public Action OnJump = null;
    public Action OnAttack = null;
    public Action<Vector2> OnSprint = null;

    void Awake()
    {
        Assert.IsNotNull<InputFactory>(m_oInputFactory);
    }

	void Start()
	{
		InitInput();
	}
	
	void Update()
	{
        m_oInput.InputUpdate();
	}

	private void JumpDetecet()
	{
		if(OnJump != null)
		{
			OnJump();
		}
	}

	private void AttackDetected()
	{
        if (OnAttack != null)
		{
            OnAttack();
		}
	}

    private void JumpAndAttack(Vector2 v2Direction)
    {
        if(OnSprint != null)
        {
            OnSprint(v2Direction);
        }
    }

	private void InitInput()
	{
		m_oInput = m_oInputFactory.GetInput(m_eInputSource);
		
		if(m_oInput != null)
		{
            m_oInput.Activate(JumpDetecet, AttackDetected, JumpAndAttack);
			m_oInput.InitInput();
		}
	}

	public void ChangeInput(eInputSource eNewInputSource)
	{
		if(eNewInputSource != m_eInputSource)
		{
			if(m_oInput != null)
			{
				m_oInput.Deactivate();
				m_oInput = null;
			}

			m_eInputSource = eNewInputSource;

			InitInput(); 
		}
	}

	//VARS
	public enum eInputSource
	{
		PLAYER_TOUCH = 0,
		PLAYER_MOUSE,
        PLAYER_KEYBORAD,
		COUNT
	}
	
    [SerializeField] private InputFactory m_oInputFactory;
	[SerializeField] private eInputSource 	m_eInputSource = eInputSource.PLAYER_KEYBORAD;

	private InputBase m_oInput;
}
