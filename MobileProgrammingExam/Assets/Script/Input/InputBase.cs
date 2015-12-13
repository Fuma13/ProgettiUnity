using UnityEngine;
using System.Collections;
using System;

//Based class, all input type inherit form here.
//It's also an interface to user, all commands and events pass by this class to child class using internal functions
public abstract class InputBase : MonoBehaviour
{
    //Init the gestures to monitor
    //Create an identifier foreach simultaneous input
	public void InitInput()
	{
        int iNumGestureInput = GetInputCount();
        if(m_oJumpGesture != null)
        {
            m_oJumpIdentifier = new ContinuousGestureIdentifier[iNumGestureInput];
            for (int i = 0; i < iNumGestureInput; ++i)
            {
                m_oJumpIdentifier[i] = new ContinuousGestureIdentifier();
                m_oJumpIdentifier[i].Init(m_bJumpAtEndInput, m_oJumpGesture);
                m_oJumpIdentifier[i].OnGestureDone += InternalJumpDetected;
            }
        }
        if(m_oAttackGesture != null)
        {
            m_oAttackIdentifier = new ContinuousGestureIdentifier[iNumGestureInput];
            for (int i = 0; i < iNumGestureInput; ++i)
            {
                m_oAttackIdentifier[i] = new ContinuousGestureIdentifier();
                m_oAttackIdentifier[i].Init(m_bAttackAtEndInput, m_oAttackGesture);
                m_oAttackIdentifier[i].OnGestureDone += InternalAttackDetected;
            }
        }
        if(m_oSprintGesture != null)
        {
            m_oSprintIdentifier = new ContinuousGestureIdentifier[iNumGestureInput];
            for (int i = 0; i < iNumGestureInput; ++i)
            {
                m_oSprintIdentifier[i] = new ContinuousGestureIdentifier();
                m_oSprintIdentifier[i].Init(m_bSprintAtEndInput, m_oSprintGesture);
                m_oSprintIdentifier[i].OnGestureDoneDirection += InternalSprintDetected;
            }
        }
        InternalInitInput();
	}
    //Return the number of simultaneous inputs the system can manage 
    protected abstract int GetInputCount();

    protected abstract void InternalInitInput();

    public void InputUpdate()
    {
        InternalInputUpdate();
    }

    protected abstract void InternalInputUpdate();

    //Update all gestures with data form inputs
    protected void UpdateGesture(int iID, Vector3 v3Position, float fDeltaTime)
    {
        m_oAttackIdentifier[iID].UpdateInput(v3Position, fDeltaTime);
        m_oJumpIdentifier[iID].UpdateInput(v3Position, fDeltaTime);
        m_oSprintIdentifier[iID].UpdateInput(v3Position, fDeltaTime);
    }

    //End gestures with data from inputs
    protected void EndGesture(int iID, Vector3 v3Position, float fDeltaTime)
    {
        m_oAttackIdentifier[iID].EndInput(v3Position, fDeltaTime);
        m_oJumpIdentifier[iID].EndInput(v3Position, fDeltaTime);
        m_oSprintIdentifier[iID].EndInput(v3Position, fDeltaTime);
    }

    //Called from gesture's check or directly from sub input type (as InputPlayerKeyboard)
	protected void InternalJumpDetected()
	{
		if(m_actJumpCallback != null)
		{
			m_actJumpCallback();
		}
	}

    //Called from gesture's check or directly from sub input type (as InputPlayerKeyboard)
    protected void InternalAttackDetected()
	{
        if (m_actAttackCallback != null)
		{
            m_actAttackCallback();
		}
	}

    //Called from gesture's check or directly from sub input type (as InputPlayerKeyboard)
    protected void InternalSprintDetected(Vector2 v2Direction)
    {
        if (m_actSprintCallback != null)
        {
            m_actSprintCallback(v2Direction);
        }
    }

    //Activate the callback of inputs
    public void Activate(Action actJumpInput, Action actAttackInput, Action<Vector2> actSprintInput)
	{
		m_actJumpCallback = actJumpInput;
        m_actAttackCallback = actAttackInput;
        m_actSprintCallback = actSprintInput;
	}

    //Deactivate the callback of inputs
    public void Deactivate()
	{
		m_actJumpCallback = null;
		m_actAttackCallback = null;
		m_actSprintCallback = null;
	}

	//VARS
    private event Action m_actJumpCallback = null;
	private event Action m_actAttackCallback = null;
    private event Action<Vector2> m_actSprintCallback = null;

    private ContinuousGestureIdentifier[] m_oJumpIdentifier;
    private ContinuousGestureIdentifier[] m_oAttackIdentifier;
    private ContinuousGestureIdentifier[] m_oSprintIdentifier;

    [SerializeField] private GestureFSM m_oJumpGesture;
    [SerializeField] private GestureFSM m_oAttackGesture;
    [SerializeField] private GestureFSM m_oSprintGesture;

    [SerializeField] private bool m_bJumpAtEndInput;
    [SerializeField] private bool m_bAttackAtEndInput;
    [SerializeField] private bool m_bSprintAtEndInput;
}
