using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using System;

public class ContinuousGestureIdentifier : MonoBehaviour 
{
    public Action<int> OnGestureDone;

    void Awake()
    {
        Init();
    }

    public void Init()
    {
        m_aoInputInfos = new InputInfo[mk_iMaxInputNumber];
        InitInput();
        m_bInit = true;
    }

    private void InitInput()
    {
        for (int i = 0; i < mk_iMaxInputNumber; ++i)
        {
            m_aoInputInfos[i].m_oContInput = new ContinuousInput(mk_iInputWindowSize);
            m_aoInputInfos[i].m_bStarted = false;
            m_aoInputInfos[i].m_iCurrentGestureFSMIndex = 0;
        }
    }

    private void InputFinished(int iID)
    {
        m_aoInputInfos[iID].m_oContInput.Clear();

        m_aoInputInfos[iID].m_bStarted = false;
    }

    private void CheckGesture(int iID)
    {
        for (int iGestureFSM = 0; iGestureFSM < m_aoGestureFSM.Length; ++iGestureFSM)
        {
            float fTime = 0.0f;
            float fDistance = 0.0f;
            Vector3 vDirection = Vector3.zero;
            GestureFSM.Gesture currentGesture;
            if (m_aoGestureFSM[iGestureFSM].GetGesture(m_aoInputInfos[iID].m_iCurrentGestureFSMIndex, out currentGesture))
            {
                m_aoInputInfos[iID].m_oContInput.GetGestureStatus(out fDistance, out fTime, out vDirection);

                float fSpeed = fDistance / fTime;
                if (fDistance >= currentGesture.m_fMinDistanceForValidate && fSpeed > currentGesture.m_fMinSpeedForValidate)
                {
                    bool bValidAngle = VectorUtils.IsAngleWithinThreshold(vDirection, mk_vUpVector, currentGesture.m_vReferenceDirection, currentGesture.m_iAngleTreshold);

                    if (bValidAngle)
                    {
                        //Clear the gesture info..
                        //m_aoInputInfos[iID].m_oContInput.Clear(); ?? 
                        Debug.Log("Gesture: " + iGestureFSM + " done");
                        if (!m_aoGestureFSM[iGestureFSM].HasNextGesture(m_aoInputInfos[iID].m_iCurrentGestureFSMIndex))
                        {
                            if (OnGestureDone != null)
                            {
                                OnGestureDone(iGestureFSM);
                            }
                            m_aoInputInfos[iID].m_iCurrentGestureFSMIndex = 0;
                            //m_aoGestureFSM[iGestureFSM].Reset();
                        }
                        else
                        {
                            ++m_aoInputInfos[iID].m_iCurrentGestureFSMIndex;
                        }
                    }
                }
            }
            else
            {

            }
        }
    }

    private struct InputInfo
    {
        public ContinuousInput m_oContInput;
        public int m_iCurrentGestureFSMIndex;
        public bool m_bStarted;
    }

    [SerializeField]private GestureFSM[] m_aoGestureFSM;
    private InputInfo[] m_aoInputInfos;
    private bool m_bInit = false;

    private const int mk_iMaxInputNumber = 10;
    private const int mk_iInputWindowSize = 30;

    private readonly Vector3 mk_vUpVector = new Vector3(0.0f, 0.0f, 1.0f);
}
