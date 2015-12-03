using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using System;

public class ContinuousGestureIdentifier 
{
    public Action OnGestureDone;
    public Action<Vector2> OnGestureDoneDirection;

    public void Init(bool bAtInputEnd, GestureFSM oGestureFSM)
    {
        m_oContInput = new ContinuousInput(mk_iInputWindowSize);
        m_iCurrentGestureFSMIndex = 0;
        m_oGestureFSM = oGestureFSM;
        m_bAtInputEnd = bAtInputEnd;
    }

    public void UpdateInput(Vector3 v3Position, float fDeltaTime)
    {
        m_oContInput.AddPosition(v3Position, fDeltaTime);

        if (!m_bAtInputEnd)
        {
            CheckGesture();
        }
    }

    public void EndInput(Vector3 v3Position, float fDeltaTime)
    {
        m_oContInput.AddPosition(v3Position, fDeltaTime);
        CheckGesture();

        m_iCurrentGestureFSMIndex = 0;
        m_oContInput.Clear();
    }

    private void CheckGesture()
    {
        float fTime = 0.0f;
        float fDistance = 0.0f;
        Vector2 vDirection = Vector2.zero;
        GestureFSM.Gesture currentGesture;
        if (m_oGestureFSM.GetGesture(m_iCurrentGestureFSMIndex, out currentGesture))
        {
            m_oContInput.GetGestureStatus(out fDistance, out fTime, out vDirection);

            float fSpeed = fDistance / fTime;
            if (fDistance >= currentGesture.m_fMinDistanceForValidate && fSpeed > currentGesture.m_fMinSpeedForValidate)
            {
                bool bValidAngle = VectorUtils.IsAngleWithinThreshold(vDirection, mk_vUpVector, currentGesture.m_vReferenceDirection, currentGesture.m_iAngleTreshold);

                if (bValidAngle)
                {
                    if (!m_oGestureFSM.HasNextGesture(m_iCurrentGestureFSMIndex))
                    {
                        if (OnGestureDone != null)
                        {
                            OnGestureDone();
                        }
                        if(OnGestureDoneDirection != null)
                        {
                            OnGestureDoneDirection(vDirection);
                        }
                        m_iCurrentGestureFSMIndex = 0;
                        m_oContInput.Clear();
                    }
                    else
                    {
                        ++m_iCurrentGestureFSMIndex;
                    }
                }
            }
        }
    }

    private ContinuousInput m_oContInput;
    private int m_iCurrentGestureFSMIndex;

    private GestureFSM m_oGestureFSM;
    private bool m_bAtInputEnd = false;

    private const int mk_iInputWindowSize = 30;

    private readonly Vector3 mk_vUpVector = new Vector3(0.0f, 0.0f, 1.0f);
}
