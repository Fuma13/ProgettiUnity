using System.Collections;
using UnityEngine;

//Pool of MonoBehaviour scripts with GameObject
public class MonobehaviourObjectPool<T> where T : MonoBehaviour
{	
	public MonobehaviourObjectPool(string poolObjectName, int maxSize, GameObject instance)
	{
		m_iMaxSize = maxSize;
		m_iNumObjAvaiable = maxSize;
		m_aObjectArray = new T[maxSize];
		m_aCratedObjectFlag = new bool[maxSize];

		GameObject parent = GameObject.Find (poolObjectName);
		if(parent == null){
			//Create a parent object where insert T objects
			parent = new GameObject ();
			parent.name = poolObjectName;
		}

		//Instatiate all objects in the pool
		for (int i=0; i<m_iMaxSize; ++i) {
			GameObject instantiated = (GameObject)GameObject.Instantiate(instance, Vector3.zero, Quaternion.identity);
			instantiated.SetActive(false);
			instantiated.transform.parent = parent.transform;
			m_aObjectArray[i] = instantiated.GetComponent<T>();
			if(m_aObjectArray[i] == null)
			{
				m_aObjectArray[i] = instantiated.AddComponent<T>();
			}
		}
	}

	//Return a free instance of T not initializated
	//If the pool is empty it return null obj
	public T GetObject()
	{
		if (HasFreeObjects) 
		{
			int avaiable = 0;
			//Search the first position avaiable
			for(; avaiable < m_iMaxSize && m_aCratedObjectFlag[avaiable]; ++avaiable){}
			m_aCratedObjectFlag[avaiable] = true;
			//Enable the object
			m_aObjectArray[avaiable].transform.gameObject.SetActive(true);
			--m_iNumObjAvaiable;
			return m_aObjectArray[avaiable];
		}
		return default(T);
	}

	//Set as avaible the obj
	public void ReleaseObject(T obj)
	{
		for (int index = 0; index <  m_iMaxSize; ++index) 
		{
			if (m_aCratedObjectFlag [index] && m_aObjectArray [index].Equals (obj)) 
			{
				m_aCratedObjectFlag [index] = false;
				//Disable the object
				m_aObjectArray [index].gameObject.SetActive (false);
				++m_iNumObjAvaiable;
			}
		}
	}

	public bool HasFreeObjects
	{
		get{ return (m_iNumObjAvaiable > 0);}
	}

	private int m_iMaxSize;
	private int m_iNumObjAvaiable;
	private int m_iFirstFree;
	private T[] m_aObjectArray;
	private bool[] m_aCratedObjectFlag;
}
