using UnityEngine;
using System.Collections;

public class InputFactory : MonoBehaviour
{
	public InputBase GetInput(InputManager.eInputSource eInputType)
	{
		InputBase oInputImplementation = null;

		switch(eInputType)
		{
		case InputManager.eInputSource.PLAYER_TOUCH:
                oInputImplementation = m_oPlayerTouch;
			break;
		case InputManager.eInputSource.PLAYER_MOUSE:
            oInputImplementation = m_oPlayerMouse;
			break;
        case InputManager.eInputSource.PLAYER_KEYBORAD:
            oInputImplementation = m_oPlayerKeyboard;
            break;
		};

		if(oInputImplementation == null)
		{
			Debug.LogError("Input implementation not available!");
		}

		return oInputImplementation;
	}

    [SerializeField] InputBase m_oPlayerTouch;
    [SerializeField] InputBase m_oPlayerMouse;
    [SerializeField] InputBase m_oPlayerKeyboard;
}
