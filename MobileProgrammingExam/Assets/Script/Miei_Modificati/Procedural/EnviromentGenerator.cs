using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System;

public class EnviromentGenerator : MonoBehaviour 
{
	void Awake () 
	{
		SectionData oCurrentSectionVariant;
		m_oParent = new GameObject ();
		m_oParent.name = "Enviroment";
		//Create the pool of objects 
		for (int iType=0; iType<m_aoSectionType.Length; ++iType) 
		{
            //Foreach section type a create n variant pools
			oCurrentSectionVariant = m_aoSectionType[iType];
			int iVariantLength = oCurrentSectionVariant.m_aoSectionVariant.Length;
			oCurrentSectionVariant.m_aoSectionVariantPool = new GenericObjectPool[iVariantLength];

			//The size of the pool for this type is splited between variants
			int iPoolLength = Mathf.CeilToInt((float)oCurrentSectionVariant.m_iPullSize / (float)iVariantLength);
			for(int iVariant = 0; iVariant < iVariantLength; ++iVariant)
			{
				GameObject prefab = oCurrentSectionVariant.m_aoSectionVariant[iVariant];
				oCurrentSectionVariant.m_aoSectionVariantPool[iVariant] = 
					new GenericObjectPool(prefab.name,iPoolLength,prefab);
				oCurrentSectionVariant.m_aoSectionVariantPool[iVariant].GetParent().transform.parent = m_oParent.transform;
			}

			oCurrentSectionVariant.m_iTotProbability = 0;
			for(int iPossibleNext=0; iPossibleNext < oCurrentSectionVariant.m_aiNextTypeSection.Length; ++iPossibleNext)
			{
				oCurrentSectionVariant.m_iTotProbability += oCurrentSectionVariant.m_aiNextTypeSection[iPossibleNext].m_iProbability;
			}
			m_aoSectionType[iType] = oCurrentSectionVariant;
		}
	}

	void Start()
	{
		m_oRandom = new System.Random ();
		SetBegin ();
		GenerateSection(m_iStartGenerationCount);
	}

	void OnEnable()
	{
		m_oGameManager.OnRestartEvent += OnRestart;
	}

	void OnDisable()
	{
		m_oGameManager.OnRestartEvent -= OnRestart;
	}
	

	public void ReleaseOnInvisible(GameObject oSection, int iType, int iVariant)
	{
        DestroyObstacle[] oDestroyObstacles = oSection.GetComponentsInChildren<DestroyObstacle>();
        for (int i = 0; i < oDestroyObstacles.Length; ++i )
        {
            oDestroyObstacles[i].ResetObstacle();
        }
        m_aoSectionType[iType].m_aoSectionVariantPool[iVariant].ReleaseObject(oSection);
		GenerateSection (1);
	}


	private void SetBegin()
	{
        m_iLastSectionType = m_iBeginType;
        m_tLastPrefabTransform = m_tBeginTransform;
	}

	private void GenerateSection(int iLength)
	{
		GameObject oNextSection;
		for (int iCurrent = 0; iCurrent < iLength; ++iCurrent) 
		{
			m_iLastSectionType = GetNextPrefab(m_iLastSectionType,out oNextSection);
			if(m_iLastSectionType != -1 && oNextSection != null)
			{
				oNextSection.transform.position = m_tLastPrefabTransform.position + new Vector3(0.0f,0.0f,m_fSectionLength);
				m_tLastPrefabTransform = oNextSection.transform;
			}
			else
			{
				break;
			}
		}
	}

	private int GetNextPrefab(int iCurrentType, out GameObject oNextPrefab)
	{
		int iType = GetNextType (iCurrentType);
		int iVariant = GetPrefabVariant (iType);
		int iMaxLoop = 3;
		while (!m_aoSectionType [iType].m_aoSectionVariantPool [iVariant].HasFreeObjects
		      && iMaxLoop >= 0) 
		{
			Debug.LogWarning("Pool too small: Type: "+ iType + " Variant: " + iVariant);
			iType = GetNextType (iCurrentType);
			iVariant = GetPrefabVariant (iType);
			--iMaxLoop;
		}

		if (m_aoSectionType [iType].m_aoSectionVariantPool [iVariant].HasFreeObjects) 
		{
			oNextPrefab = m_aoSectionType [iType].m_aoSectionVariantPool [iVariant].GetObject ();
			SetReleaseComponent(iType,iVariant,oNextPrefab);
			return iType;
		} 
		else 
		{
			Debug.LogError("Pool too small: Type: "+ iType + " Variant: " + iVariant + " - Invalid GameObject returned");
			oNextPrefab = null;
			return -1;
		}
	}

