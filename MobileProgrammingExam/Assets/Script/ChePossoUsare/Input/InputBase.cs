using UnityEngine;
using System.Collections;
using System;

public abstract class InputBase : MonoBehaviour
{
	public void InitInput()
	{
        if(m_oJumpGesture != null)
        {
            m_oJumpIdentifier = new ContinuousGestureIdentifier();
            m_oJumpIdentifier.Init(m_bJumpAtEndInput, m_oJumpGesture);
            m_oJumpIdentifier.OnGestureDone += InternalJumpDetected;
        }
        if(m_oAttackGesture != null)
        {
            m_oAttackIdentifier = new ContinuousGestureIdentifier();
            m_oAttackIdentifier.Init(m_bAttackAtEndInput, m_oAttackGesture);
            m_oAttackIdentifier.OnGestureDone += InternalAttackDetected;
        }
        if(m_oSprintGesture != null)
        {
            m_oSprintIdentifier = new ContinuousGestureIdentifier();
            m_oSprintIdentifier.Init(m_bSprintAtEndInput, m_oSprintGesture);
            m_oSprintIdentifier.OnGestureDoneDirection += InternalSprintDetected;
        }
        InternalInitInput();
	}

    protected abstract void InternalInitInput();

    public void InputUpdate()
    {
        InternalInputUpdate();
    }

    protected abstract void InternalInputUpdate();


    protected void UpdateGesture(Vector3 v3Position, float fDeltaTime)
    {
        m_oAttackIdentifier.UpdateInput(v3Position, fDeltaTime);
        m_oJumpIdentifier.UpdateInput(v3Position, fDeltaTime);
        m_oSprintIdentifier.UpdateInput(v3Position, fDeltaTime);
    }

    protected void EndGesture(Vector3 v3Position, float fDeltaTime)
    {
        m_oAttackIdentifier.EndInput(v3Position, fDeltaTime);
        m_oJumpIdentifier.EndInput(v3Position, fDeltaTime);
        m_oSprintIdentifier.EndInput(v3Position, fDeltaTime);
    }

	protected void InternalJumpDetected()
	{
		if(m_actJumpCallback != null)
		{
			m_actJumpCallback();
		}
	}

	protected void InternalAttackDetected()
	{
        if (m_actAttackCallback != null)
		{
            m_actAttackCallback();
		}
	}

    protected void InternalSprintDetected(Vector2 v2Direction)
    {
        if (m_actSprintCallback != null)
        {
            m_actSprintCallback(v2Direction);
        }
    }

    public void Activate(Action actJumpInput, Action actAttackInput, Action<Vector2> actSprintInput)
	{
		m_actJumpCallback = actJumpInput;
        m_actAttackCallback = actAttackInput;
        m_actSprintCallback = actSprintInput;
	}
	
	public void Deactivate()
	{
		m_actJumpCallback = null;
		m_actAttackCallback = null;
		m_actSprintCallback = null;
	}

	//VARS
	protected event Action m_actJumpCallback = null;
	protected event Action m_actAttackCallback = null;
    protected event Action<Vector2> m_actSprintCallback = null;

    private ContinuousGestureIdentifier m_oJumpIdentifier;
    private ContinuousGestureIdentifier m_oAttackIdentifier;
    private ContinuousGestureIdentifier m_oSprintIdentifier;

    [SerializeField] private GestureFSM m_oJumpGesture;
    [SerializeField] private GestureFSM m_oAttackGesture;
    [SerializeField] private GestureFSM m_oSprintGesture;

    [SerializeField] private bool m_bJumpAtEndInput;
    [SerializeField] private bool m_bAttackAtEndInput;
    [SerializeField] private bool m_bSprintAtEndInput;
}
