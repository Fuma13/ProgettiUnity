using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour 
{
	public Button exit;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (exit.button_state_press)
        {
			Application.Quit();
        }
	}
}
