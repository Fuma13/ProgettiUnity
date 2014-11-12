#pragma strict

private var personaggio: GameObject;
private var personaggio_animazione: int; //1 idle, 2 run, 3 jump air, 4 jump land
public var personaggio_velo: float;
private var personaggio_alt:float;
private var personaggio_pos: float[];
private var personaggio_salto_ang: float;

personaggio_pos = new float[10]; //comprende posizione(0,1,2) rotazione(3,4,5) scala (6,7,8)

function Start () {

	personaggio=Instantiate(Resources.Load("Models/Belinda_anim1",GameObject));
	personaggio_salto_ang = 90*Mathf.Deg2Rad;
}

function Update () {

	//La velocità non cresce all'infinito xkè ogni cicolo riduco
	personaggio_velo *= .9;
	personaggio_alt += 30*Mathf.Cos(personaggio_salto_ang)*Time.deltaTime;

	if(personaggio_alt < .05){
	
		if(Input.GetKey(KeyCode.W)){
			personaggio_velo += 1 * Time.deltaTime;
		}
		
		if(Input.GetKey(KeyCode.A)){
			personaggio_pos[4] -= 200*(Time.deltaTime); 
		}
		if(Input.GetKey(KeyCode.D)){
			personaggio_pos[4] += 200*(Time.deltaTime); 
		}
		personaggio_salto_ang = 90*Mathf.Deg2Rad;
	}
	else {
		personaggio_salto_ang -= 0.1;
	}
	
	if(personaggio_animazione < 3 && Input.GetKeyDown(KeyCode.Space)){
		
		personaggio_salto_ang = 0;
	}
	
	//Compenso rotazione del modello rispetto alle nostre convenzioni per applicarla nel movimento
	var rad:float = (360-personaggio_pos[4])*Mathf.Deg2Rad;
	//Uso il coseno e seno della rotazione della y per regolare la direzione in base alla mia rotazione
	personaggio_pos[0] = personaggio_pos[0] + Mathf.Cos(rad)*personaggio_velo;
	personaggio_pos[1] = personaggio_alt;
	personaggio_pos[2] = personaggio_pos[2] + Mathf.Sin(rad)*personaggio_velo;
	
	
	
	personaggio.transform.position = Vector3(personaggio_pos[0],personaggio_pos[1],personaggio_pos[2]);
	//Costante di rotazione per sistemare gli assi del modello con convenzione diversa
	personaggio.transform.localEulerAngles = Vector3(0,personaggio_pos[4]+90,0);
	
	//Passaggio animazioni
	if(personaggio_alt < .05){
		
		if(personaggio_velo < .01){
			Idle();
		}
		else{
			Run();
		}
	}
	else{
		Jump();
	}
}

function Idle(){
	if(personaggio_animazione != 1){
		Debug.Log("Idle");
		personaggio_animazione = 1;
		personaggio.animation.CrossFade("Idle",0.55);
		personaggio.animation.wrapMode = WrapMode.Loop;
		personaggio.animation["Idle"].speed = 1;
	}
}

function Run(){
	if(personaggio_animazione == 1){
		Debug.Log("Run");
		personaggio_animazione = 2;
		personaggio.animation.CrossFade("Run",0.55);
		personaggio.animation.wrapMode = WrapMode.Loop;
	}
	personaggio.animation["Run"].speed = personaggio_velo * 5;
}

function Jump(){
	if(personaggio_animazione != 3){
		Debug.Log("Jump");
		personaggio_animazione = 3;
		personaggio.animation.CrossFade("Jump", 0.1);
		personaggio.animation.wrapMode = WrapMode.Once;
		personaggio.animation["Jump"].speed = 1;
	}
}