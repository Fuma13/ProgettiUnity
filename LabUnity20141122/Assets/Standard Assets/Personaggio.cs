using UnityEngine;
using System.Collections;

public class Personaggio : MonoBehaviour {

    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public GameObject cam;
    public GameObject eroe;

    private Vector3 moveDirection = Vector3.zero;

    public float distanza_cam = 5;
    public float altezza_cam = 8;
    public float altezza_visuale = 1;

    private int personaggio_anim;

    private float indice_angolo;
    private CharacterController controller;

    private int vitaEroe;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();

        indice_angolo = 0;
        vitaEroe = 20;
    }
	

	void Update () {

        if (vitaEroe > 0)
        {

            Vector3 posizione_vecchia = transform.position;
            float verticalAxis = Input.GetAxisRaw("Vertical");
            float horizontalAxis = Input.GetAxisRaw("Horizontal");

            if (controller.isGrounded)
            {
                moveDirection = new Vector3(horizontalAxis, 0, verticalAxis);
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= speed;
                if (Input.GetButton("Jump"))
                {
                    moveDirection.y = jumpSpeed;
                }
            }
            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);

            if (verticalAxis != 0 || horizontalAxis != 0)
                indice_angolo = (Mathf.Sign(horizontalAxis) * Mathf.Abs(verticalAxis - 2)) - (1 - Mathf.Abs(horizontalAxis)) * verticalAxis;

            eroe.transform.localEulerAngles = new Vector3(0, Mathf.LerpAngle(eroe.transform.localEulerAngles.y, indice_angolo * 45, .1f), 0);
            //eroe.transform.localEulerAngles = new Vector3(0, indice_angolo * 45, 0);


            float angolo_y = transform.localEulerAngles.y;
            float rad = angolo_y * Mathf.Deg2Rad;

            float xx = transform.position.x - Mathf.Sin(rad) * distanza_cam;
            float yy = transform.position.y + altezza_cam;
            float zz = transform.position.z - Mathf.Cos(rad) * distanza_cam;

            cam.transform.position = new Vector3(xx, yy, zz);

            float xx2 = transform.position.x;
            float yy2 = transform.position.y + .3f;
            float zz2 = transform.position.z;

            cam.transform.LookAt(new Vector3(xx2, yy2, zz2));


            if (((posizione_vecchia.x - transform.position.x) + (posizione_vecchia.z - transform.position.z)) == 0)
            {
                Idle();
            }
            else
            {
                Run();
            }

            if (Input.GetButton("Fire1"))
                Attack();
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
            eroe.animation.CrossFade("idle", 0.3f);
            eroe.animation.wrapMode = WrapMode.Loop;
            eroe.animation["idle"].speed = 0.3f;
        }
    }

    void Run()
    {
        if (personaggio_anim != 0)
        {
            personaggio_anim = 0;
            eroe.animation.CrossFade("run", 0.1f);
            eroe.animation.wrapMode = WrapMode.Loop;
            eroe.animation["run"].speed = 1.2f;
        }
    }

    void Attack()
    {
        if (personaggio_anim != 2)
        {
            personaggio_anim = 2;
            eroe.animation.CrossFade("sparo0", 0.1f);
            eroe.animation.wrapMode = WrapMode.Loop;
            eroe.animation["sparo0"].speed = 1.5f;
        }
    }

    void Muori()
    {
        if (personaggio_anim != 3)
        {
            personaggio_anim = 3;
            eroe.animation.CrossFade("sconfitta0", 0.1f);
            eroe.animation.wrapMode = WrapMode.ClampForever;
            eroe.animation["sconfitta0"].speed = 1.5f;
        }
    }

    public bool StoAttaccando()
    {
        return (personaggio_anim == 2 && eroe.animation["sparo0"].normalizedTime > 0.25);
    }

    public int GetVita()
    {
        return vitaEroe;
    }

    public void Colpito()
    {
        if(vitaEroe > 0)
            vitaEroe--;
    }



}
