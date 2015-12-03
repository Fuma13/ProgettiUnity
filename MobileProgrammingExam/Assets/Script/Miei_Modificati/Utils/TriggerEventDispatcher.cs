using UnityEngine;
using System.Collections;
using System;

public class TriggerEventDispatcher : MonoBehaviour {

    public Action<Collider> OnTriggernEnterEvent;
    public Action<Collider> OnTriggernExitEvent;


    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & m_oLayerMask.value) != 0)
        {
            if (OnTriggernEnterEvent != null)
            {
                OnTriggernEnterEvent(other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == m_oLayerMask)
        {
            if (OnTriggernExitEvent != null)
            {
                OnTriggernExitEvent(other);
            }
        }
    }

    [SerializeField] private LayerMask m_oLayerMask;
}
