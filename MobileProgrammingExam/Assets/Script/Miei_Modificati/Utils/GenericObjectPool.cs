using UnityEngine;
using System.Collections;

public class GenericObjectPool
{
	public GenericObjectPool(string poolObjectName, int maxSize, GameObject instance)
	{
		m_iMaxSize = maxSize;
		m_iNumObjAvaiable = maxSize;
		m_aObjectArray = new GameObject[maxSize];
		m_aCratedObjectFlag = new bool[maxSize];
		
		m_oParent = GameObject.Find (poolObjectName);
		if(m_oParent == null){
			//Create a parent object where insert T objects
			m_oParent = new GameObject ();
			m_oParent.name = poolObjectName;
		}
		
		//Instatiate all objects in the pool
		for (int i=0; i<m_iMaxSize; ++i) {
			GameObject instantiated = (GameObject)GameObject.Instantiate(instance, Vector3.zero, Quaternion.identity);
			instantiated.SetActive(false);
			instantiated.transform.parent = m_oParent.transform;
			m_aObjectArray[i] = instantiated;
		}
	}

	public GameObject GetParent()
	{
		return m_oParent;
	}
	
	//Return a free instance of T not initializated
	//If the pool is empty it return null obj
	public GameObject GetObject()
	{
		if (HasFreeObjects) 
		{
			int avaiable = 0;
			//Search the first position avaiable
			for(; avaiable < m_iMaxSize && m_aCratedObjectFlag[avaiable]; ++avaiable){}
            if (!m_aObjectArray[avaiable].activeInHierarchy)
            {
                m_aCratedObjectFlag[avaiable] = true;
                //Enable the object
                m_aObjectArray[avaiable].transform.gameObject.SetActive(true);
                --m_iNumObjAvaiable;
                return m_aObjectArray[avaiable];
            }
            else
            {
                Debug.LogError("Trying to get an active object");
            }
		}
		return null;
	}
	
	//Set as avaible the obj
	public void ReleaseObject(GameObject obj)
	{
		for (int index = 0; index <  m_iMaxSize; ++index) 
		{
			if (m_aCratedObjectFlag [index] && m_aObjectArray [index].Equals (obj)) 
			{
                ReleaseObjectAtIndex(index);
			}
		}
	}

	public void ReleaseAllObjects()
	{
		for (int index = 0; index <  m_iMaxSize; ++index) 
		{
			if (m_aCratedObjectFlag [index]) 
			{
                ReleaseObjectAtIndex(index);
			}
		}
	}

    private void ReleaseObjectAtIndex(int iIndex)
    {
        m_aCratedObjectFlag[iIndex] = false;
        //Disable the object
        m_aObjectArray[iIndex].gameObject.SetActive(false);
        ++m_iNumObjAvaiable;
    }
	
	public bool HasFreeObjects
	{
		get{ return (m_iNumObjAvaiable > 0);}
	}
	
	private int m_iMaxSize;
	private int m_iNumObjAvaiable;
	private int m_iFirstFree;
	private GameObject[] m_aObjectArray;
	private bool[] m_aCratedObjectFlag;
	private GameObject m_oParent;
}