	private void SetReleaseComponent(int iType, int iVariant, GameObject oPrefab)
	{
		int iCountChild = 0;

		ReleaseOnInvisible oReleaseComponent = oPrefab.GetComponent<ReleaseOnInvisible> ();
		if (oReleaseComponent == null) 
		{
			oReleaseComponent = oPrefab.AddComponent<ReleaseOnInvisible> ();
		}

		if (oPrefab.GetComponent<Collider> () != null) 
		{
			++iCountChild;
		}

		Collider[] oChildCollider = oPrefab.GetComponentsInChildren<Collider>();
		for(int iChild=0; iChild < oChildCollider.Length; ++iChild)
		{
			if(oPrefab != oChildCollider[iChild].gameObject)
			{
				ReleaseOnInvisible oChildReleaseComponent = oChildCollider[iChild].gameObject.GetComponent<ReleaseOnInvisible> ();
				if (oChildReleaseComponent == null) 
				{
					oChildReleaseComponent = oChildCollider[iChild].gameObject.AddComponent<ReleaseOnInvisible> ();
				}
				oChildReleaseComponent.SetupChild (oReleaseComponent);
				++iCountChild;
			}
		}

		oReleaseComponent.Setup (this, iType, iVariant,iCountChild);
		
	}

	private int GetNextType(int iCurrentType)
	{
		int iCurrentSectionNextLength = m_aoSectionType [iCurrentType].m_aiNextTypeSection.Length;
		int iTypeProbability = m_oRandom.Next(0,m_aoSectionType [iCurrentType].m_iTotProbability);
		int iCurrentProb = 0;
		for (int iNextTypeIndex= 0; iNextTypeIndex<iCurrentSectionNextLength; ++iNextTypeIndex) 
		{
			iCurrentProb += m_aoSectionType [iCurrentType].m_aiNextTypeSection[iNextTypeIndex].m_iProbability;
			if(iTypeProbability <= iCurrentProb)
			{
				return m_aoSectionType [iCurrentType].m_aiNextTypeSection [iNextTypeIndex].m_iType;
			}
		}

		//It never pass from here because
		//sum(m_aoSectionType [iCurrentType].m_aiNextTypeSection[iNextTypeIndex].m_iProbability) == m_aoSectionType [iCurrentType].m_iTotProbability;
		return 0;
	}

	private int GetPrefabVariant(int iType)
	{
		int currentTypeLength = m_aoSectionType [iType].m_aoSectionVariant.Length;
		return m_oRandom.Next (0, currentTypeLength) % currentTypeLength;
	}

	private void OnRestart()
	{
		ReleaseOnInvisible[] m_aoReleaseOnInvisibleScripts = m_oParent.GetComponentsInChildren<ReleaseOnInvisible> ();
		for (int iIndex = 0; iIndex < m_aoReleaseOnInvisibleScripts.Length; ++iIndex) 
		{
			m_aoReleaseOnInvisibleScripts[iIndex].DisableScript();
		}


		for (int iType=0; iType<m_aoSectionType.Length; ++iType) 
		{
			int iVariantLength = m_aoSectionType[iType].m_aoSectionVariant.Length;
			for(int iVariant = 0; iVariant < iVariantLength; ++iVariant)
			{
				m_aoSectionType[iType].m_aoSectionVariantPool[iVariant].ReleaseAllObjects();
			}
		}

		m_tLastPrefabTransform = m_tBeginTransform;
		GenerateSection (m_iStartGenerationCount);
	}

	[Header("Setup")]
	[SerializeField] private GameManager m_oGameManager;
	[SerializeField] private int m_iStartGenerationCount;

	[SerializeField] private Transform m_tBeginTransform;
	[SerializeField] private float m_fSectionLength;

	[SerializeField] private int m_iBeginType;
	
	[SerializeField] private SectionData[] m_aoSectionType;
	[Serializable] private struct SectionData
	{
        [SerializeField] private string m_sDebugName;
		public int m_iPullSize;
		public GameObject[] m_aoSectionVariant;
		public NextTypeData[] m_aiNextTypeSection;
		[HideInInspector] public int m_iTotProbability;
		[HideInInspector] public GenericObjectPool[] m_aoSectionVariantPool;
	}

	[Serializable] private struct NextTypeData
	{
		public int m_iType;
		[Tooltip("[0-100]%")]
		public int m_iProbability;
	}

	private int m_iLastSectionType;
	private System.Random m_oRandom;
	private Transform m_tLastPrefabTransform;
	private GameObject m_oParent;
}
