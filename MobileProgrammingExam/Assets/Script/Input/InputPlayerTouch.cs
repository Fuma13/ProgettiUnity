using UnityEngine;
using System.Collections;

//Sub input for mouse
//It call the gesture update to check gestures foreach finger used
public class InputPlayerTouch : InputBase 
{
    protected override int GetInputCount()
    {
        return mk_iMaxTouchNumber;
    }
    protected override void InternalInitInput()
    {
    }

    protected override void InternalInputUpdate()
    {
        for (int i = 0; i < Input.touchCount; ++i)
        {
            if (Input.touches[i].fingerId >= mk_iMaxTouchNumber)
            {
                Debug.LogError("Finger ID excedes max touch numbers");
                return;
            }
            Vector3 vPos = Input.touches[i].position;
            vPos.x /= Screen.width;
            vPos.y /= Screen.height;
            if(Input.touches[i].phase != TouchPhase.Ended ||Input.touches[i].phase != TouchPhase.Canceled)
            {
                UpdateGesture(Input.touches[i].fingerId, vPos, Time.unscaledDeltaTime);
            }
            else
            {
                EndGesture(Input.touches[i].fingerId, vPos, Time.unscaledDeltaTime);
            }
        }
    }

    private int mk_iMaxTouchNumber = 10;
}
