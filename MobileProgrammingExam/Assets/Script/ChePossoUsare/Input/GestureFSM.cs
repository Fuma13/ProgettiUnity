using UnityEngine;
using System.Collections;

public class GestureFSM : MonoBehaviour
{
    public bool GetGesture(int iGestureIndex, out Gesture oGesture)
    {
        if (iGestureIndex >= 0 && iGestureIndex < m_aoGestures.Length)
        {
            oGesture = m_aoGestures[iGestureIndex];
            return true;
        }
        else
        {
            oGesture = default(Gesture);
            return false;
        }
    }

    public bool HasNextGesture(int iGestureIndex)
    {
        if (iGestureIndex>= 0 && iGestureIndex < m_aoGestures.Length - 1)
        {
            return true;
        }
        return false;
    }


    [System.Serializable]
    public struct Gesture
    {
        public int m_iAngleTreshold;
        public float m_fMinSpeedForValidate;
        public float m_fMinDistanceForValidate;
        public Vector3 m_vReferenceDirection;
    }

    [SerializeField] private Gesture[] m_aoGestures;
}
