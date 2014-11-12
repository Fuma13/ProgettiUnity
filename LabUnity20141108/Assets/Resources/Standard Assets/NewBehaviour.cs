using UnityEngine;
using System.Collections;

public class NewBehaviour : MonoBehaviour {

	private enum State{
		Idle,
		Run,
		Jump
	}

	private State character_state;
	private bool ground;
	// Use this for initialization
	void Start () {
		character_state = State.Idle;
		animation.Play ("Idle");
		ground = true;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey (KeyCode.W)) {
			if (character_state == State.Idle) {
				character_state = State.Run;
				animation.CrossFade ("Run");
			}
		} 
		else{
			if(character_state != State.Idle){

				animation.CrossFade("Idle");
			}

		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			if(character_state != State.Jump){
				character_state = State.Jump;
				animation.CrossFade("Jump");
			}
		}

		if (character_state == State.Jump) {
			if(ground){
				character_state = State.Idle;
			}
		}
		
	
	}
}
