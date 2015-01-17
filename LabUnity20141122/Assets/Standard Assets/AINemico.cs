using UnityEngine;
using System.Collections;

public class AINemico : MonoBehaviour {

    public float speed = 2.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float sqrDistSegue = 50.0f;
    public float sqrDistAttacca = 6.0f;
    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 jumpDirection = Vector3.zero;
    private int indiceAngolo;
    private float deltaMovimento = 0.001f;

    private int personaggio_anim;
    
    public GameObject orco;
    public GameObject eroe;

    private int vitaNemico;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        indiceAngolo = 0;
        vitaNemico = 10;
    }
	

	void Update () {

        if (vitaNemico > 0)
        {

            Vector3 posizione_vecchia = transform.position;

            Vector3 direzione = eroe.transform.position - transform.position;
            float distanza = direzione.sqrMagnitude;
            //MOVIMENTO
            if (controller.isGrounded)
            {
                if (distanza < sqrDistSegue && distanza > sqrDistAttacca)
                {
                    moveDirection = new Vector3(direzione.normalized.x, 0, direzione.normalized.z);
                    moveDirection = transform.TransformDirection(moveDirection);
                    moveDirection *= speed;
                }
                else
                {
                    moveDirection = new Vector3(0, 0, 0);
                }
            }
            //moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);

            //SALTO
            bool fermo = (((posizione_vecchia - transform.position).sqrMagnitude) < deltaMovimento * Time.deltaTime);

            if (fermo && controller.isGrounded)
            {
                if (distanza < sqrDistSegue && distanza > sqrDistAttacca)
                {
                    if (direzione.y > 0.1f)
                        jumpDirection.y = jumpSpeed;
                }
            }

            jumpDirection.y -= gravity * Time.deltaTime;
            controller.Move(jumpDirection * Time.deltaTime);

            //ROTAZIONE
            float angolo = Mathf.Rad2Deg * Mathf.Atan2(direzione.x, direzione.z);
            angolo = (360 * 5 + angolo) % 360;  //Lo rendo positivo

            indiceAngolo = (int)angolo / 45;

            orco.transform.localEulerAngles = new Vector3(0, Mathf.LerpAngle(orco.transform.localEulerAngles.y, (indiceAngolo * 45), .05f), 0);
            //orco.transform.LookAt(new Vector3(eroe.transform.position.x, 0, eroe.transform.position.z));

            //ANIMAZIONE
            if (fermo)
            {
                if (distanza < sqrDistAttacca)
                    Attack();
                else
                    Idle();
            }
            else
            {
                Run();
            }


        }
        else
        {
            Muori();
        }
	
	}

    void Idle()
    {
        if (personaggio_anim != 1)
        {
            personaggio_anim = 1;
            orco.animation.CrossFade("idle", 0.3f);
            orco.animation.wrapMode = WrapMode.Loop;
            orco.animation["idle"].speed = 0.3f;
        }
    }

    void Run()
    {
        if (personaggio_anim != 0)
        {
            personaggio_anim = 0;
            orco.animation.CrossFade("run", 0.1f);
            orco.animation.wrapMode = WrapMode.Loop;
            orco.animation["run"].speed = 1.2f;
        }
    }

    void Attack()
    {
        if (personaggio_anim != 2)
        {
            personaggio_anim = 2;
            orco.animation.CrossFade("sparo0", 0.1f);
            orco.animation.wrapMode = WrapMode.Loop;
            orco.animation["sparo0"].speed = 0.6f;
        }
    }

    void Muori()
    {
        if (personaggio_anim != 3)
        {
            personaggio_anim = 3;
            orco.animation.CrossFade("sconfitta0", 0.1f);
            orco.animation.wrapMode = WrapMode.ClampForever;
            orco.animation["sconfitta0"].speed = 1.5f;
        }
    }

    public bool StoAttaccando()
    {
        float normalizedTime = orco.animation["sparo0"].normalizedTime % 1;
        //Debug.Log(normalizedTime);
        return (personaggio_anim == 2 && normalizedTime > 0.25 && normalizedTime < 0.8);
    }

    public void Colpito()
    {
        if(vitaNemico > 0)
            vitaNemico--;
    }

    public int GetVita()
    {
        return vitaNemico;
    }
}
