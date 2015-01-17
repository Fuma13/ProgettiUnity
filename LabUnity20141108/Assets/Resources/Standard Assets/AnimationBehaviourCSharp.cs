using UnityEngine;
using System.Collections;

public class AnimationBehaviourCSharp : MonoBehaviour {

    private GameObject personaggio;
    private int personaggio_animazione;
    private float personaggio_velo;
    private float personaggio_alt;
    private float[] personaggio_pos;
    private float personaggio_salto_ang;

    private const float HALF_PI = Mathf.PI/2f;// 1.570796325f;
    private const float ALTEZZA_MAX = 5;
    private float TEMPO_DI_VOLO;

    private float startJumpTime;

	// Use this for initialization
	void Start () {
        personaggio_pos = new float[10];
        personaggio = Instantiate(Resources.Load("Models/Belinda_anim1")) as GameObject;
        personaggio_salto_ang = 0;
        startJumpTime = -1;
        TEMPO_DI_VOLO = Mathf.Sqrt(8 * ALTEZZA_MAX / 19.6f);
        Debug.Log(TEMPO_DI_VOLO);
	}
	
	// Update is called once per frame
	void Update () {

        //personaggio_alt += 30 * Mathf.Cos(personaggio_salto_ang)*Time.deltaTime;
        //* Time.deltaTime;

        if (personaggio_animazione != 3)
        {
            //La velocità non cresce all'infinito xkè ogni cicolo riduco
            personaggio_velo *= .8f;
            //Lo posiziono a 0
            personaggio_alt = 0;

            if (Input.GetKey(KeyCode.W))
            {
                personaggio_velo += 3 * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.A))
            {
                personaggio_pos[4] -= 200 * (Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                personaggio_pos[4] += 200 * (Time.deltaTime);
            }
        }
        else if(startJumpTime > 0)
        {
            personaggio_velo *= .99f;
            float deltaTime = (Time.time - startJumpTime);
            float fracTime = deltaTime / TEMPO_DI_VOLO;
            //Debug.Log(deltaTime);

            personaggio_salto_ang = Mathf.Lerp(0, Mathf.PI, fracTime) ; //180
            personaggio_alt = 3 * Mathf.Sin(personaggio_salto_ang);

            if(fracTime >= 1)
            {
                startJumpTime = -1;
                personaggio_animazione = 0;
            }
        }

        if (personaggio_animazione != 3 && Input.GetKeyDown(KeyCode.Space) && startJumpTime == -1)
        {
            startJumpTime = Time.time;
            Jump();
        }



        //Compenso rotazione del modello rispetto alle nostre convenzioni per applicarla nel movimento
	    float rad = (360-personaggio_pos[4])*Mathf.Deg2Rad;
	    //Uso il coseno e seno della rotazione della y per regolare la direzione in base alla mia rotazione
	    personaggio_pos[0] = personaggio_pos[0] + Mathf.Cos(rad)*personaggio_velo;
	    personaggio_pos[1] = personaggio_alt;
	    personaggio_pos[2] = personaggio_pos[2] + Mathf.Sin(rad)*personaggio_velo;
	
	
	
	    personaggio.transform.position = new Vector3(personaggio_pos[0],personaggio_pos[1],personaggio_pos[2]);
	    //Costante di rotazione per sistemare gli assi del modello con convenzione diversa
	    personaggio.transform.localEulerAngles = new Vector3(0,personaggio_pos[4]+90,0);
	
	    //Passaggio animazioni
	    if(personaggio_animazione != 3){
		    if(personaggio_velo < .1){
			    Idle();
		    }
		    else{
			    Run();
		    }
	    }
        else
        {
            Jump();
        }
	
	}

    void Idle()
    {
        if (personaggio_animazione != 1)
        {
            Debug.Log("Idle");
            personaggio_animazione = 1;
            personaggio.animation.CrossFade("Idle", 0.4f);
            personaggio.animation.wrapMode = WrapMode.Loop;
            personaggio.animation["Idle"].speed = 1;
        }
    }

    void Run()
    {
        if (personaggio_animazione != 2)
        {
            Debug.Log("Run");
            personaggio_animazione = 2;
            personaggio.animation.CrossFade("Run", 0.2f);
            personaggio.animation.wrapMode = WrapMode.Loop;
        }
        personaggio.animation["Run"].speed = personaggio_velo * 1;
    }

    void Jump()
    {
        if (personaggio_animazione != 3)
        {
            Debug.Log("Jump");
            personaggio_animazione = 3;
            personaggio.animation.CrossFade("Jump", 0.1f);
            personaggio.animation.wrapMode = WrapMode.Once;
            personaggio.animation["Jump"].speed = .85f;
        }
    }
}
