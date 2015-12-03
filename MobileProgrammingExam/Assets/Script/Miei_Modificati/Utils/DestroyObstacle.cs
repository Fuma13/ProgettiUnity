using UnityEngine;
using System.Collections;

public class DestroyObstacle : MonoBehaviour 
{
    public void DestoryObstacle()
    {
        if (!m_bDestroyed)
        {
            gameObject.SetActive(false);
            m_oCollider.isTrigger = true;
            //m_oColor = m_oRenderer.material.color;
            //m_oColor.a = 0.3f;
            //m_oRenderer.material.color = m_oColor;
            m_bDestroyed = true;
        }
    }

    public void ResetObstacle()
    {
        if(m_bDestroyed)
        {
            gameObject.SetActive(true);
            m_oCollider.isTrigger = false;
            //m_oColor = m_oRenderer.material.color;
            //m_oColor.a = 1f;
            //m_oRenderer.material.color = m_oColor;
            m_bDestroyed = false;
        }
    }

    public bool Destroyed
    {
        get { return m_bDestroyed; }
    }

    [SerializeField] Renderer m_oRenderer;
    [SerializeField] Collider m_oCollider;
    private bool m_bDestroyed = false;
    private Color m_oColor;
}
