using UnityEngine;
using System.Collections;

public class InputPlayerMouse : InputBase 
{
    protected override void InternalInitInput()
    {
    }

    protected override void InternalInputUpdate()
    {
        if(Input.GetMouseButton(0))
        {
            UpdateGesture(Input.mousePosition, Time.unscaledDeltaTime);
        }
        if(Input.GetMouseButtonUp(0))
        {
            EndGesture(Input.mousePosition, Time.unscaledDeltaTime);
        }
    }
    //public override void InputUpdate()
    //{
    //    base.InputUpdate();
		
    //    if(Input.GetMouseButtonDown(0))
    //    {

    //    }

    //    if (Input.GetMouseButtonUp (0)) 
    //    {
    //    }
    //}
}
