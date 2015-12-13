using UnityEngine;
using System.Collections;

//ReleaseOnInvisible is added to parent object and its childs.
//It take count of how many childs are visible and how many are become invisible.
//When all childs and this object become invisible, it notify the evniroment generator
public class ReleaseOnInvisible : MonoBehaviour 
{
    //Setup the parent that take count of all childs
	public void Setup(EnviromentGenerator oEviromentGenerator, int iType, int iVariant, int iChildCount)
	{
		m_oEviromentGenerator = oEviromentGenerator;
		m_iType = iType;
		m_iVariant = iVariant;
		m_iChildCount = iChildCount;
	}

    //Setup the child, so it can notify the parent when it's invisible
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
        //if there is a link to m_oReleaseParent, this objects it's a child and notify the parent
		if (m_oReleaseParent != null) {
			m_oReleaseParent.ReleaseChild ();
		} else {
            //Otherwise release itself
			ReleaseChild();
		}
	}

    //When all childs and itself become invisible, it notify the enviroment generator
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
