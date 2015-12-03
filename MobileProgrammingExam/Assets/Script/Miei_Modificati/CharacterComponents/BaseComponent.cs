using UnityEngine;
using System.Collections;

public class BaseComponent : MonoBehaviour {


    [Header("Setup Base")]
    [SerializeField] protected GameManager m_oGameManager;
    [SerializeField] protected InputManager m_oInputManager;
	[SerializeField] protected Transform m_tTarget;
    [SerializeField] protected CharacterFSM m_oCharacterFSM;
}
