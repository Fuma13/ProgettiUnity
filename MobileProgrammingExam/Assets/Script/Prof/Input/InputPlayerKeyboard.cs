using UnityEngine;
using System.Collections;
using System;

public class InputPlayerKeyboard : InputBase
{
    protected override void InternalInitInput()
    {
    }
	protected override void InternalInputUpdate()
	{
		if(Input.GetKeyDown(m_eJumpKeycode))
		{
			InternalJumpDetected();
		}

		if(Input.GetKeyDown(m_eAttackKeycode))
		{
			InternalAttackDetected();
		}


        if(Input.GetKeyDown(m_eSprintKeycode))
        {
            v2SprintDirection.x = Input.GetAxis("Horizontal");
            v2SprintDirection.y = Input.GetAxis("Vertical");
            InternalSprintDetected(v2SprintDirection.normalized);
        }
	}

    private Vector2 v2SprintDirection = Vector2.zero;
    [SerializeField] private KeyCode m_eJumpKeycode;
    [SerializeField] private KeyCode m_eAttackKeycode;
    [SerializeField] private KeyCode m_eSprintKeycode;

}
