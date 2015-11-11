using UnityEngine;
using System.Collections;
using System;

public class InputPlayerKeyboard : InputBase
{
	public override void InputUpdate()
	{
		base.InputUpdate();

		if(Input.GetKeyDown(KeyCode.J))
		{
			InternalJumpDetected();
		}

		if(Input.GetKeyDown(KeyCode.Space))
		{
			InternalShootDetected(default(Vector3));
		}
	}
}
