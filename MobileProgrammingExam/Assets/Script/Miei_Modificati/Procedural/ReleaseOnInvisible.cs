using UnityEngine;
using System.Collections;

public class ReleaseOnInvisible : MonoBehaviour 
{
	public void Setup(EnviromentGenerator oEviromentGenerator, int iType, int iVariant, int iChildCount)
	{
		m_oEviromentGenerator = oEviromentGenerator;
		m_iType = iType;
		m_iVariant = iVariant;
		m_iChildCount = iChildCount;
	}

	public void SetupChild(ReleaseOnInvisible oReleaseParent)
	{
		m_oReleaseParent = oReleaseParent;
	}

	public void DisableScript()
	{
		m_oEviromentGenerator = null;
		m_oReleaseParent = null;
	}

	void OnBecameInvisible()
	{
		if (m_oReleaseParent != null) {
			m_oReleaseParent.ReleaseChild ();
		} else {
			ReleaseChild();
		}
	}

	private void ReleaseParent()
	{
		if (m_oEviromentGenerator != null) 
		{
			m_oEviromentGenerator.ReleaseOnInvisible (gameObject, m_iType, m_iVariant);
			if(m_bDebug)
				Debug.Log("Realesed: " + gameObject.name);
		}
	}

	private void ReleaseChild()
	{
		--m_iChildCount;
		if (m_iChildCount <= 0) 
		{
			ReleaseParent();
		}
	}

	private EnviromentGenerator m_oEviromentGenerator;
	private ReleaseOnInvisible m_oReleaseParent;
	private int m_iType;
	private int m_iVariant;
	private int m_iChildCount;

	private bool m_bDebug = true;
}
