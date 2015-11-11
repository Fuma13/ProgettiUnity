using UnityEngine;
using System.Collections;

public class CollisionEvent : MonoBehaviour {

	public delegate void SimpleCollisionCallback();
	public delegate void DetailedCollisionCallback();
	public SimpleCollisionCallback SimpleCollisionEnter;
	public SimpleCollisionCallback SimpleCollisionExit;


	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == m_oLayerMask) {
			Debug.Log ("Enter");
			if (SimpleCollisionEnter != null) {
				SimpleCollisionEnter ();
			}
		}
	}

	void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.layer == m_oLayerMask) {
			Debug.Log ("Exit");
			if (SimpleCollisionExit != null) {
				SimpleCollisionExit ();
			}
		}
	}

	[SerializeField] private LayerMask m_oLayerMask;
}
