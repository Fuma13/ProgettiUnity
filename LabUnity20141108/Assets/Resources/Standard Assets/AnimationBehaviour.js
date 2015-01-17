#pragma strict

private var personaggio: GameObject;
private var personaggio_animazione: int; //1 idle, 2 run, 3 jump air, 4 jump land
private var personaggio_velo: float;
private var personaggio_alt:float;
private var personaggio_pos: float[];
private var personaggio_salto_ang: float;

personaggio_pos = new float[10]; //comprende posizione(0,1,2) rotazione(3,4,5) scala (6,7,8)

function Start () {

	personaggio=Instantiate(Resources.Load("Models/Belinda_anim1",GameObject));
	personaggio_salto_ang = 90*Mathf.Deg2Rad;
}

function Update () {
	
	personaggio_alt += 30*Mathf.Cos(personaggio_salto_ang)*Time.deltaTime;

	if(personaggio_alt < .05){
	
		//La velocità non cresce all'infinito xkè ogni cicolo riduco
		personaggio_velo *= .8;
		//Faccio riusltare 0 il calcolo del personaggio_alt fatto sopra (inutile xkè azzero dopo)
		personaggio_salto_ang = 1.570796325;//90*Mathf.Deg2Rad;
		//Lo posiziono a 0
		personaggio_alt = 0;
	
		if(Input.GetKey(KeyCode.W)){
			personaggio_velo += 4 * Time.deltaTime;
		}
		
		if(Input.GetKey(KeyCode.A)){
			personaggio_pos[4] -= 200*(Time.deltaTime); 
		}
		if(Input.GetKey(KeyCode.D)){
			personaggio_pos[4] += 200*(Time.deltaTime); 
		}
	}
	else {
		//personaggio_salto_ang += 0.1;
		//Adatto l'andamento del salto all'animazione
		personaggio_salto_ang = Mathf.Lerp(0,Mathf.PI,personaggio.animation["Jump"].normalizedTime); //180
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
		
		//if(personaggio_animazione != 4)
		{
			if(personaggio_velo < .1){
				Idle();
			}
			else{
				Run();
			}
		}
//		else{
//			JumpLand();
//		}
	}
	else{
//		if(personaggio_alt > .3){
			Jump();
//		}
//		else{
//			//JumpLand();
//		}
	}
}

function Idle(){
	if(personaggio_animazione != 1){
		Debug.Log("Idle");
		personaggio_animazione = 1;
		personaggio.animation.CrossFade("Idle", 0.4);
		personaggio.animation.wrapMode = WrapMode.Loop;
		personaggio.animation["Idle"].speed = 1;
	}
}

function Run(){
	if(personaggio_animazione != 2){
		Debug.Log("Run");
		personaggio_animazione = 2;
		personaggio.animation.CrossFade("Run",0.2);
		personaggio.animation.wrapMode = WrapMode.Loop;
	}
	personaggio.animation["Run"].speed = personaggio_velo * 1;
}

function Jump(){
	if(personaggio_animazione != 3){
		Debug.Log("Jump");
		personaggio_animazione = 3;
		personaggio.animation.CrossFade("Jump", 0.1);
		personaggio.animation.wrapMode = WrapMode.ClampForever;
		personaggio.animation["Jump"].speed = .9;
	}
}

//function JumpLand(){
//	if(personaggio_animazione == 3){
//		Debug.Log("JumpLanding");
//		personaggio_animazione = 4;
//		personaggio.animation.Play("JumpLanding");
//		personaggio.animation.wrapMode = WrapMode.ClampForever;
//		personaggio.animation["JumpLanding"].speed = 1;
//	}
//	else if(personaggio_animazione == 4)
//	{
//		if(personaggio.animation["JumpLanding"].normalizedTime > .9){
//			personaggio_animazione = 0;
//		}
//	}
//}