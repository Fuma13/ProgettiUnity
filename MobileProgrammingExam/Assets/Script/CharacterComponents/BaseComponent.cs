using UnityEngine;
using System.Collections;

//Base class to iherit usefull links
public class BaseComponent : MonoBehaviour 
{
    [SerializeField] protected GameManager m_oGameManager;
    [SerializeField] protected InputManager m_oInputManager;
    [SerializeField] protected Transform m_tTarget;
    [SerializeField] protected CharacterFSM m_oCharacterFSM;
}
