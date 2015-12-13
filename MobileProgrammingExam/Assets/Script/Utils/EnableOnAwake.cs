using UnityEngine;
using System.Collections;

//On awake enable all objects in the list
public class EnableOnAwake : MonoBehaviour {

	void Awake () {

		for (int iIndex = 0; iIndex < m_oToEnable.Length; ++iIndex) 
		{
			if(!m_oToEnable[iIndex].activeInHierarchy)
			{
				m_oToEnable[iIndex].SetActive(true);
			}
		}

	}

	[SerializeField] GameObject[] m_oToEnable;
}
