using UnityEngine;
using System.Collections;

public class InputPlayerMouse : InputBase 
{

	public override void InputUpdate()
	{
		base.InputUpdate();
		
		if(Input.GetMouseButtonDown(0))
		{
			InternalGestureDetected(Input.mousePosition);

			Vector3 mouse = Input.mousePosition;
			mouse.z = Camera.main.nearClipPlane;
			InternalShootDetected(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		}

		if (Input.GetMouseButtonUp (0)) 
		{
			InternalGestureDetected(Input.mousePosition);
		}
	}
}
