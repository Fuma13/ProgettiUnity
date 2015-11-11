using UnityEngine;
using System.Collections;

public class Translation : MonoBehaviour 
{
	public float speed, forceJump;
	public float speedJump;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		//transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.World);
		
		/*if(Input.GetKeyDown(KeyCode.Space))
		{
			Debug.Log("space");
			transform.Translate(Vector3.up * forceJump * Time.deltaTime);
			//transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x,transform.position.y + forceJump, transform.position.z), speedJump);
			//transform.rigidbody.AddForce(transform.up, ForceMode.Impulse);
		}*/
	}
}
