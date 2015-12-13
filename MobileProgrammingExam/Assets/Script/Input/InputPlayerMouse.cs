using UnityEngine;
using System.Collections;

//Sub input for mouse
//It call the gesture update to check gestures
public class InputPlayerMouse : InputBase 
{
    protected override int GetInputCount()
    {
        return 1;
    }
    protected override void InternalInitInput()
    {
    }

    protected override void InternalInputUpdate()
    {
        if(Input.GetMouseButton(0))
        {
            UpdateGesture(0, Input.mousePosition, Time.unscaledDeltaTime);
        }
        if(Input.GetMouseButtonUp(0))
        {
            EndGesture(0, Input.mousePosition, Time.unscaledDeltaTime);
        }
    }
}
