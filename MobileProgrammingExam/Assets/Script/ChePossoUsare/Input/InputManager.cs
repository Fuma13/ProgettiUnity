using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour 
{
	public delegate void InputEvent();
	public delegate void DetailedInputEvent(Vector3 position);
	public delegate void DetailedGestureEvent(Vector2 direction);
	public event InputEvent OnJump = null;
	public event InputEvent OnShoot = null;
	public event DetailedGestureEvent OnGesture = null;
	public event DetailedInputEvent OnShootDetailed = null;

	void Start()
	{
		InitInput();
	}
	
	void Update()
	{
		if(m_oInput != null)
		{
			m_oInput.InputUpdate();
		}
	}

	private void JumpDetecet()
	{
		if(OnJump != null)
		{
			OnJump();
		}
	}

	private void ShootDetected(Vector3 position)
	{
		if(OnShoot != null)
		{
			OnShoot();

			if(position != default(Vector3) && OnShootDetailed != null)
			{
				OnShootDetailed(position);
			}

		}
	}

	private void InputForGestureDetected(Vector2 point)
	{
		m_oGestureIdentifier.StartOrEndGestureInput (point);
	}

	private void GestureDetected(Vector2 direction)
	{
		if (OnGesture != null) {
			if(direction != default(Vector2)){
				OnGesture(direction);
			}
		}
	}

	private void InitInput()
	{
		m_oInput = InputFactory.GetInput(m_eInputSource);
		
		if(m_oInput != null)
		{
			m_oInput.Activate(JumpDetecet, ShootDetected, InputForGestureDetected);
			m_oInput.InitInput();
		}

		if (m_oGestureIdentifier == null) {
			m_oGestureIdentifier = gameObject.AddComponent<GestureIdentifier>();
		}
		m_oGestureIdentifier.Init (m_fGestureTime, GestureDetected);
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
		PLAYER = 0,
		PLAYER_MOUSE,
		AI,
		REPLAY,
		NETWORK,
		COUNT
	}
	
	[SerializeField] private eInputSource 	m_eInputSource = eInputSource.PLAYER;
	[SerializeField] private float			m_fGestureTime = 2.0f;

	private InputBase m_oInput;
	private GestureIdentifier m_oGestureIdentifier;
}
